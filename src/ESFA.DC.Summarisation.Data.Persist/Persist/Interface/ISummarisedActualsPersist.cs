using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Data.Persist.Persist.Interface
{
    public interface ISummarisedActualsPersist
    {
        Task Save(IList<Output.Model.SummarisedActual> summarisedActuals, CancellationToken cancellationToken);
    }
}
