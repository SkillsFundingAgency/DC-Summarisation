using ESFA.DC.Summarisation.Data.BAU.Persist.Model;
using ESFA.DC.Summarisation.Data.BAU.Persist.Interfaces;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Data.BAU.Persist
{
    public class DataStorePersistenceServiceBAU : IDataStorePersistenceServiceBAU
    {
        private const string InsertEventlogSql = @"INSERT INTO EventLog (CollectionType, CollectionReturnCode, DateTime) VALUES (@CollectionType, @CollectionReturnCode, @DateTime);";
        private const string DeleteEventLogSql = @"DELETE FROM EventLog WHERE CollectionType = @CollectionType And CollectionReturnCode = @CollectionReturnCode";
        private const string DeleteSummarisedActualsSql = @"DELETE FROM SummarisedActuals WHERE CollectionType = @CollectionType And CollectionReturnCode = @CollectionReturnCode";

        private readonly ISummarisedActualsPersistBAU _summarisedActualsPersist;
        private readonly ISummarisationDataOptions _summarisationDataOptions;

        public DataStorePersistenceServiceBAU(
            ISummarisedActualsPersistBAU summarisedActualsPersist,
            ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisedActualsPersist = summarisedActualsPersist;
            _summarisationDataOptions = summarisationDataOptions;
        }

        public async Task StoreSummarisedActualsDataAsync(IEnumerable<SummarisedActualBAU> summarisedActuals, EventLog eventLog, CancellationToken cancellationToken)
        {
            using (var sqlConnection = new SqlConnection(_summarisationDataOptions.SummarisedActualsBAUConnectionString))
            {
                await sqlConnection.OpenAsync(cancellationToken);                

                using (var transaction = sqlConnection.BeginTransaction())
                {
                    try
                    {
                        // Remove Existing Summarised Actuals for the given CollectionReturnCode
                        await sqlConnection.ExecuteAsync(DeleteSummarisedActualsSql, eventLog, transaction);

                        await sqlConnection.ExecuteAsync(DeleteEventLogSql, eventLog, transaction);

                        // Persist new set of Summarised Actuals
                        await sqlConnection.ExecuteAsync(InsertEventlogSql, eventLog, transaction);

                        await this._summarisedActualsPersist.SaveAsync(summarisedActuals, sqlConnection, transaction, cancellationToken);

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
    }
}
