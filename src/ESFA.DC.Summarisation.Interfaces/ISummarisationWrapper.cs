using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Output.Model;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationWrapper
    {
        Task<IEnumerable<SummarisedActual>> Summarise(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
