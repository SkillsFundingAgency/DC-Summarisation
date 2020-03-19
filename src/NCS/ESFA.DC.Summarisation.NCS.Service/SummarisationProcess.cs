using System;
using System.Collections.Concurrent;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.NCS.Model.Config;
using ESFA.DC.Summarisation.NCS.Interfaces;
using ESFA.DC.Summarisation.NCS.Model;

namespace ESFA.DC.Summarisation.NCS.Service
{
    public class SummarisationProcess : ISummarisationProcess
    {
        public string ProcessType => ProcessTypeConstants.Ncs;

        private readonly IFcsRepository _fcsRepository;
        private readonly IEnumerable<ISummarisationConfigProvider<CollectionPeriod>> _collectionPeriodsProviders;
        private readonly IEnumerable<ISummarisationConfigProvider<FundingType>> _fundingTypesProviders;
        private readonly ILogger _logger;
        private readonly Func<IInputDataRepository<TouchpointProviderFundingData>> _repositoryFactory;
        private readonly int _dataRetrievalMaxConcurrentCalls;

        private readonly IProviderSummarisationService<TouchpointProviderFundingData> _providerSummarisationService;

        public SummarisationProcess(
            IFcsRepository fcsRepository,
            IEnumerable<ISummarisationConfigProvider<CollectionPeriod>> collectionPeriodsProviders,
            IEnumerable<ISummarisationConfigProvider<FundingType>> fundingTypesProviders,
            Func<IInputDataRepository<TouchpointProviderFundingData>> repositoryFactory,
            ISummarisationDataOptions dataOptions,
            ILogger logger,
            IProviderSummarisationService<TouchpointProviderFundingData> providerSummarisationService)
        {
            _fcsRepository = fcsRepository;
            _collectionPeriodsProviders = collectionPeriodsProviders;
            _fundingTypesProviders = fundingTypesProviders;
            _logger = logger;
            _repositoryFactory = repositoryFactory;
            _providerSummarisationService = providerSummarisationService;

            _dataRetrievalMaxConcurrentCalls = 4;
            int.TryParse(dataOptions.DataRetrievalMaxConcurrentCalls, out _dataRetrievalMaxConcurrentCalls);
        }

        public async Task<ICollection<SummarisedActual>> CollateAndSummariseAsync(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Collection Periods Start");

            _logger.LogInfo($"Summarisation Message: CollectionType : {summarisationMessage.CollectionType}, CollectionReturnCode: {summarisationMessage.CollectionReturnCode}, ILRCollectionYear: {summarisationMessage.CollectionYear}, ILRReturnPeriod: {summarisationMessage.CollectionMonth}");

            var collectionPeriods = _collectionPeriodsProviders.Single(w => w.CollectionType.Equals(summarisationMessage.CollectionType, StringComparison.OrdinalIgnoreCase)).Provide();

            var fundingTypeConfiguration = _fundingTypesProviders.Single(w => w.CollectionType.Equals(summarisationMessage.CollectionType, StringComparison.OrdinalIgnoreCase)).Provide();

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Collection Periods End");

            _logger.LogInfo($"Summarisation Wrapper: Retrieving FCS Contracts Start");

            var fcsContractAllocations = await _fcsRepository.RetrieveContractAllocationsAsync(FundingStreamConstants.NCSFundingStreams_1920, cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Retrieving FCS Contracts End");

            var summarisedActuals = new List<SummarisedActual>();

            ICollection<TouchpointProvider> providerIdentifiers;

            providerIdentifiers = await _repositoryFactory.Invoke().GetAllIdentifiersAsync(cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Providers to be summarised : {providerIdentifiers.Count}");

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Providers Data Start");

            var providersData = await RetrieveProvidersData(providerIdentifiers.ToList(), summarisationMessage, cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Providers Data End");

            _logger.LogInfo($"Summarisation Wrapper: Summarisation Start");

            int runningCount = 1;
            int totalProviderCount = providerIdentifiers.Count;

            foreach (var provider in providerIdentifiers)
            {
                _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {provider.UKPRN} UKPRN: {provider.TouchpointId} Start, {runningCount} / {totalProviderCount}");

                var providerData = providersData.First(q => q.Provider.UKPRN == provider.UKPRN && q.Provider.TouchpointId.Equals(provider.TouchpointId,StringComparison.OrdinalIgnoreCase));

                var providerActuals = await _providerSummarisationService.Summarise(providerData, collectionPeriods, fundingTypeConfiguration, fcsContractAllocations, summarisationMessage, cancellationToken);

                summarisedActuals.AddRange(providerActuals);

                _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {provider.UKPRN} UKPRN: {provider.TouchpointId} End, {runningCount++} / {totalProviderCount}");
            }

            _logger.LogInfo($"Summarisation Wrapper: Summarisation End");

            return summarisedActuals;
        }

        private async Task<IReadOnlyCollection<TouchpointProviderFundingData>> RetrieveProvidersData(ICollection<TouchpointProvider> providerIdentifiers, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var identifiers = new ConcurrentQueue<TouchpointProvider>(providerIdentifiers);

            var concurrentBag = new ConcurrentBag<TouchpointProviderFundingData>();

            var providerIdentifiersList = providerIdentifiers.ToList();

            var tasks = Enumerable.Range(1, _dataRetrievalMaxConcurrentCalls).Select(async _ =>
            {
                int totalCount = providerIdentifiers.Count;

                while (identifiers.TryDequeue(out TouchpointProvider identifier))
                {
                    var index = providerIdentifiersList.IndexOf(identifier) + 1;

                    _logger.LogInfo($"Summarisation Wrapper: Retrieving Data for UKPRN: {identifier} Start, {index} / {totalCount}");

                    var learningProvider = await RetrieveProviderData(identifier, summarisationMessage, cancellationToken);

                    concurrentBag.Add(learningProvider);

                    _logger.LogInfo($"Summarisation Wrapper: Retrieving Data for UKPRN: {identifier} End, {index} / {totalCount}");
                }
            }).ToList();

            await Task.WhenAll(tasks);

            return concurrentBag;
        }

        private async Task<TouchpointProviderFundingData> RetrieveProviderData(TouchpointProvider identifier, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var repo = _repositoryFactory();

            return await repo.ProvideAsync(identifier, summarisationMessage, cancellationToken);
        }
    }
}
