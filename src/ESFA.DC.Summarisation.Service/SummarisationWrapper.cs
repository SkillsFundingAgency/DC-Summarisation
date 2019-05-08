using System;
using System.Collections.Concurrent;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Output.Model;
using ESFA.DC.Summarisation.Data.Persist;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interface;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Configuration.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;

namespace ESFA.DC.Summarisation.Service
{
    public class SummarisationWrapper : ISummarisationWrapper
    {
        private readonly IFcsRepository _fcsRepository;
        private readonly ISummarisedActualsRepository _summarisedActualsRepository;
        private readonly ISummarisationService _summarisationService;
        private readonly ISummarisationConfigProvider<FundingType> _fundingTypesProvider;
        private readonly ISummarisationConfigProvider<CollectionPeriod> _collectionPeriodsProvider;
        private readonly IDataStorePersistenceService _dataStorePersistenceService;
        private readonly ILogger _logger;
        private readonly Func<IProviderRepository> _repositoryFactory;
        private readonly int _dataRetrievalMaxConcurrentCalls;

        public SummarisationWrapper(
            IFcsRepository fcsRepository,
            ISummarisedActualsRepository summarisedActualsRepository,
            ISummarisationConfigProvider<FundingType> fundingTypesProvider,
            ISummarisationConfigProvider<CollectionPeriod> collectionPeriodsProvider,
            ISummarisationService summarisationService,
            IDataStorePersistenceService dataStorePersistenceService,
            Func<IProviderRepository> repositoryFactory,
            ISummarisationDataOptions dataOptions,
            ILogger logger)
        {
            _fundingTypesProvider = fundingTypesProvider;
            _fcsRepository = fcsRepository;
            _summarisedActualsRepository = summarisedActualsRepository;
            _summarisationService = summarisationService;
            _collectionPeriodsProvider = collectionPeriodsProvider;
            _dataStorePersistenceService = dataStorePersistenceService;
            _logger = logger;
            _repositoryFactory = repositoryFactory;

            _dataRetrievalMaxConcurrentCalls = 4;
            int.TryParse(dataOptions.DataRetrievalMaxConcurrentCalls, out _dataRetrievalMaxConcurrentCalls);
        }

