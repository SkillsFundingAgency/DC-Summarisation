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
    public class ProviderSummarisationService : IProviderSummarisationService<ILearningProvider>
    {
        private readonly IEnumerable<ISummarisationService> _summarisationServices;
        private readonly ILogger _logger;
        private readonly IProviderContractsService _providerContractsService;
        private readonly IProviderFundingDataRemovedService _providerFundingDataRemovedService;

        public ProviderSummarisationService(
            IEnumerable<ISummarisationService> summarisationServices,
            ILogger logger,
            IProviderContractsService providerContractsService,
            IProviderFundingDataRemovedService providerFundingDataRemovedService)
        {
            _summarisationServices = summarisationServices;
            _logger = logger;
            _providerContractsService = providerContractsService;
            _providerFundingDataRemovedService = providerFundingDataRemovedService;
    }

        public async Task<IEnumerable<SummarisedActual>> Summarise(ILearningProvider providerData, IEnumerable<CollectionPeriod> collectionPeriods, IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fcsContractAllocations, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var providerActuals = new List<SummarisedActual>();

            var summarisationService = _summarisationServices.FirstOrDefault(x => x.ProcessType.Equals(summarisationMessage.ProcessType, StringComparison.OrdinalIgnoreCase));

            foreach (var summarisationType in summarisationMessage.SummarisationTypes)
            {
                if (!summarisationType.Equals(ConstantKeys.ReRunSummarisation, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {providerData.UKPRN}, Fundmodel {summarisationType} Start");

                    var providerfundingstreamsContracts = await _providerContractsService.GetProviderContracts(providerData.UKPRN, summarisationMessage.CollectionType, summarisationType, cancellationToken);
                    
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
                _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed  Rule UKPRN: {providerData.UKPRN} Start");

                var actualsToCarry = await _providerFundingDataRemovedService.FundingDataRemovedAsync(providerActuals, summarisationMessage, cancellationToken);

                providerActuals.AddRange(actualsToCarry);

                _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed  Rule UKPRN: {providerData.UKPRN} End");
            }

            return providerActuals;
        }
    }
}
