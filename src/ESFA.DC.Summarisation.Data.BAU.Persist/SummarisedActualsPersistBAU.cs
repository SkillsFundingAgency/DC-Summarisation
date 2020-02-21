using System;
using System.Collections.Generic;
using ESFA.DC.Summarisation.Data.BAU.Persist.Interfaces;
using ESFA.DC.Summarisation.Data.BAU.Persist.Model;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.BulkCopy.Interfaces;

namespace ESFA.DC.Summarisation.Data.BAU.Persist
{
    public class SummarisedActualsPersistBAU : ISummarisedActualsPersistBAU
    {
        private readonly IBulkInsert _bulkInsert;

        public SummarisedActualsPersistBAU(IBulkInsert bulkInsert)
        {
            _bulkInsert = bulkInsert;
        }

        public async Task Save(IEnumerable<SummarisedActualBAU> summarisedActuals, SqlConnection sqlConnection, SqlTransaction sqlTransaction, CancellationToken cancellationToken)
        {
            await _bulkInsert.Insert(SummarisedActualsConstants.SummarisedActuals, summarisedActuals, sqlConnection, sqlTransaction, cancellationToken);
        }
    }
}