using System;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Generic.Interfaces;
using ESFA.DC.Summarisation.Service.Model;

namespace ESFA.DC.Summarisation.Generic.Service
{
    public class ProviderSummarisationService : IProviderSummarisationService<IEnumerable<SummarisedActual>>
    {
        private readonly ILogger _logger;
        private readonly IProviderFundingDataRemovedService _providerFundingDataRemovedService;

        public ProviderSummarisationService(
            ILogger logger,
            IProviderFundingDataRemovedService providerFundingDataRemovedService)
        {
            _logger = logger;
            _providerFundingDataRemovedService = providerFundingDataRemovedService;
        }

        public async Task<ICollection<SummarisedActual>> Summarise(IEnumerable<SummarisedActual> inputSummarisedActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var providerActuals = inputSummarisedActuals.ToList();

            var groupedActuals = inputSummarisedActuals?
                .GroupBy(x => x.OrganisationId, StringComparer.OrdinalIgnoreCase)
                .Select(sa => new
                {
                    OrganisationId = sa.Key,
                    SummarisedActuals = sa.ToList()
                }).ToList();

            foreach (var group in groupedActuals)
            {
                _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed Rule OrgId: {group.OrganisationId} Start");

                var actualsToCarry = await _providerFundingDataRemovedService.FundingDataRemovedAsync(group.OrganisationId, group.SummarisedActuals, summarisationMessage, cancellationToken);

                providerActuals.AddRange(actualsToCarry);

                _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed  Rule OrgId: {group.OrganisationId} End");
            }

            return providerActuals;
        }
    }
}
