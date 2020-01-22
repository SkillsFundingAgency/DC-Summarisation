using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Repository.Interface
{
    public interface IExistingSummarisedActualsRepository
    {
        Task<CollectionReturn> GetLastCollectionReturnForCollectionTypeAsync(string collectionType, CancellationToken cancellationToken);

        Task<IEnumerable<Service.Model.SummarisedActual>> GetSummarisedActualsForCollectionReturnAndOrganisationAsync(int collectionReturnId, string organisationId, CancellationToken cancellationToken);
    }
}
