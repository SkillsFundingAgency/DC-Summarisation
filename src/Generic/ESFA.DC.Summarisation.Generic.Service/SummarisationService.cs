using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Generic.Interfaces;
using ESFA.DC.Summarisation.Service.Extensions;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;

namespace ESFA.DC.Summarisation.Generic.Service
{
    public class SummarisationService : ISummarisationService
    {
        private readonly ILogger _logger;

        public SummarisationService(ILogger logger)
        {
            _logger = logger;
        }

        public ICollection<SummarisedActual> Summarise(ICollection<FcsContractAllocation> fcsContractAllocations, ICollection<SummarisedActual> summarisedActuals)
        {
            _logger.LogInfo($"Summarisation Wrapper: Summarising Generic Collection Data Start");

            var deliveryUkprns = fcsContractAllocations.Select(u => u.DeliveryUkprn.Value).ToList();
            var validSummarisedActuals = summarisedActuals.Where(u => deliveryUkprns.Contains(int.Parse(u.OrganisationId))).ToList();

            var contractAllocationaDictionary = fcsContractAllocations?
                .Select(f => new
                {
                    ukprn = f.DeliveryUkprn,
                    organisation = f.DeliveryOrganisation
                })
                .Distinct()
                .ToDictionary(u => u.ukprn, o => o.organisation);

            _logger.LogInfo($"Summarisation Wrapper: Mapping Generic Collection Ukprns to OrgIds");

            foreach (var summarisedActual in validSummarisedActuals)
            {
                summarisedActual.OrganisationId = contractAllocationaDictionary.GetValueOrDefault(int.Parse(summarisedActual.OrganisationId));
            }

            _logger.LogInfo($"Summarisation Wrapper: Summarising Generic Collection Data End");

            return validSummarisedActuals;
        }
    }
}
