using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Persist.Constants;
using ESFA.DC.Summarisation.Data.Persist.Persist.Interface;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Model;
using SummarisedActual = ESFA.DC.Summarisation.Model.SummarisedActual;

namespace ESFA.DC.Summarisation.Data.Persist.Persist
{
    public class SummarisedActualsPersist : ISummarisedActualsPersist
    {
        private readonly IBulkInsert _bulkInsert;

        public SummarisedActualsPersist(IBulkInsert bulkInsert)
        {
            _bulkInsert = bulkInsert;
        }

        //public async Task Save(IList<SummarisedActual> summarisedActuals, SqlConnection sqlConnection, SqlTransaction sqlTransaction, CancellationToken cancellationToken)
        //    => await _bulkInsert.Insert(SummarisedActualsConstants.SummarisedActuals, summarisedActuals, sqlConnection, sqlTransaction, cancellationToken);

        public async Task Save(IList<Output.Model.SummarisedActual> summarisedActuals, int collectionReturnId, SqlConnection sqlConnection, SqlTransaction sqlTransaction, CancellationToken cancellationToken)
        {
            foreach (var item in summarisedActuals)
            {
                item.CollectionReturnId = collectionReturnId;
            }

            await _bulkInsert.Insert(SummarisedActualsConstants.SummarisedActuals, summarisedActuals, sqlConnection, sqlTransaction, cancellationToken);
        }
    }
}