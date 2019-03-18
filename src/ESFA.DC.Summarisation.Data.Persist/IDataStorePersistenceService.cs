using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist
{
    public interface IDataStorePersistenceService
    {
        Task<CollectionReturn> StoreCollectionReturnAsync(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);

        Task StoreSummarisedActualsDataAsync(IList<Output.Model.SummarisedActual> summarisedActuals, CollectionReturn collectionReturn, SqlConnection sqlConnection, CancellationToken cancellationToken);
    }
}