using System;
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

            //Non-Levy FSPs ending with 2018, get data from Summarised Actuals.
            if (_summarisationMessage.CollectionType.Equals(CollectionType.Apps1920.ToString(), StringComparison.OrdinalIgnoreCase)
                || _summarisationMessage.CollectionType.Equals(CollectionType.Apps1819.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                List<string> nonlevy2018FSPs = new List<string> { ConstantKeys.NonLevy_1618NLAP2018, ConstantKeys.NonLevy_ANLAP2018 };

                var latestCollectionReturnIdApps1819 = await _summarisedActualsProcessRepository.GetLastCollectionReturnForCollectionTypeAsync(CollectionType.Apps1819.ToString(), cancellationToken);

                if (latestCollectionReturnIdApps1819 != null)
                {
                    var summarisedActuals2018 = await _summarisedActualsProcessRepository.GetSummarisedActualsForCollectionRetrunAndFSPsAsync(latestCollectionReturnIdApps1819.Id, nonlevy2018FSPs, cancellationToken);

                    summarisedActuals.AddRange(summarisedActuals2018);
                }
            }

            IList<int> providerIdentifiers;

            if (!string.IsNullOrEmpty(_summarisationMessage.Ukprn))
            {
                providerIdentifiers = new List<int> { Convert.ToInt32(_summarisationMessage.Ukprn) };
            }
            else
            {
                providerIdentifiers = await _repositoryFactory.Invoke().GetAllProviderIdentifiersAsync(_summarisationMessage.CollectionType,cancellationToken);
            }

            var latestCollectionReturn = await _summarisedActualsProcessRepository.GetLastCollectionReturnForCollectionTypeAsync(_summarisationMessage.CollectionType, cancellationToken);

            var providersData = await RetrieveProvidersData(providerIdentifiers, cancellationToken);

            foreach (var ukprn in providerIdentifiers)
            {
                var providerActuals = new List<SummarisedActual>();
                
                foreach (var SummarisationType in _summarisationMessage.SummarisationTypes)
                {
                    _logger.LogInfo($"Summarisation Wrapper: Summarising Fundmodel {SummarisationType} Start");

                    providerActuals.AddRange(SummariseByFundModel(SummarisationType, collectionPeriods, fcsContractAllocations, providersData[ukprn]));

                    _logger.LogInfo($"Summarisation Wrapper: Summarising Fundmodel {SummarisationType} End");
                }

                var organisationId = providerActuals.Select(x => x.OrganisationId).FirstOrDefault();

                if (latestCollectionReturn != null)
                {
                    var actualsToCarry = await GetFundingDataRemoved(latestCollectionReturn.Id, organisationId, providerActuals, cancellationToken);
                    providerActuals.AddRange(actualsToCarry);
                }

                if (_summarisationMessage.CollectionType.Equals(
                    CollectionType.ESF.ToString(), StringComparison.OrdinalIgnoreCase)
                    && providerActuals.Count > 0)
                {
                    _logger.LogInfo($"Summarisation Wrapper: Adding missing esf zero value summarised records Start");
                    providerActuals.AddRange(
                        GetESFMissingSummarisedActuals(
                            providerActuals,
                            collectionPeriods,
                            fcsContractAllocations.SelectMany(f => f.Value.Where(c => c.DeliveryUkprn == ukprn)).ToList()));
                    _logger.LogInfo($"Summarisation Wrapper: Adding missing esf zero value summarised records End");
                }
                summarisedActuals.AddRange(providerActuals);
            }
            
            _logger.LogInfo($"Summarisation Wrapper: Storing data to Summarised Actuals Start");
            
            await _dataStorePersistenceService.StoreSummarisedActualsDataAsync(
                summarisedActuals.ToList(),
                _summarisationMessage,
                cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Storing data to Summarised Actuals End");

            return summarisedActuals;
        }

        public IEnumerable<SummarisedActual> GetESFMissingSummarisedActuals(
            List<SummarisedActual> providerActuals,
            IEnumerable<CollectionPeriod> collectionPeriods,
            List<IFcsContractAllocation> fcsContractAllocations)
        {
            List<SummarisedActual> missingSummarisedActuals = new List<SummarisedActual>();

            var fundingTypes = GetFundingTypesData(SummarisationType.ESF_SuppData.ToString());

            if (fcsContractAllocations == null
                || fundingTypes == null
                || collectionPeriods == null)
            {
                return missingSummarisedActuals;
            }

            var summarisationCollectionPeriod = collectionPeriods
                .Where(c => c.CollectionYear == _summarisationMessage.CollectionYear
                    && c.CollectionMonth == _summarisationMessage.CollectionMonth).SingleOrDefault();
            if (summarisationCollectionPeriod == null)
            {
                return missingSummarisedActuals;
            }

            foreach (var fundType in fundingTypes)
            {
                foreach (var fcs in fcsContractAllocations
                    .Where(f => f.FundingStreamPeriodCode.Equals(fundType.PeriodCode, StringComparison.OrdinalIgnoreCase)))
                {
                    var fcsCollectionPeriods = GetCollectionPeriodsForDateRange(
                                fcs.ContractStartDate,
                                fcs.ContractEndDate,
                                summarisationCollectionPeriod,
                                collectionPeriods);

                    if ((fcsCollectionPeriods?.Count ?? 0) == 0)
                    {
                        continue;
                    }

                    foreach (var collectionPeriod in fcsCollectionPeriods)
                    {
                        SummarisedActual missingSummarisedActual = new SummarisedActual()
                        {
                            ContractAllocationNumber = fcs.ContractAllocationNumber,
                            DeliverableCode = fundType.DeliverableLineCode,
                            FundingStreamPeriodCode = fundType.PeriodCode,
                            OrganisationId = fcs.DeliveryOrganisation,
                            Period = collectionPeriod.ActualsSchemaPeriod,
                            PeriodTypeCode = PeriodTypeCode.CM.ToString(),
                            ActualVolume = 0,
                            ActualValue = 0.00M
                        };
                        if (!SummarisedActualAlreadyExist(missingSummarisedActual, providerActuals))
                        {
                            missingSummarisedActuals.Add(missingSummarisedActual);
                        }
                    }
                }
            }
            
            return missingSummarisedActuals;
        }
        
        public bool SummarisedActualAlreadyExist(
            SummarisedActual missingSummarisedActual,
            List<SummarisedActual> providerActuals)
        {
            return providerActuals?.Any(p =>
                p.ContractAllocationNumber.Equals(missingSummarisedActual.ContractAllocationNumber, StringComparison.OrdinalIgnoreCase)
                && p.Period == missingSummarisedActual.Period
                && p.DeliverableCode == missingSummarisedActual.DeliverableCode) ?? false;
        }

        public IList<CollectionPeriod> GetCollectionPeriodsForDateRange(
            int contractStartDate,
            int contractEndDate,
            CollectionPeriod summarisationCollectionPeriod,
            IEnumerable<CollectionPeriod> collectionPeriods)
        {
            IList<CollectionPeriod> collectionPeriodsRange = new List<CollectionPeriod>();

            int msgCollectionPeriod = int.Parse($"{summarisationCollectionPeriod.CalendarYear.ToString()}{summarisationCollectionPeriod.CalendarMonth.ToString("D2")}");
            
            if (contractStartDate == 0 
                || contractStartDate > msgCollectionPeriod)
            {
                return collectionPeriodsRange;
            }

            if (contractEndDate == 0
                || contractEndDate > msgCollectionPeriod)
            {
                contractEndDate = msgCollectionPeriod;
            }

            return collectionPeriods?.Where(c => c.ActualsSchemaPeriod >= contractStartDate && c.ActualsSchemaPeriod <= contractEndDate).ToList();
        }

        private IEnumerable<SummarisedActual> SummariseByFundModel(
           string summarisationType,
           IEnumerable<CollectionPeriod> collectionPeriods,
           IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fcsContractAllocations,
           IProvider provider)
        {
            var fundingStreams = GetFundingTypesData(summarisationType);

            var actuals = new List<SummarisedActual>();

            _logger.LogInfo($"Summarisation Wrapper: Summarising UKPRN: {provider.UKPRN} Start");

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
            var providerData = await repo.ProvideAsync(identifier, _summarisationMessage, cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Data for UKPRN: {identifier} End");

            return providerData;
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
            _logger.LogInfo($"Summarisation Wrapper: Retrieve Latest Summarised Actuals Start");

            var previousActuals = await _summarisedActualsProcessRepository
                .GetSummarisedActualsForCollectionReturnAndOrganisationAsync(collectionReturnId, organisationId, cancellationToken);
            var actualsToCarry = previousActuals.Except(providerActuals, new SummarisedActualsComparer());

            actualsToCarry.ToList().ForEach(a => { a.ActualVolume = 0; a.ActualValue = 0; });

            _logger.LogInfo($"Summarisation Wrapper: Retrieve Latest Summarised Actuals End");
            return actualsToCarry;
        }
    }
}
