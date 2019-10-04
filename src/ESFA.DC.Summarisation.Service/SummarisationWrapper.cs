﻿using System;
using System.Collections.Concurrent;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
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
using ESFA.DC.Summarisation.Model;
using ESFA.DC.Summarisation.Service.EqualityComparer;
using SummarisedActual = ESFA.DC.Summarisation.Data.Output.Model.SummarisedActual;
using ESFA.DC.Summarisation.Configuration.Enum;
using ESFA.DC.Summarisation.Data.Output.Model;
using ESFA.DC.Summarisation.Constants;

namespace ESFA.DC.Summarisation.Service
{
    public class SummarisationWrapper : ISummarisationWrapper
    {
        private readonly IFcsRepository _fcsRepository;
        private readonly ISummarisedActualsProcessRepository _summarisedActualsProcessRepository;
        private readonly IEnumerable<ISummarisationService> _summarisationServices;
        private readonly IEnumerable<ISummarisationConfigProvider<FundingType>> _fundingTypesProviders;
        private readonly IEnumerable<ISummarisationConfigProvider<CollectionPeriod>> _collectionPeriodsProviders;
        private readonly IDataStorePersistenceService _dataStorePersistenceService;
        private readonly ILogger _logger;
        private readonly Func<IProviderRepository> _repositoryFactory;
        private readonly int _dataRetrievalMaxConcurrentCalls;
        private readonly ISummarisationMessage _summarisationMessage;
        
        public SummarisationWrapper(
            IFcsRepository fcsRepository,
            ISummarisedActualsProcessRepository summarisedActualsProcessRepository,
            IEnumerable<ISummarisationConfigProvider<FundingType>> fundingTypesProviders,
            IEnumerable<ISummarisationConfigProvider<CollectionPeriod>> collectionPeriodsProviders,
            IEnumerable<ISummarisationService> summarisationServices,
            IDataStorePersistenceService dataStorePersistenceService,
            Func<IProviderRepository> repositoryFactory,
            ISummarisationDataOptions dataOptions,
            ILogger logger,
            ISummarisationMessage summarisationMessage)
        {
            _fundingTypesProviders = fundingTypesProviders;
            _fcsRepository = fcsRepository;
            _summarisedActualsProcessRepository = summarisedActualsProcessRepository;
            _summarisationServices = summarisationServices;
            _collectionPeriodsProviders = collectionPeriodsProviders;
            _dataStorePersistenceService = dataStorePersistenceService;
            _logger = logger;
            _repositoryFactory = repositoryFactory;
            _summarisationMessage = summarisationMessage;

            _dataRetrievalMaxConcurrentCalls = 4;
            int.TryParse(dataOptions.DataRetrievalMaxConcurrentCalls, out _dataRetrievalMaxConcurrentCalls);
        }

