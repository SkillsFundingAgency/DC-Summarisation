using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist.Persist.Interface
{
    public interface ICollectionReturnPersist
    {
        Task<CollectionReturn> Save(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}