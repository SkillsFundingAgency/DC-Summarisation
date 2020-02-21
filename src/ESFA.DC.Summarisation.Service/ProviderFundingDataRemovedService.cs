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

        public async Task<ICollection<SummarisedActual>> FundingDataRemovedAsync(string organisationId, ICollection<SummarisedActual> providerActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var latestCollectionReturn = await _summarisedActualsProcessRepository.GetLastCollectionReturnForReRunAsync(summarisationMessage.CollectionType, summarisationMessage.CollectionReturnCode, cancellationToken);

            var actualsToCarry = new List<SummarisedActual>();

            if (latestCollectionReturn != null)
            {
                var previousActuals = await _summarisedActualsProcessRepository.GetSummarisedActualsAsync(latestCollectionReturn.Id, organisationId, cancellationToken);

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

        public async Task<ICollection<SummarisedActual>> FundingDataRemovedAsync(string organisationId, string uopCode, ICollection<SummarisedActual> providerActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var latestCollectionReturn = await _summarisedActualsProcessRepository.GetLastCollectionReturnForReRunAsync(summarisationMessage.CollectionType, summarisationMessage.CollectionReturnCode, cancellationToken);

            var actualsToCarry = new List<SummarisedActual>();

            if (latestCollectionReturn != null)
            {
                var previousActuals = await _summarisedActualsProcessRepository.GetSummarisedActualsAsync(latestCollectionReturn.Id, organisationId, uopCode, cancellationToken);

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
