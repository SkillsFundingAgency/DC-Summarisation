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
        private readonly ISummarisationService _summarisationService;
        private readonly ISummarisationConfigProvider<FundingType> _fundingTypesProvider;
        private readonly ISummarisationConfigProvider<CollectionPeriod> _collectionPeriodsProvider;
        private readonly IDataStorePersistenceService _dataStorePersistenceService;
        private readonly ILogger _logger;
        private readonly Func<IProviderRepository> _repositoryFactory;
        private readonly int _maxParallelisation;

        public SummarisationWrapper(
            IFcsRepository fcsRepository,
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
            _summarisationService = summarisationService;
            _collectionPeriodsProvider = collectionPeriodsProvider;
            _dataStorePersistenceService = dataStorePersistenceService;
            _logger = logger;
            _repositoryFactory = repositoryFactory;

            _maxParallelisation = 4;
            int.TryParse(dataOptions.MaxParallelisation, out _maxParallelisation);
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
                providerIdentifiers = new List<int>();

                providerIdentifiers.Add(Convert.ToInt32(summarisationContext.Ukprn));
            }
            else
            {
                providerIdentifiers = await _repositoryFactory.Invoke().GetAllProviderIdentifiersAsync(cancellationToken);
            }

            var providersData = await RetrieveProvidersData(providerIdentifiers, _maxParallelisation, cancellationToken);

            foreach (var ukprn in providerIdentifiers)
            {
                foreach (var SummarisationType in summarisationContext.SummarisationTypes)
                {
                    _logger.LogInfo($"Summarisation Wrapper: Summarising Fundmodel {SummarisationType} Start");

                    summarisedActuals.AddRange(SummariseByFundModel(SummarisationType, collectionPeriods, fcsContractAllocations, providersData[ukprn]));

                    _logger.LogInfo($"Summarisation Wrapper: Summarising Fundmodel {SummarisationType} End");
                }
            }

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

        private async Task<IDictionary<int, IProvider>> RetrieveProvidersData(IList<int> providerIdentifiers, int maxDegreesOfParallelism, CancellationToken cancellationToken)
        {
            var identifiers = new ConcurrentQueue<int>(providerIdentifiers);

            var tasks = Enumerable.Range(1, maxDegreesOfParallelism).Select(async _ =>
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
    }
}
