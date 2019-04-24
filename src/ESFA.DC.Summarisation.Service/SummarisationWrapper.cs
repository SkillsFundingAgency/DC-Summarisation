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

namespace ESFA.DC.Summarisation.Service
{
    public class SummarisationWrapper : ISummarisationWrapper
    {
        private const int PageSize = 1;

        private readonly IFcsRepository _fcsRepository;
        private readonly IProviderRepository _repository;
        private readonly ISummarisationService _summarisationService;
        private readonly ISummarisationConfigProvider<FundingType> _fundingTypesProvider;
        private readonly ISummarisationConfigProvider<CollectionPeriod> _collectionPeriodsProvider;
        private readonly IDataStorePersistenceService _dataStorePersistenceService;

        public SummarisationWrapper(
            IFcsRepository fcsRepository,
            ISummarisationConfigProvider<FundingType> fundingTypesProvider,
            ISummarisationConfigProvider<CollectionPeriod> collectionPeriodsProvider,
            IProviderRepository repository,
            ISummarisationService summarisationService,
            IDataStorePersistenceService dataStorePersistenceService)
        {
            _fundingTypesProvider = fundingTypesProvider;
            _fcsRepository = fcsRepository;
            _repository = repository;
            _summarisationService = summarisationService;
            _collectionPeriodsProvider = collectionPeriodsProvider;
            _dataStorePersistenceService = dataStorePersistenceService;
        }

        public async Task<IEnumerable<SummarisedActual>> Summarise(ISummarisationContext summarisationContext, ILogger logger, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Summarisation Wrapper: Retrieving Collection Periods Start");

            var collectionPeriods = _collectionPeriodsProvider.Provide();

            logger.LogInfo($"Summarisation Wrapper: Retrieving Collection Periods End");

            logger.LogInfo($"Summarisation Wrapper: Retrieving FCS Contracts Start");

            var fcsContractAllocations = await _fcsRepository.RetrieveAsync(cancellationToken);

            logger.LogInfo($"Summarisation Wrapper: Retrieving FCS Contracts End");

            var summarisedActuals = new List<SummarisedActual>();

            foreach (var SummarisationType in summarisationContext.SummarisationTypes)
            {
                logger.LogInfo($"Summarisation Wrapper: Summarising Fundmodel {SummarisationType} Start");

                summarisedActuals.AddRange(await SummariseByFundModel(summarisationContext.CollectionType, SummarisationType, collectionPeriods, fcsContractAllocations, logger, cancellationToken));

                logger.LogInfo($"Summarisation Wrapper: Summarising Fundmodel {SummarisationType} End");
            }

            logger.LogInfo($"Summarisation Wrapper: Storing data to Summarised Actuals Start");

            await _dataStorePersistenceService.StoreSummarisedActualsDataAsync(summarisedActuals, summarisationContext, cancellationToken);

            logger.LogInfo($"Summarisation Wrapper: Storing data to Summarised Actuals End");

            return summarisedActuals;
        }

        private async Task<IEnumerable<SummarisedActual>> SummariseByFundModel(
           string collectionType,
           string summarisationType,
           IEnumerable<CollectionPeriod> collectionPeriods,
           IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fcsContractAllocations,
           ILogger logger,
           CancellationToken cancellationToken)
        {
            var fundingStreams = _fundingTypesProvider.Provide().Where(x => x.SummarisationType == summarisationType).SelectMany(fs => fs.FundingStreams).ToList();

            return await SummariseProviders(fundingStreams, collectionPeriods, fcsContractAllocations, logger, cancellationToken);
        }

        public async Task<IEnumerable<SummarisedActual>> SummariseProviders(
            IList<FundingStream> fundingStreams,
            IEnumerable<CollectionPeriod> collectionPeriods,
            IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fcsContractAllocations,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            var providerIdentifiers = await _repository.GetAllProviderIdentifiersAsync(cancellationToken);

            var actuals = new List<SummarisedActual>();

            foreach (var identifier in providerIdentifiers)
            {
                var provider = await _repository.ProvideAsync(identifier, cancellationToken);

                logger.LogInfo($"Summarisation Wrapper: Summarising UKPRN: {provider.UKPRN} Start");

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

                logger.LogInfo($"Summarisation Wrapper: Summarising UKPRN: {provider.UKPRN} End");
            }

            return actuals;
        }
    }
}
