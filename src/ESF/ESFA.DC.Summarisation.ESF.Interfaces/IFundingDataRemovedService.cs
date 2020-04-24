using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.ESF.Interfaces
{
    public interface IFundingDataRemovedService
    {
        Task<ICollection<SummarisedActual>> FundingDataRemovedAsync(ICollection<SummarisedActual> providerActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
