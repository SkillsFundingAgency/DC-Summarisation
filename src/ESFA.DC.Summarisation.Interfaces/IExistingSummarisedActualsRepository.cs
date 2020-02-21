using ESFA.DC.Summarisation.Service.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IExistingSummarisedActualsRepository
    {
        Task<CollectionReturn> GetLastCollectionReturnForReRunAsync(string collectionType, string collectionReturnCode, CancellationToken cancellationToken);

        Task<CollectionReturn> GetLastCollectionReturnAsync(string collectionType, string collectionReturnCode, CancellationToken cancellationToken);

        Task<IEnumerable<SummarisedActual>> GetSummarisedActualsAsync(int collectionReturnId, CancellationToken cancellationToken);

        Task<IEnumerable<SummarisedActual>> GetSummarisedActualsAsync(int collectionReturnId, string organisationId, CancellationToken cancellationToken);

        Task<IEnumerable<SummarisedActual>> GetSummarisedActualsAsync(int collectionReturnId, string organisationId, string uopCode, CancellationToken cancellationToken);
    }
}
