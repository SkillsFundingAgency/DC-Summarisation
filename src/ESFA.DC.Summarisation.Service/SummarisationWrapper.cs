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

        public SummarisationWrapper(
            IFcsRepository fcsRepository,
            ISummarisationConfigProvider<FundingType> fundingTypesProvider,
            ISummarisationConfigProvider<CollectionPeriod> collectionPeriodsProvider,
            ISummarisationService summarisationService,
            IDataStorePersistenceService dataStorePersistenceService,
            Func<IProviderRepository> repositoryFactory,
            ILogger logger)
        {
            _fundingTypesProvider = fundingTypesProvider;
            _fcsRepository = fcsRepository;
            _summarisationService = summarisationService;
            _collectionPeriodsProvider = collectionPeriodsProvider;
            _dataStorePersistenceService = dataStorePersistenceService;
            _logger = logger;
            _repositoryFactory = repositoryFactory;
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

            var providersData = new ConcurrentDictionary<int, IProvider>();

            RetrieveProvidersData(providerIdentifiers, providersData, cancellationToken);

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

        private void RetrieveProvidersData(IList<int> providerIdentifiers, ConcurrentDictionary<int, IProvider> providerDictionary, CancellationToken cancellationToken)
        {
            var identifiers = new ConcurrentBag<int>(providerIdentifiers);

            List<Task> retrievalTasks = new List<Task>
            {
                RetrieveProviderData(identifiers, providerDictionary, cancellationToken),
                RetrieveProviderData(identifiers, providerDictionary, cancellationToken),
                RetrieveProviderData(identifiers, providerDictionary, cancellationToken),
                RetrieveProviderData(identifiers, providerDictionary, cancellationToken)
            };

            Task.WaitAll(retrievalTasks.ToArray());
        }

        private async Task RetrieveProviderData(ConcurrentBag<int> identifiers, ConcurrentDictionary<int, IProvider> providerDictionary, CancellationToken cancellationToken)
        {
            int providerIdentifier;
            while (!identifiers.IsEmpty)
            {
                var repo = _repositoryFactory.Invoke();
                if (identifiers.TryTake(out providerIdentifier))
                {
                    _logger.LogInfo($"Summarisation Wrapper: Retrieving Data for UKPRN: {providerIdentifier} Start");
                    var providerData = await repo.ProvideAsync(providerIdentifier, cancellationToken);
                    _logger.LogInfo($"Summarisation Wrapper: Retrieving Data for UKPRN: {providerIdentifier} End");


                    providerDictionary.TryAdd(providerIdentifier, providerData);
                }
            }
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
    }
}
