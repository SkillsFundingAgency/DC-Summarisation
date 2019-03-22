using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Persist;
using ESFA.DC.Summarisation.Model;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Modules.Stubs
{
    public class DataStorePersistenceServiceStub : IDataStorePersistenceService
    {
        public Task<CollectionReturn> StoreCollectionReturnAsync(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CollectionReturn());
        }

        public Task StoreSummarisedActualsDataAsync(IList<Data.Output.Model.SummarisedActual> summarisedActuals, CollectionReturn collectionReturn, SqlConnection sqlConnection, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
