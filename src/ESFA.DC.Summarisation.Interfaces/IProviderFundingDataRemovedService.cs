using ESFA.DC.Summarisation.Service.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IProviderFundingDataRemovedService
    {
        Task<ICollection<SummarisedActual>> FundingDataRemovedAsync(ICollection<SummarisedActual> providerActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
