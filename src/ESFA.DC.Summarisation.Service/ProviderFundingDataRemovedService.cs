using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Service.EqualityComparer;
using ESFA.DC.Summarisation.Service.Model;

namespace ESFA.DC.Summarisation.Service
{
    public class ProviderFundingDataRemovedService : IProviderFundingDataRemovedService
    {
        private readonly IExistingSummarisedActualsRepository _summarisedActualsProcessRepository;

        public ProviderFundingDataRemovedService(IExistingSummarisedActualsRepository summarisedActualsProcessRepository)
        {
            _summarisedActualsProcessRepository = summarisedActualsProcessRepository;
        }

        public async Task<ICollection<SummarisedActual>> FundingDataRemovedAsync(ICollection<SummarisedActual> providerActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var latestCollectionReturn = await _summarisedActualsProcessRepository.GetLastCollectionReturnForCollectionTypeAsync(summarisationMessage.CollectionType, cancellationToken);

            var organisationId = providerActuals.Select(x => x.OrganisationId).FirstOrDefault();

            var actualsToCarry = new List<SummarisedActual>();

            if (latestCollectionReturn != null)
            {
                var previousActuals = await _summarisedActualsProcessRepository.GetSummarisedActualsForCollectionReturnAndOrganisationAsync(latestCollectionReturn.Id, organisationId, cancellationToken);

                var comparer = new CarryOverActualsComparer();

                actualsToCarry = previousActuals.Except(providerActuals, comparer).ToList();

                foreach (var actuals in actualsToCarry)
                {
                    actuals.ActualVolume = 0;
                    actuals.ActualValue = 0;
                }

            }

            return actualsToCarry;
        }

    }
}
