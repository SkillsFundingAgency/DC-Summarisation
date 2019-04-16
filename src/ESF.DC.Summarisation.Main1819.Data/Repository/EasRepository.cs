using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Interface;
using Dapper;
using ESFA.DC.Summarisation.Data.Input.Model;
using Newtonsoft.Json;

namespace ESF.DC.Summarisation.Main1819.Data.Repository
{
    public class EasRepository : IProviderRepository
    {
        private readonly Func<SqlConnection> _sqlConnectionFactory;

        private const string providerCountsql = @"SELECT COUNT(DISTINCT UKPRN) FROM EAS_Submission";

        private const string querySql = @";WITH Payments_CTE AS
                                            (
                                                SELECT
                                                    [S].[UKPRN],
                                                    [S].[CollectionPeriod],
                                                    [SV].[PaymentValue],
                                                    [PT].[PaymentName]
                                                FROM [EAS_Submission_Values] [SV]
                                                INNER JOIN [Payment_Types] [PT]
                                                    ON [SV].[Payment_Id] = [PT].[Payment_Id]
                                                INNER JOIN [EAS_Submission] [S]
                                                    ON [S].[Submission_Id] = [SV].[Submission_Id]
                                                    AND [S].[CollectionPeriod] = [SV].[CollectionPeriod]
                                            ),
                                            UKPRN_CTE AS
                                            (
                                                SELECT
                                                    [UKPRN]
                                                FROM [EAS_Submission]
                                                GROUP BY [UKPRN]
                                            ),
                                            PaymentTypes_CTE AS
                                            (
                                                SELECT
                                                    [PaymentName],
                                                    [UKPRN]
                                                FROM [Payment_Types] [PT]
                                                INNER JOIN [EAS_Submission_Values] [SV]
                                                    ON [PT].[Payment_Id] = [SV].[Payment_Id]
                                                INNER JOIN [EAS_Submission] [S]
                                                    ON [S].[Submission_Id] = [SV].[Submission_Id]
                                                    AND [S].[CollectionPeriod] = [SV].[CollectionPeriod]
                                                GROUP BY [PaymentName], [UKPRN]
                                            )
                                            SELECT
                                            [UKPRN],
                                                JSON_QUERY((
                                                    SELECT
                                                        0 AS AimSeqNumber,
                                                        [PT].[PaymentName] AS FundLine,
                                                        JSON_QUERY((
                                                            SELECT
                                                                [PT].[PaymentName] AS AttributeName,
                                                                JSON_QUERY((
                                                                    SELECT
                                                                        [IP].[CollectionPeriod] AS PeriodId,
                                                                        [IP].[PaymentValue] AS Value
                                                                    FROM Payments_CTE IP
                                                                    WHERE [IP].[UKPRN] = [UKPRN_CTE].[UKPRN]
                                                                    AND [PT].[PaymentName] = [IP].[PaymentName]
                                                                    FOR JSON PATH
                                                                )) AS Periods
                                                            FOR JSON PATH
                                                        )) AS PeriodisedData
                                                    FROM PaymentTypes_CTE PT
                                                    WHERE [UKPRN_CTE].[UKPRN] = [PT].[UKPRN]
                                                    FOR JSON PATH
                                                )) AS LearningDeliveries
                                            FROM UKPRN_CTE
                                            ORDER BY UKPRN ASC
                                            OFFSET @offSet ROWS
                                            FETCH NEXT @pageSize ROWS ONLY
                                            FOR JSON PATH";

        public string SummarisationType => nameof(ESFA.DC.Summarisation.Configuration.Enum.SummarisationType.Main1819_EAS);

        public string CollectionType => nameof(ESFA.DC.Summarisation.Configuration.Enum.CollectionType.ILR1819);

        public EasRepository(Func<SqlConnection> sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(int pageSize, int pageNumber, CancellationToken cancellationToken)
        {
            var offset = (pageNumber - 1) * pageSize;

            using (var connection = _sqlConnectionFactory.Invoke())
            {
                var json = await connection.QueryAsync<string>(querySql, new { offset, pageSize });

                var results = JsonConvert.DeserializeObject<IList<Provider>>(string.Join("", json));

                return results.ToList();
            }
        }

        public async Task<int> RetrieveProviderPageCountAsync(int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _sqlConnectionFactory.Invoke())
            {
                var providerCount = await connection.ExecuteScalarAsync<int>(providerCountsql);

                return (providerCount % pageSize) > 0
                    ? (providerCount / pageSize) + 1
                    : (providerCount / pageSize);
            }
        }
    }
}
