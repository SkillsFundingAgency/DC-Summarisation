using ESFA.DC.Summarisation.Data.Mapper;
using ESFA.DC.Summarisation.Data.Mapper.Interface;
using ESFA.DC.Summarisation.Data.output.Model;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;

namespace ESFA.DC.Summarisation.Data.Repository
{
    public class SummarisedActualsPersist : ISummarisedActualsPersist
    {
        private readonly IBulkInsert _bulkInsert;
        private readonly SqlConnection _sqlConnection;
        public SummarisedActualsPersist(IBulkInsert bulkInsert, SqlConnection sqlConnection)
        {
            _bulkInsert = bulkInsert;
            _sqlConnection = sqlConnection;
        }

        public void Save(List<SummarisedActual> summarisedActuals)
        {
            ISummarisedActualsMapper summarisedActualsMapper = new SummarisedActualsMapper();
            var summarisedActualsMapped =  summarisedActualsMapper.MapSummarisedActuals(summarisedActuals);

            _bulkInsert.Insert("SummarisedActuals", summarisedActualsMapped, _sqlConnection, CancellationToken.None);
        }
    }
}
