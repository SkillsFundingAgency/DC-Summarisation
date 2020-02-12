using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IGenericCollectionRepository
    {
        Task<ICollection<Service.Model.SummarisedActual>> RetrieveAsync(string collectionType, CancellationToken cancellationToken);
    }
}
