using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Persist.Persist.Interface;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist
{
    public class DataStorePersistenceService : IDataStorePersistenceService
    {
        private readonly ISummarisedActualsPersist _summarisedActualsPersist;
        private readonly ICollectionReturnPersist _collectionReturnPersist;

        public DataStorePersistenceService(ISummarisedActualsPersist summarisedActualsPersist, ICollectionReturnPersist collectionReturnPersist)
        {
            _summarisedActualsPersist = summarisedActualsPersist;
            _collectionReturnPersist = collectionReturnPersist;
        }

        public async Task<CollectionReturn> StoreCollectionReturnAsync(Output.Model.CollectionReturn collectionReturn, CancellationToken cancellationToken)
            => await _collectionReturnPersist.Save(collectionReturn, cancellationToken);

        public async Task StoreSummarisedActualsDataAsync(IList<Output.Model.SummarisedActual> summarisedActuals, CollectionReturn collectionReturn, SqlConnection sqlConnection, CancellationToken cancellationToken)
            => await _summarisedActualsPersist.Save(summarisedActuals, collectionReturn, cancellationToken);
    }
}