        public async Task<IEnumerable<SummarisedActual>> Summarise(CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Summarisation Wrapper: Retrieving Collection Periods Start");

            _logger.LogInfo($"Summarisation Message: CollectionType : {_summarisationMessage.CollectionType}, CollectionReturnCode: {_summarisationMessage.CollectionReturnCode}, ILRCollectionYear: {_summarisationMessage.CollectionYear}, ILRReturnPeriod: {_summarisationMessage.CollectionMonth}");

            var collectionPeriods = _collectionPeriodsProviders.SingleOrDefault(w => w.CollectionType == _summarisationMessage.CollectionType)?.Provide();

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Collection Periods End");

            _logger.LogInfo($"Summarisation Wrapper: Retrieving FCS Contracts Start");

            var fcsContractAllocations = await _fcsRepository.RetrieveAsync(cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Retrieving FCS Contracts End");

            var summarisedActuals = new List<SummarisedActual>();

            IList<int> providerIdentifiers;

            if (!string.IsNullOrEmpty(_summarisationMessage.Ukprn) && Convert.ToInt32(_summarisationMessage.Ukprn) > 0)
            {
                providerIdentifiers = new List<int> { Convert.ToInt32(_summarisationMessage.Ukprn) };
            }
            else
            {
                providerIdentifiers = await _repositoryFactory.Invoke().GetAllProviderIdentifiersAsync(_summarisationMessage.CollectionType,cancellationToken);
            }

            _logger.LogInfo($"Summarisation Wrapper: Providers to be summarised : {providerIdentifiers.Count}");

            var latestCollectionReturn = await _summarisedActualsProcessRepository.GetLastCollectionReturnForCollectionTypeAsync(_summarisationMessage.CollectionType, cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Providers Data Start");

            var providersData = await RetrieveProvidersData(providerIdentifiers, cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Providers Data End");

            _logger.LogInfo($"Summarisation Wrapper: Summarisation Start");

            int runningCount = 1;
            int totalProviderCount = providerIdentifiers.Count;

            foreach (var ukprn in providerIdentifiers)
            {
                _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {ukprn} Start, {runningCount} / {totalProviderCount}");

                var providerActuals = new List<SummarisedActual>();
                
                foreach (var SummarisationType in _summarisationMessage.SummarisationTypes)
                {
                    if (!SummarisationType.Equals(ConstantKeys.ReRunSummarisation, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {ukprn}, Fundmodel {SummarisationType} Start");

                        providerActuals.AddRange(SummariseByFundModel(SummarisationType, collectionPeriods, fcsContractAllocations, providersData[ukprn]));

                        _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {ukprn}, Fundmodel {SummarisationType} End");
                    }
                }

                if (!_summarisationMessage.ProcessType.Equals(ProcessType.Payments.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    var organisationId = providerActuals.Select(x => x.OrganisationId).FirstOrDefault();

                    if (latestCollectionReturn != null)
                    {
                        _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed  Rule UKPRN: {ukprn} Start");

                        var actualsToCarry = await GetFundingDataRemoved(latestCollectionReturn.Id, organisationId, providerActuals, cancellationToken);
                        providerActuals.AddRange(actualsToCarry);

                        _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed  Rule UKPRN: {ukprn} End");
                    }
                }

                summarisedActuals.AddRange(providerActuals);
                
                _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {ukprn} End, {runningCount++} / {totalProviderCount}");
            }

            _logger.LogInfo($"Summarisation Wrapper: Summarisation End");

            _logger.LogInfo($"Summarisation Wrapper: Storing data to Summarised Actuals Start");
            
            await _dataStorePersistenceService.StoreSummarisedActualsDataAsync(
                summarisedActuals.ToList(),
                _summarisationMessage,
                cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Storing data to Summarised Actuals End");

            return summarisedActuals;
        }

        private IEnumerable<SummarisedActual> SummariseByFundModel(
           string summarisationType,
           IEnumerable<CollectionPeriod> collectionPeriods,
           IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fcsContractAllocations,
           IProvider provider)
        {
            var fundingStreams = GetFundingTypesData(summarisationType);

            var actuals = new List<SummarisedActual>();

            var contractFundingStreams = new List<FundingStream>();
            var allocations = new List<IFcsContractAllocation>();

            if (fundingStreams == null)
            {
                _logger.LogInfo($"Summarisation Wrapper: Summarising UKPRN: {provider.UKPRN} End; Funding streams found null for Summarisation Type: {summarisationType} ");
                return actuals;
            }

            foreach (var fs in fundingStreams)
            {
                if (fcsContractAllocations.ContainsKey(fs.PeriodCode)
                    && fcsContractAllocations[fs.PeriodCode].Any(x => x.DeliveryUkprn == provider.UKPRN))
                {
                    contractFundingStreams.Add(fs);

                    foreach (var allocation in fcsContractAllocations[fs.PeriodCode].Where(x => x.DeliveryUkprn == provider.UKPRN))
                    {
                        if (!allocations.Any(
                            w => w.ContractAllocationNumber.Equals(allocation.ContractAllocationNumber, StringComparison.OrdinalIgnoreCase)
                                && w.FundingStreamPeriodCode.Equals(fs.PeriodCode, StringComparison.OrdinalIgnoreCase)))
                            allocations.Add(allocation);
                    }    
                }
            }

            var summarisationService = _summarisationServices
                .FirstOrDefault(x => x.ProcessType.Equals(_summarisationMessage.ProcessType, StringComparison.OrdinalIgnoreCase));

            if (summarisationService == null)
            {
                _logger.LogInfo($"Summarisation Wrapper: Summarising UKPRN: {provider.UKPRN} End; Summarisation service found null for Process Type: {_summarisationMessage.ProcessType} ");
                return actuals;
            }

            actuals.AddRange(
                summarisationService
                .Summarise(contractFundingStreams, provider, allocations, collectionPeriods, _summarisationMessage));

            return actuals;
        }

        private async Task<IDictionary<int, IProvider>> RetrieveProvidersData(IList<int> providerIdentifiers, CancellationToken cancellationToken)
        {
            var identifiers = new ConcurrentQueue<int>(providerIdentifiers);

            var tasks = Enumerable.Range(1, _dataRetrievalMaxConcurrentCalls).Select(async _ =>
            {
                var dictionary = new Dictionary<int, IProvider>();

                int totalCount = providerIdentifiers.Count;

                while (identifiers.TryDequeue(out int identifier))
                {
                    _logger.LogInfo($"Summarisation Wrapper: Retrieving Data for UKPRN: {identifier} Start, {providerIdentifiers.IndexOf(identifier) + 1} / {totalCount}");

                    dictionary.Add(identifier, await RetrieveProviderData(identifier, cancellationToken));

                    _logger.LogInfo($"Summarisation Wrapper: Retrieving Data for UKPRN: {identifier} End, {providerIdentifiers.IndexOf(identifier) + 1} / {totalCount}");
                }

                return dictionary;
            }).ToList();

            await Task.WhenAll(tasks);

            return tasks.SelectMany(t => t.Result).ToDictionary(p => p.Key, p => p.Value);
        }

        private async Task<IProvider> RetrieveProviderData(int identifier, CancellationToken cancellationToken)
        {
            var repo = _repositoryFactory.Invoke();

            return await repo.ProvideAsync(identifier, _summarisationMessage, cancellationToken);
        }

        public IList<FundingStream> GetFundingTypesData(string summarisationType)
        {
            return _fundingTypesProviders
                .FirstOrDefault(w => w.CollectionType.Equals(_summarisationMessage.CollectionType, StringComparison.OrdinalIgnoreCase))?
                .Provide().Where(x => x.SummarisationType.Equals(summarisationType, StringComparison.OrdinalIgnoreCase))
                .SelectMany(fs => fs.FundingStreams)
                .ToList();
        }

        public async Task<IEnumerable<SummarisedActual>> GetFundingDataRemoved(
            int collectionReturnId,
            string organisationId,
            IEnumerable<SummarisedActual> providerActuals,
            CancellationToken cancellationToken)
        {
            var previousActuals = await _summarisedActualsProcessRepository
                .GetSummarisedActualsForCollectionReturnAndOrganisationAsync(collectionReturnId, organisationId, cancellationToken);
            var actualsToCarry = previousActuals.Except(providerActuals, new SummarisedActualsComparer());

            actualsToCarry.ToList().ForEach(a => { a.ActualVolume = 0; a.ActualValue = 0; });

            return actualsToCarry;
        }
    }
}
