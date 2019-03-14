using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist
{
    public interface IDataStorePersistenceService
    {
        Task<CollectionReturn> StoreCollectionReturnAsync(Output.Model.CollectionReturn collectionReturn, CancellationToken cancellationToken);

        Task StoreSummarisedActualsDataAsync(IList<Output.Model.SummarisedActual> summarisedActuals, SqlConnection sqlConnection, CancellationToken cancellationToken);
    }
}