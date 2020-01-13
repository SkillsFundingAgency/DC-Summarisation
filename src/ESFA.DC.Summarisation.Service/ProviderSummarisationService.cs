using System;
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
using ESFA.DC.Summarisation.Service.EqualityComparer;
using SummarisedActual = ESFA.DC.Summarisation.Data.Output.Model.SummarisedActual;
using ESFA.DC.Summarisation.Constants;

namespace ESFA.DC.Summarisation.Service
{
    public class ProviderSummarisationService : IProviderSummarisationService
    {
        private readonly ISummarisedActualsProcessRepository _summarisedActualsProcessRepository;
        private readonly IEnumerable<ISummarisationService> _summarisationServices;
        private readonly IEnumerable<ISummarisationConfigProvider<FundingType>> _fundingTypesProviders;
        private readonly ILogger _logger;
        private readonly IProviderContractsService _providerContractsService;

        public ProviderSummarisationService(
            IFcsRepository fcsRepository,
            ISummarisedActualsProcessRepository summarisedActualsProcessRepository,
            IEnumerable<ISummarisationConfigProvider<FundingType>> fundingTypesProviders,
            IEnumerable<ISummarisationService> summarisationServices,
            IDataStorePersistenceService dataStorePersistenceService,
            Func<IProviderRepository> repositoryFactory,
            ISummarisationDataOptions dataOptions,
            ILogger logger,
            IProviderContractsService providerContractsService)
        {
            _fundingTypesProviders = fundingTypesProviders;
            _summarisedActualsProcessRepository = summarisedActualsProcessRepository;
            _summarisationServices = summarisationServices;
            _logger = logger;
            _providerContractsService = providerContractsService;
        }

        public async Task<IEnumerable<SummarisedActual>> SummariseProviderData(ILearningProvider providerData, IEnumerable<CollectionPeriod> collectionPeriods, IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fcsContractAllocations, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var latestCollectionReturn = await _summarisedActualsProcessRepository.GetLastCollectionReturnForCollectionTypeAsync(summarisationMessage.CollectionType, cancellationToken);

            var providerActuals = new List<SummarisedActual>();

            foreach (var summarisationType in summarisationMessage.SummarisationTypes)
            {
                if (!summarisationType.Equals(ConstantKeys.ReRunSummarisation, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {providerData.UKPRN}, Fundmodel {summarisationType} Start");

                    var providerfundingstreamsContracts = await _providerContractsService.GetProviderContracts(providerData.UKPRN, summarisationMessage.CollectionType, summarisationType, cancellationToken);

                    var summarisationService = _summarisationServices
                        .FirstOrDefault(x => x.ProcessType.Equals(summarisationMessage.ProcessType, StringComparison.OrdinalIgnoreCase));

                    if (summarisationService == null)
                    {
                        _logger.LogInfo($"Summarisation Wrapper: Summarising UKPRN: {providerData.UKPRN} End; Summarisation service found null for Process Type: {summarisationMessage.ProcessType} ");
                        continue;
                    }

                    providerActuals.AddRange(summarisationService.Summarise(providerfundingstreamsContracts.FundingStreams, providerData, providerfundingstreamsContracts.FcsContractAllocations, collectionPeriods, summarisationMessage));

                    _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {providerData.UKPRN}, Fundmodel {summarisationType} End");
                }

            }

            if (!summarisationMessage.ProcessType.Equals(ProcessTypeConstants.Payments, StringComparison.OrdinalIgnoreCase))
            {
                var organisationId = providerActuals.Select(x => x.OrganisationId).FirstOrDefault();

                if (latestCollectionReturn != null)
                {
                    _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed  Rule UKPRN: {providerData.UKPRN} Start");

                    var actualsToCarry = await GetFundingDataRemoved(latestCollectionReturn.Id, organisationId, providerActuals, cancellationToken);
                    providerActuals.AddRange(actualsToCarry);

                    _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed  Rule UKPRN: {providerData.UKPRN} End");
                }
            }

            return providerActuals;
        }

        public async Task<IEnumerable<SummarisedActual>> GetFundingDataRemoved(
            int collectionReturnId,
            string organisationId,
            IEnumerable<SummarisedActual> providerActuals,
            CancellationToken cancellationToken)
        {
            var previousActuals = await _summarisedActualsProcessRepository
                .GetSummarisedActualsForCollectionReturnAndOrganisationAsync(collectionReturnId, organisationId, cancellationToken);

            var comparer = new CarryOverActualsComparer();

            var actualsToCarry = previousActuals.Except(providerActuals, comparer);

            foreach (var actuals in actualsToCarry)
            {
                actuals.ActualVolume = 0;
                actuals.ActualValue = 0;
            }

            return actualsToCarry;
        }
    }
}
