using System;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.ESF.Interfaces;
using ESFA.DC.Summarisation.ESF.Model;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using ESFA.DC.Summarisation.ESF.Model.Config;

namespace ESFA.DC.Summarisation.ESF.Service
{
    public class ProviderSummarisationService : IProviderSummarisationService<LearningProvider>
    {
        private readonly ISummarisationService _summarisationService;
        private readonly ILogger _logger;
        private readonly IProviderContractsService _providerContractsService;

        public ProviderSummarisationService(
            ISummarisationService summarisationService,
            ILogger logger,
            IProviderContractsService providerContractsService)
        {
            _summarisationService = summarisationService;
            _logger = logger;
            _providerContractsService = providerContractsService;
        }

        public async Task<ICollection<SummarisedActual>> Summarise(LearningProvider providerData, ICollection<CollectionPeriod> collectionPeriods, ICollection<FundingType> fundingTypes,  ICollection<FcsContractAllocation> contractAllocations, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var providerActuals = new List<SummarisedActual>();            
            
            foreach (var summarisationType in summarisationMessage.SummarisationTypes)
            {
                _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {providerData.UKPRN}, Fundmodel {summarisationType} Start");

                var fundingStreams = fundingTypes?
                    .Where(x => x.SummarisationType.Equals(summarisationType, StringComparison.OrdinalIgnoreCase))
                    .SelectMany(fs => fs.FundingStreams)
                    .ToList();

                var providerfundingstreamsContracts = await _providerContractsService.GetProviderContracts(providerData.UKPRN, fundingStreams, contractAllocations, cancellationToken);
               
                var summarisedData = _summarisationService.Summarise(providerfundingstreamsContracts.FundingStreams, providerData, providerfundingstreamsContracts.FcsContractAllocations, collectionPeriods, summarisationMessage);

                providerActuals.AddRange(summarisedData);

                _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {providerData.UKPRN}, Fundmodel {summarisationType} End");
            }

            return providerActuals;
        }
    }
}
