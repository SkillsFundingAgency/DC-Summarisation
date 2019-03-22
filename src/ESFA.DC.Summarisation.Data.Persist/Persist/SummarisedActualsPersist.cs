using System;
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
        private readonly ISummarisedActualsMapper _summarisedActualsMapper;
        private readonly Func<SqlConnection> _sqlConnectionFactory;

        public SummarisedActualsPersist(IBulkInsert bulkInsert, Func<SqlConnection> sqlConnectionFactory, ISummarisedActualsMapper summarisedActualsMapper)
        {
            _bulkInsert = bulkInsert;
            _sqlConnectionFactory = sqlConnectionFactory;
            _summarisedActualsMapper = summarisedActualsMapper;
        }

        public async Task Save(IList<Output.Model.SummarisedActual> summarisedActuals, CollectionReturn collectionReturn, CancellationToken cancellationToken)
        {
            var summarisedActualsMapped = _summarisedActualsMapper.MapSummarisedActuals(summarisedActuals, collectionReturn);

            using (var sqlConnection = _sqlConnectionFactory.Invoke())
            {
                await sqlConnection.OpenAsync(cancellationToken);

                await _bulkInsert.Insert(SummarisedActualsConstants.SummarisedActuals, summarisedActualsMapped, sqlConnection, cancellationToken);
            }
        }
    }
}