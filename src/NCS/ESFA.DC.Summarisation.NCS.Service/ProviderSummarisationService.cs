using System;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using ESFA.DC.Summarisation.NCS.Interfaces;
using ESFA.DC.Summarisation.NCS.Model;
using ESFA.DC.Summarisation.NCS.Model.Config;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Constants;

namespace ESFA.DC.Summarisation.NCS.Service
{
    public class ProviderSummarisationService : IProviderSummarisationService<TouchpointProviderFundingData>
    {
        private readonly ISummarisationService _summarisationService;
        private readonly ILogger _logger;
        private readonly IProviderContractsService _providerContractsService;
        private readonly IProviderFundingDataRemovedService _providerFundingDataRemovedService;        

        public ProviderSummarisationService(
            ISummarisationService summarisationService,
            ILogger logger,
            IProviderContractsService providerContractsService,
            IProviderFundingDataRemovedService providerFundingDataRemovedService)
        {
            _summarisationService = summarisationService;
            _logger = logger;
            _providerContractsService = providerContractsService;
            _providerFundingDataRemovedService = providerFundingDataRemovedService;
        }

        public async Task<ICollection<SummarisedActual>> Summarise(TouchpointProviderFundingData providerData, ICollection<CollectionPeriod> collectionPeriods, ICollection<FundingType> fundingTypes,  ICollection<FcsContractAllocation> contractAllocations, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var providerActuals = new List<SummarisedActual>();

            var provider = providerData.Provider;
                                    
            foreach (var summarisationType in summarisationMessage.SummarisationTypes)
            {
                if (!summarisationType.Equals(ConstantKeys.ReRunSummarisation, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {providerData.Provider.UKPRN}, Fundmodel {summarisationType} Start");

                    var fundingStreams = fundingTypes?
                        .Where(x => x.SummarisationType.Equals(summarisationType, StringComparison.OrdinalIgnoreCase))
                        .SelectMany(fs => fs.FundingStreams)
                        .ToList();

                    var providerfundingstreamsContracts = await _providerContractsService.GetProviderContracts(provider.UKPRN, fundingStreams, contractAllocations, cancellationToken);
                    
                    var summarisedData = _summarisationService.Summarise(providerfundingstreamsContracts.FundingStreams, providerData, providerfundingstreamsContracts.FcsContractAllocations, collectionPeriods);

                    providerActuals.AddRange(summarisedData);

                    _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {provider.UKPRN}, Fundmodel {summarisationType} End");
                }
            }

            if (!summarisationMessage.ProcessType.Equals(ProcessTypeConstants.Payments, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed  Rule UKPRN: {provider.UKPRN} Start");

                var organisationId = providerActuals.Select(x => x.OrganisationId).FirstOrDefault();

                var actualsToCarry = await _providerFundingDataRemovedService.FundingDataRemovedAsync(organisationId, providerData.Provider.TouchpointId, providerActuals, summarisationMessage, cancellationToken);

                providerActuals.AddRange(actualsToCarry);

                _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed  Rule UKPRN: {provider.UKPRN} End");
            }

            return providerActuals;
        }
    }
}
