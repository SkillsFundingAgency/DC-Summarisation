using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Data.Persist.Persist.Interface;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist
{
    public class DataStorePersistenceService : IDataStorePersistenceService
    {
        private readonly ICollectionReturnMapper _collectionReturnMapper;
        private readonly ISummarisedActualsMapper _summarisedActualsMapper;

        private readonly ISummarisedActualsPersist _summarisedActualsPersist;
        private readonly Func<SqlConnection> _sqlConnectionFactory;

        public DataStorePersistenceService(ISummarisedActualsPersist summarisedActualsPersist, ICollectionReturnMapper collectionReturnMapper, ISummarisedActualsMapper summarisedActualsMapper, Func<SqlConnection> sqlConnectionFactory)
        {
            _summarisedActualsPersist = summarisedActualsPersist;
            _sqlConnectionFactory = sqlConnectionFactory;
            _collectionReturnMapper = collectionReturnMapper;
            _summarisedActualsMapper = summarisedActualsMapper;
        }

        public async Task StoreSummarisedActualsDataAsync(IList<Output.Model.SummarisedActual> summarisedActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            using (var sqlConnection = _sqlConnectionFactory.Invoke())
            {
                await sqlConnection.OpenAsync(cancellationToken);

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    var collectionReturn = _collectionReturnMapper.MapCollectionReturn(summarisationMessage);
                    var collectionReturnId = await sqlConnection.InsertAsync(collectionReturn, transaction);

                    var mappedActuals = _summarisedActualsMapper.MapSummarisedActuals(summarisedActuals, collectionReturnId).ToList();

                    await _summarisedActualsPersist.Save(mappedActuals, sqlConnection, cancellationToken);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        transaction.Rollback();
                    }
                    else
                    {
                        transaction.Commit();
                    }
                }
            }
        }
    }
}