        public async Task<IEnumerable<SummarisedActual>> Summarise(ISummarisationContext summarisationContext, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Summarisation Wrapper: Retrieving Collection Periods Start");

            var collectionPeriods = _collectionPeriodsProvider.Provide();

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Collection Periods End");

            _logger.LogInfo($"Summarisation Wrapper: Retrieving FCS Contracts Start");

            var fcsContractAllocations = await _fcsRepository.RetrieveAsync(cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Retrieving FCS Contracts End");

            var summarisedActuals = new List<SummarisedActual>();

            IList<int> providerIdentifiers;

            if (!string.IsNullOrEmpty(summarisationContext.Ukprn))
            {
                providerIdentifiers = new List<int> { Convert.ToInt32(summarisationContext.Ukprn) };

            }
            else
            {
                providerIdentifiers = await _repositoryFactory.Invoke().GetAllProviderIdentifiersAsync(cancellationToken);
            }

            var providersData = await RetrieveProvidersData(providerIdentifiers, cancellationToken);

            foreach (var ukprn in providerIdentifiers)
            {
                foreach (var SummarisationType in summarisationContext.SummarisationTypes)
                {
                    _logger.LogInfo($"Summarisation Wrapper: Summarising Fundmodel {SummarisationType} Start");

                    summarisedActuals.AddRange(SummariseByFundModel(SummarisationType, collectionPeriods, fcsContractAllocations, providersData[ukprn]));

                    _logger.LogInfo($"Summarisation Wrapper: Summarising Fundmodel {SummarisationType} End");
                }
                
            }

            _logger.LogInfo($"Summarisation Wrapper: Reteieve Latest Summarised Actuals Start");

            var summarisedActualsLast = await _summarisedActualsRepository.GetLatestSummarisedActualsAsync(summarisationContext.CollectionType, cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Reteieve Latest Summarised Actuals End");

            _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed Start");

            summarisedActuals.AddRange(GetFundingDataRemoved(summarisedActualsLast, summarisedActuals));

            _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed End");

            _logger.LogInfo($"Summarisation Wrapper: Storing data to Summarised Actuals Start");

            await _dataStorePersistenceService.StoreSummarisedActualsDataAsync(summarisedActuals.ToList(), summarisationContext, cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Storing data to Summarised Actuals End");

            return summarisedActuals;
        }

        private IEnumerable<SummarisedActual> SummariseByFundModel(
           string summarisationType,
           IEnumerable<CollectionPeriod> collectionPeriods,
           IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fcsContractAllocations,
           IProvider provider)
        {
            var fundingStreams = _fundingTypesProvider.Provide().Where(x => x.SummarisationType == summarisationType).SelectMany(fs => fs.FundingStreams).ToList();

            var actuals = new List<SummarisedActual>();

            _logger.LogInfo($"Summarisation Wrapper: Summarising UKPRN: {provider.UKPRN} Start");

            var contractFundingStreams = new List<FundingStream>();
            var allocations = new List<IFcsContractAllocation>();

            foreach (var fs in fundingStreams)
            {
                if (fcsContractAllocations.ContainsKey(fs.PeriodCode) && fcsContractAllocations[fs.PeriodCode].Any(x => x.DeliveryUkprn == provider.UKPRN))
                {
                    contractFundingStreams.Add(fs);
                    allocations.Add(fcsContractAllocations[fs.PeriodCode].First(x => x.DeliveryUkprn == provider.UKPRN));
                }
            }

            actuals.AddRange(_summarisationService.Summarise(contractFundingStreams, provider, allocations, collectionPeriods));

            _logger.LogInfo($"Summarisation Wrapper: Summarising UKPRN: {provider.UKPRN} End");

            return actuals;
        }

        private async Task<IDictionary<int, IProvider>> RetrieveProvidersData(IList<int> providerIdentifiers, CancellationToken cancellationToken)
        {
            var identifiers = new ConcurrentQueue<int>(providerIdentifiers);

            var tasks = Enumerable.Range(1, _dataRetrievalMaxConcurrentCalls).Select(async _ =>
            {
                var dictionary = new Dictionary<int, IProvider>();

                while (identifiers.TryDequeue(out int identifier))
                {
                    dictionary.Add(identifier, await RetrieveProviderData(identifier, cancellationToken));
                }

                return dictionary;
            }).ToList();

            await Task.WhenAll(tasks);

            return tasks.SelectMany(t => t.Result).ToDictionary(p => p.Key, p => p.Value);
        }

        private async Task<IProvider> RetrieveProviderData(int identifier, CancellationToken cancellationToken)
        {
            var repo = _repositoryFactory.Invoke();

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Data for UKPRN: {identifier} Start");
            var providerData = await repo.ProvideAsync(identifier, cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Data for UKPRN: {identifier} End");

            return providerData;
        }

        public IEnumerable<SummarisedActual> GetFundingDataRemoved(
            IEnumerable<SummarisedActual> summarisedActualsLast,
            IEnumerable<SummarisedActual> summarisedActualsCurrent)
        {
            var fundingRemovedActuals = new List<SummarisedActual>();

            fundingRemovedActuals = summarisedActualsLast.GroupJoin(
                    summarisedActualsCurrent,
                    last => new { last.OrganisationId, last.FundingStreamPeriodCode, last.DeliverableCode, last.Period, last.UoPCode },
                    current => new { current.OrganisationId, current.FundingStreamPeriodCode, current.DeliverableCode, current.Period, current.UoPCode },
                    (last, current) => new { last, current })
                .SelectMany(
                    x => x.current.DefaultIfEmpty(),
                    (x, y) => new { x.last, current = y })
                .Where(x => x.current == null)
                .Select(x => x.last).ToList();

            return fundingRemovedActuals;
        }
    }
}
