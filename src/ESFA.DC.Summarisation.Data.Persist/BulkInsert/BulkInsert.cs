using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using FastMember;

namespace ESFA.DC.Summarisation.Data.Persist.BulkInsert
{
   public class BulkInsert : IBulkInsert
    {

        private List<string> sourceColumnNames = new List<string>
                        { "CollectionReturnId",
                        "OrganisationId",
                        "UoPcode",
                        "FundingStreamPeriodCode",
                        "Period",
                        "DeliverableCode",
                        "ActualVolume",
                        "ActualValue",
                        "PeriodTypeCode",
                        "ContractAllocationNumber"};


        public async Task Insert<T>(string table, IEnumerable<T> source, SqlConnection sqlConnection, CancellationToken cancellationToken)
        {
            using (var sqlBulkCopy = BuildSqlBulkCopy(sqlConnection))
            {
                try
                {
                    if (!source.Any())
                    {
                        return;
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    using (var reader = ObjectReader.Create(source, sourceColumnNames.ToArray()))
                    {
                        sqlBulkCopy.DestinationTableName = table;

                        sqlBulkCopy.ColumnMappings.Add("CollectionReturnId", "CollectionReturnId");
                        sqlBulkCopy.ColumnMappings.Add("OrganisationId", "OrganisationId");
                        sqlBulkCopy.ColumnMappings.Add("UoPcode", "UoPCode");
                        sqlBulkCopy.ColumnMappings.Add("FundingStreamPeriodCode", "FundingStreamPeriodCode");
                        sqlBulkCopy.ColumnMappings.Add("Period", "Period");
                        sqlBulkCopy.ColumnMappings.Add("DeliverableCode", "DeliverableCode");
                        sqlBulkCopy.ColumnMappings.Add("ActualVolume", "ActualVolume");
                        sqlBulkCopy.ColumnMappings.Add("ActualValue", "ActualValue");
                        sqlBulkCopy.ColumnMappings.Add("PeriodTypeCode", "PeriodTypeCode");
                        sqlBulkCopy.ColumnMappings.Add("ContractAllocationNumber", "ContractAllocationNumber");


                        await sqlBulkCopy.WriteToServerAsync(reader, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(table);
                    Console.Write(ex);
                    throw;
                }
            }
        }

        // transaction is null in order to use existing Connection with Options overload.
        private SqlBulkCopy BuildSqlBulkCopy(SqlConnection sqlConnection)
        {
            return new SqlBulkCopy(sqlConnection)
            {
                BatchSize = 5_000, // https://stackoverflow.com/questions/779690/what-is-the-recommended-batch-size-for-sqlbulkcopy
                BulkCopyTimeout = 600
            };
        }
    }
}
