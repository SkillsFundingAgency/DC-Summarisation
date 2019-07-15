using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Configuration.Enum;
using ESFA.DC.Summarisation.Data.Output.Model;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Data.Persist.Persist.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Model;
using ESFA.DC.Summarisation.Model.Interface;

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
            this._summarisedActualsPersist = summarisedActualsPersist;
            this._sqlConnectionFactory = sqlConnectionFactory;
            this._collectionReturnMapper = collectionReturnMapper;
        }

        public async Task StoreSummarisedActualsDataAsync(
            IList<Output.Model.SummarisedActual> summarisedActuals,
            ICollectionReturn lastCollectionReturn,
            ISummarisationMessage summarisationMessage,
            IEnumerable<CollectionPeriod> collectionPeriods,
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

                    if (this.CheckESFPeriodCollectionMissing(collectionReturn, lastCollectionReturn, summarisationMessage))
                    {
                        await this.StoreESFSummarisedActualsDataForMissingPeriodsAsync(
                           this.GetZeroValueSummarisedActuals(summarisedActuals.ToList()),
                           summarisationMessage,
                           lastCollectionReturn.CollectionReturnCode,
                           collectionPeriods,
                           sqlConnection,
                           transaction,
                           cancellationToken);
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

        public IList<Output.Model.SummarisedActual> GetZeroValueSummarisedActuals(List<Output.Model.SummarisedActual> summarisedActuals)
        {
            IList<Output.Model.SummarisedActual> zeroValueSummarisedActuals = new List<Output.Model.SummarisedActual>();

            foreach (var summarised in summarisedActuals)
            {
                zeroValueSummarisedActuals.Add(
                    new Output.Model.SummarisedActual()
                    {
                        ActualValue = 0,
                        ActualVolume = 0,
                        CollectionReturnId = 0,
                        ContractAllocationNumber = summarised.ContractAllocationNumber,
                        DeliverableCode = summarised.DeliverableCode,
                        FundingStreamPeriodCode = summarised.FundingStreamPeriodCode,
                        OrganisationId = summarised.OrganisationId,
                        Period = summarised.Period,
                        PeriodTypeCode = summarised.PeriodTypeCode,
                        UoPCode = summarised.UoPCode
                    });
            }

            return zeroValueSummarisedActuals;
        }

        private async Task<int> InsertCollectionReturnAsync(CollectionReturn collectionReturn, SqlConnection sqlConnection, SqlTransaction sqlTransaction)
        {
            var result = await sqlConnection.QueryAsync<int>(InsertCollectionReturnSql, collectionReturn, sqlTransaction);
            return result.Single();
        }

        public async Task StoreESFSummarisedActualsDataForMissingPeriodsAsync(
            IList<Output.Model.SummarisedActual> summarisedActuals,
            ISummarisationMessage summarisationMessage,
            string lastCollectionReturnCode,
            IEnumerable<CollectionPeriod> collectionPeriods,
            SqlConnection sqlConnection,
            SqlTransaction sqlTransaction,
            CancellationToken cancellationToken)
        {
            List<string> newCollectionPeriodsToAdd =
                this.GetMissingCollectionReturnCodes(
                    collectionPeriods,
                    lastCollectionReturnCode,
                    summarisationMessage.CollectionReturnCode);

            foreach (var periodCode in newCollectionPeriodsToAdd)
            {
                var collectionReturnId = await this.InsertCollectionReturnAsync(
                    new CollectionReturn() { CollectionType = CollectionType.ESF.ToString(), CollectionReturnCode = periodCode },
                    sqlConnection,
                    sqlTransaction);

                await this._summarisedActualsPersist.Save(summarisedActuals, collectionReturnId, sqlConnection, sqlTransaction, cancellationToken);
            }
        }

        public bool CheckESFPeriodCollectionMissing(
            ICollectionReturn newCollectionReturn,
            ICollectionReturn lastCollectionReturn,
            ISummarisationMessage summarisationMessage)
        {
            return summarisationMessage.CollectionType == CollectionType.ESF.ToString()
                         && lastCollectionReturn != null
                         && (!lastCollectionReturn.CollectionReturnCode.Equals(
                             newCollectionReturn.CollectionReturnCode, StringComparison.OrdinalIgnoreCase)
                             || this.GetRemainingMissingPeriods(lastCollectionReturn.CollectionReturnCode, newCollectionReturn.CollectionReturnCode) > 1);
        }

        public int GetRemainingMissingPeriods(string lastCollectionReturnCode, string newCollectionReturnCode)
        {
            return this.GetPeriodFromCode(newCollectionReturnCode) - (this.GetPeriodFromCode(lastCollectionReturnCode) + 1);
        }

        public List<string> GetMissingCollectionReturnCodes(
            IEnumerable<CollectionPeriod> collectionPeriods,
            string lastCollectionReturnCode,
            string newCollectionReturnCode)
        {
            List<string> missingCollectionReturnCodes = new List<string>();

            var lastCollectionPeriod = this.GetCollectionPeriodDetails(collectionPeriods, this.GetPeriodFromCode(lastCollectionReturnCode));
            var newCollectionPeriod = this.GetCollectionPeriodDetails(collectionPeriods, this.GetPeriodFromCode(newCollectionReturnCode));

            int newCollectionMonth = lastCollectionPeriod.CollectionYear == newCollectionPeriod.CollectionYear ? lastCollectionPeriod.CollectionMonth + 1 : 1;
            int newCollectionYear = newCollectionPeriod.CollectionYear;

            for (int month = newCollectionMonth; month < newCollectionPeriod.CollectionMonth; month++)
            {
                missingCollectionReturnCodes.Add($"{CollectionType.ESF.ToString()}{month.ToString("D2")}");
            }

            return missingCollectionReturnCodes;
        }

        public CollectionPeriod GetCollectionPeriodDetails(IEnumerable<CollectionPeriod> collectionPeriods, int period)
        {
            return collectionPeriods.Where(c => c.Period == period).FirstOrDefault();
        }

        public int GetPeriodFromCode(string collectionReturnCode)
        {
            Regex re = new Regex(@"\d+");
            Match m = re.Match(collectionReturnCode);

            return m.Success ? int.Parse(m.Value) : 0;
        }
    }
}