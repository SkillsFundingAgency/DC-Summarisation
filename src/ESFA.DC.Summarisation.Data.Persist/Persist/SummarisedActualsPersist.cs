using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Persist.Constants;
using ESFA.DC.Summarisation.Data.Persist.Persist.Interface;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist.Persist
{
    public class SummarisedActualsPersist : ISummarisedActualsPersist
    {
        private readonly IBulkInsert _bulkInsert;

        public SummarisedActualsPersist(IBulkInsert bulkInsert)
        {
            _bulkInsert = bulkInsert;
        }

        public async Task Save(IList<SummarisedActual> summarisedActuals, SqlConnection sqlConnection, SqlTransaction sqlTransaction, CancellationToken cancellationToken)
            => await _bulkInsert.Insert(SummarisedActualsConstants.SummarisedActuals, summarisedActuals, sqlConnection, sqlTransaction, cancellationToken);
    }
}