using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Persist.Constants;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Data.Persist.Persist.Interface;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist.Persist
{
    public class SummarisedActualsPersist : ISummarisedActualsPersist
    {
        private readonly IBulkInsert _bulkInsert;
        private readonly SqlConnection _sqlConnection;
        private readonly ISummarisedActualsMapper _summarisedActualsMapper;
        private readonly CollectionReturn _collectionReturn;

        public SummarisedActualsPersist(IBulkInsert bulkInsert, SqlConnection sqlConnection, ISummarisedActualsMapper summarisedActualsMapper, CollectionReturn collectionReturn)
        {
            _bulkInsert = bulkInsert;
            _sqlConnection = sqlConnection;
            _summarisedActualsMapper = summarisedActualsMapper;
            _collectionReturn = collectionReturn;
        }

        public async Task Save(IList<Output.Model.SummarisedActual> summarisedActuals, CancellationToken cancellationToken)
        {
            var summarisedActualsMapped = _summarisedActualsMapper.MapSummarisedActuals(summarisedActuals, _collectionReturn);

            await _bulkInsert.Insert(SummarisedActualsConstants.SummarisedActuals, summarisedActualsMapped, _sqlConnection, cancellationToken);
        }
    }
}