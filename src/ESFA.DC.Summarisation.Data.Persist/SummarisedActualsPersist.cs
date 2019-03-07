using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist
{
    public class SummarisedActualsPersist : ISummarisedActualsPersist
    {
        private readonly IBulkInsert _bulkInsert;
        private readonly SqlConnection _sqlConnection;
        private readonly ISummarisedActualsMapper _summarisedActualsMapper;

        public SummarisedActualsPersist(IBulkInsert bulkInsert, SqlConnection sqlConnection, ISummarisedActualsMapper summarisedActualsMapper, CollectionReturn collectionReturn)
        {
            _bulkInsert = bulkInsert;
            _sqlConnection = sqlConnection;
            _summarisedActualsMapper = summarisedActualsMapper;
        }

        public void Save(List<Output.Model.SummarisedActual> summarisedActuals)
        {
            var summarisedActualsMapped =  _summarisedActualsMapper.MapSummarisedActuals(summarisedActuals);

            _bulkInsert.Insert("SummarisedActuals", summarisedActualsMapped, _sqlConnection, CancellationToken.None);
        }
    }
}
