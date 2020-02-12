using ESFA.DC.Summarisation.Service.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationWrapper
    {
        Task<ICollection<SummarisedActual>> Summarise(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
