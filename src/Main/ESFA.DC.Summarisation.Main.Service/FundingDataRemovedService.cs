using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main.Interfaces;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Utils.EqualityComparer;

namespace ESFA.DC.Summarisation.Main.Service
{
    public class FundingDataRemovedService : IFundingDataRemovedService
    {
        private readonly IExistingSummarisedActualsRepository _summarisedActualsProcessRepository;

        public FundingDataRemovedService(IExistingSummarisedActualsRepository summarisedActualsProcessRepository)
        {
            _summarisedActualsProcessRepository = summarisedActualsProcessRepository;
        }

        public async Task<ICollection<SummarisedActual>> FundingDataRemovedAsync(ICollection<SummarisedActual> providerActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var latestCollectionReturn = await _summarisedActualsProcessRepository.GetLastCollectionReturnForReRunAsync(summarisationMessage.CollectionType, summarisationMessage.CollectionReturnCode, cancellationToken);

            var actualsToCarry = new List<SummarisedActual>();

            if (latestCollectionReturn != null)
            {
                var previousActuals = await _summarisedActualsProcessRepository.GetSummarisedActualsAsync(latestCollectionReturn.Id, cancellationToken);

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
