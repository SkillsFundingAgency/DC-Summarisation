﻿using System;
using System.Collections.Concurrent;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Main.Interfaces;
using ESFA.DC.Summarisation.Main.Model;
using ESFA.DC.Summarisation.Service.Model.Config;
using ESFA.DC.Summarisation.Service.Model;

namespace ESFA.DC.Summarisation.Main.Service
{
    public class SummarisationProcess : ISummarisationProcess
    {
        public string ProcessType => ProcessTypeConstants.Fundline;

        private readonly IFcsRepository _fcsRepository;
        private readonly IEnumerable<ISummarisationConfigProvider<CollectionPeriod>> _collectionPeriodsProviders;
        private readonly IEnumerable<ISummarisationConfigProvider<FundingType>> _fundingTypesProviders;
        private readonly ILogger _logger;
        private readonly Func<IInputDataRepository<LearningProvider>> _repositoryFactory;
        private readonly int _dataRetrievalMaxConcurrentCalls;

        private readonly IProviderSummarisationService<LearningProvider> _providerSummarisationService;
        private readonly IFundingDataRemovedService _fundingDataRemovedService;

        public SummarisationProcess(
            IFcsRepository fcsRepository,
            IEnumerable<ISummarisationConfigProvider<CollectionPeriod>> collectionPeriodsProviders,
            IEnumerable<ISummarisationConfigProvider<FundingType>> fundingTypesProviders,
            Func<IInputDataRepository<LearningProvider>> repositoryFactory,
            ISummarisationDataOptions dataOptions,
            ILogger logger,
            IProviderSummarisationService<LearningProvider> providerSummarisationService,
            IFundingDataRemovedService fundingDataRemovedService)
        {
            _fcsRepository = fcsRepository;
            _collectionPeriodsProviders = collectionPeriodsProviders;
            _fundingTypesProviders = fundingTypesProviders;
            _logger = logger;
            _repositoryFactory = repositoryFactory;
            _providerSummarisationService = providerSummarisationService;
            _fundingDataRemovedService = fundingDataRemovedService;

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

            var relevantFundingStreams = fundingTypeConfiguration.ToList()
                .SelectMany(ftc => ftc.FundingStreams.Select(fs => fs.PeriodCode)).Distinct().ToList();

            var fcsContractAllocations = await _fcsRepository.RetrieveContractAllocationsAsync(relevantFundingStreams, cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Retrieving FCS Contracts End");

            var summarisedActuals = new List<SummarisedActual>();

            ICollection<int> providerIdentifiers;

            if (summarisationMessage.Ukprn.HasValue && summarisationMessage.Ukprn > 0)
            {
                providerIdentifiers = new List<int> { summarisationMessage.Ukprn.Value };
            }
            else
            {
                providerIdentifiers = await _repositoryFactory().GetAllIdentifiersAsync(summarisationMessage.CollectionType, cancellationToken);
            }

            _logger.LogInfo($"Summarisation Wrapper: Providers to be summarised : {providerIdentifiers.Count}");

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Providers Data Start");

            var providersData = await RetrieveProvidersData(providerIdentifiers, summarisationMessage, cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Providers Data End");

            _logger.LogInfo($"Summarisation Wrapper: Summarisation Start");

            int runningCount = 1;
            int totalProviderCount = providersData.Count;

            foreach (var provider in providersData)
            {
                _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {provider.UKPRN} Start, {runningCount} / {totalProviderCount}");
                
                var providerActuals = await _providerSummarisationService.Summarise(provider, collectionPeriods, fundingTypeConfiguration, fcsContractAllocations, summarisationMessage, cancellationToken);

                summarisedActuals.AddRange(providerActuals);

                _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {provider.UKPRN} End, {runningCount++} / {totalProviderCount}");
            }

            //Funding Data Removed logic

            _logger.LogInfo($"Summarisation Process: Funding Data Removed Start");

            var actualsToCarry = await _fundingDataRemovedService.FundingDataRemovedAsync(summarisedActuals, summarisationMessage, cancellationToken);

            summarisedActuals.AddRange(actualsToCarry);

            _logger.LogInfo($"Summarisation Process: Funding Data Removed End");


            _logger.LogInfo($"Summarisation Wrapper: Summarisation End");

            return summarisedActuals;
        }

        private async Task<IReadOnlyCollection<LearningProvider>> RetrieveProvidersData(ICollection<int> providerIdentifiers, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var identifiers = new ConcurrentQueue<int>(providerIdentifiers);

            var concurrentBag = new ConcurrentBag<LearningProvider>();

            var providerIdentifiersList = providerIdentifiers.ToList();

            var tasks = Enumerable.Range(1, _dataRetrievalMaxConcurrentCalls).Select(async _ =>
            {
                int totalCount = providerIdentifiers.Count;

                while (identifiers.TryDequeue(out int identifier))
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

        private async Task<LearningProvider> RetrieveProviderData(int identifier, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var repo = _repositoryFactory();

            return await repo.ProvideAsync(identifier, summarisationMessage, cancellationToken);
        }
    }
}
