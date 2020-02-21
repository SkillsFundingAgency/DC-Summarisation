using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;

namespace ESFA.DC.Summarisation.Data.Persist.Persist
{
    public class SummarisedActualsPersist : ISummarisedActualsPersist
    {
        private readonly IBulkInsert _bulkInsert;

        public SummarisedActualsPersist(IBulkInsert bulkInsert)
        {
            _bulkInsert = bulkInsert;
        }

        public async Task Save(IList<SummarisedActual> summarisedActuals, int collectionReturnId, SqlConnection sqlConnection, SqlTransaction sqlTransaction, CancellationToken cancellationToken)
        {
            foreach (var item in summarisedActuals)
            {
                item.CollectionReturnId = collectionReturnId;
            }

            await _bulkInsert.Insert(SummarisedActualsConstants.SummarisedActuals, summarisedActuals, sqlConnection, sqlTransaction, cancellationToken);
        }
    }
}