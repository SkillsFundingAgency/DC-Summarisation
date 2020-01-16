using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Model;
using SummarisedActual = ESFA.DC.Summarisation.Data.output.Model.SummarisedActual;

namespace ESFA.DC.Summarisation.Data.Repository.Interface
{
    public interface IExistingSummarisedActualsRepository
    {
        Task<CollectionReturn> GetLastCollectionReturnForCollectionTypeAsync(string collectionType, CancellationToken cancellationToken);

        Task<IEnumerable<SummarisedActual>> GetSummarisedActualsForCollectionReturnAndOrganisationAsync(int collectionReturnId, string organisationId, CancellationToken cancellationToken);
    }
}
