﻿using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Model;

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
        private readonly ISummarisationDataOptions _summarisationDataOptions;

        public DataStorePersistenceService(
            ISummarisedActualsPersist summarisedActualsPersist,
            ICollectionReturnMapper collectionReturnMapper,
            ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisedActualsPersist = summarisedActualsPersist;
            _collectionReturnMapper = collectionReturnMapper;
            _summarisationDataOptions = summarisationDataOptions;
        }

        public async Task StoreSummarisedActualsDataAsync(
            IList<Service.Model.SummarisedActual> summarisedActuals,
            ISummarisationMessage summarisationMessage,
            CancellationToken cancellationToken)
        {
            using (var sqlConnection = new SqlConnection(_summarisationDataOptions.SummarisedActualsConnectionString))
            {
                await sqlConnection.OpenAsync(cancellationToken);

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        var collectionReturn = this._collectionReturnMapper.MapCollectionReturn(summarisationMessage);

                        // Remove Existing Summarised Actuals for the given CollectionReturnCode
                        collectionReturn.Id = sqlConnection.ExecuteScalar<int>(GetCollectionReturnSql, collectionReturn, transaction);

                        sqlConnection.Execute(DeleteSummarisedActualsSql, collectionReturn, transaction);

                        sqlConnection.Execute(DeleteCollectionReturnSql, collectionReturn, transaction);

                        // Persist new set of Summarised Actuals
                        var collectionReturnId = await this.InsertCollectionReturnAsync(collectionReturn, sqlConnection, transaction);

                        await this._summarisedActualsPersist.Save(summarisedActuals, collectionReturnId, sqlConnection, transaction, cancellationToken);

                        cancellationToken.ThrowIfCancellationRequested();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
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