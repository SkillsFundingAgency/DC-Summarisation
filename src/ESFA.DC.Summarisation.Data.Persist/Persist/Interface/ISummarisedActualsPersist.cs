using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Output.Model;

namespace ESFA.DC.Summarisation.Data.Persist.Persist.Interface
{
    public interface ISummarisedActualsPersist
    {
        Task Save(IList<SummarisedActual> summarisedActuals, Model.CollectionReturn collectionReturn, CancellationToken cancellationToken);
    }
}
