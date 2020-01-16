using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Data.Persist.Persist.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Model;
using SummarisedActual = ESFA.DC.Summarisation.Data.output.Model.SummarisedActual;

namespace ESFA.DC.Summarisation.Data.Persist
{
    public class DataStorePersistenceService : IDataStorePersistenceService
    {
        private const string InsertCollectionReturnSql = @"INSERT INTO CollectionReturn (CollectionType, CollectionReturnCode) VALUES (@CollectionType, @CollectionReturnCode); SELECT CAST(SCOPE_IDENTITY() as int)";
        private const string GetCollectionReturnSql = @"SELECT TOP 1  Id FROM CollectionReturn WHERE CollectionType = @CollectionType AND CollectionReturnCode = @CollectionReturnCode";
        private const string DeleteCollectionReturnSql = @"DELETE FROM CollectionReturn WHERE Id = @Id";
        private const string DeleteSummarisedActualsSql = @"DELETE FROM SummarisedActuals WHERE CollectionReturnId = @Id";

        private readonly ICollectionReturnMapper _collectionReturnMapper;
        private readonly ISummarisedActualsPersist _summarisedActualsPersist;
        private readonly Func<SqlConnection> _sqlConnectionFactory;

        public DataStorePersistenceService(
            ISummarisedActualsPersist summarisedActualsPersist,
            ICollectionReturnMapper collectionReturnMapper,
            Func<SqlConnection> sqlConnectionFactory)
        {
            _summarisedActualsPersist = summarisedActualsPersist;
            _sqlConnectionFactory = sqlConnectionFactory;
            _collectionReturnMapper = collectionReturnMapper;
        }

        public async Task StoreSummarisedActualsDataAsync(
            IList<SummarisedActual> summarisedActuals,
            ISummarisationMessage summarisationMessage,
            CancellationToken cancellationToken)
        {
            using (var sqlConnection = this._sqlConnectionFactory.Invoke())
            {
                await sqlConnection.OpenAsync(cancellationToken);

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    var collectionReturn = this._collectionReturnMapper.MapCollectionReturn(summarisationMessage);

                    if (summarisationMessage.RerunSummarisation)
                    {
                        collectionReturn.Id = sqlConnection.ExecuteScalar<int>(GetCollectionReturnSql, collectionReturn, transaction);

                        sqlConnection.Execute(DeleteSummarisedActualsSql, collectionReturn, transaction);

                        sqlConnection.Execute(DeleteCollectionReturnSql, collectionReturn, transaction);
                    }

                    var collectionReturnId = await this.InsertCollectionReturnAsync(collectionReturn, sqlConnection, transaction);

                    await this._summarisedActualsPersist.Save(summarisedActuals, collectionReturnId, sqlConnection, transaction, cancellationToken);

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

        private async Task<int> InsertCollectionReturnAsync(CollectionReturn collectionReturn, SqlConnection sqlConnection, SqlTransaction sqlTransaction)
        {
            var result = await sqlConnection.QueryAsync<int>(InsertCollectionReturnSql, collectionReturn, sqlTransaction);
            return result.Single();
        }
    }
}