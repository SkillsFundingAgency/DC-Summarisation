using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interface;
using Newtonsoft.Json;

namespace ESFA.DC.Summarisation.Main1819.Data.Repository
{
    public class Fm35Repository : IProviderRepository
    {
        private readonly Func<SqlConnection> _sqlConnectionFactory;

        private const string providerCountsql = @"SELECT COUNT(DISTINCT UKPRN) FROM Rulebase.FM35_Learner";

        private const string querySql = @";WITH UKPRN_CTE AS
                                        (
                                            SELECT
                                                UKPRN
                                            FROM Rulebase.FM35_Learner
	                                        GROUP BY UKPRN
                                            ORDER BY UKPRN ASC
                                            OFFSET @offSet ROWS
                                            FETCH NEXT @pageSize ROWS ONLY
                                        ),
                                        Periods_CTE AS
                                        (
                                            SELECT
                                                AttributeUnpivot.UKPRN,
                                                LearnRefNumber,
                                                AimSeqNumber,
                                                AttributeName,
		                                        JSON_QUERY((
			                                        SELECT
				                                        CAST(SUBSTRING(PeriodId, 8,2) AS INT) as Period,
				                                        Value
			                                        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
		                                        )) AS Json
	                                        FROM Rulebase.FM35_LearningDelivery_PeriodisedValues LDPV
                                            UNPIVOT
                                            (
                                                Value
                                                FOR PeriodId IN (Period_1, Period_2, Period_3, Period_4, Period_5, Period_6, Period_7, Period_8, Period_9, Period_10, Period_11, Period_12)
                                            ) as AttributeUnpivot
	                                        INNER JOIN UKPRN_CTE
		                                        ON UKPRN_CTE.UKPRN = AttributeUnpivot.UKPRN
                                        ),
                                        Attributes_CTE AS
                                        (
	                                        SELECT 
		                                        UKPRN,
		                                        LearnRefNumber,
		                                        AimSeqNumber,
		                                        AttributeName,
		                                        Json = JSON_QUERY((
			                                        SELECT
				                                        AttributeName,
				                                        Periods = JSON_QUERY('[' + STRING_AGG(Json, ',') + ']')
			                                        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
		                                        ))
	                                        FROM Periods_CTE
	                                        GROUP BY
		                                        UKPRN,
		                                        LearnRefNumber,
		                                        AimSeqNumber,
		                                        AttributeName
                                        ),
                                        LearningDelivery_CTE AS
                                        (
	                                        SELECT
		                                        ACTE.UKPRN,
		                                        ACTE.LearnRefNumber,
		                                        ACTE.AimSeqNumber,
		                                        Json = JSON_QUERY((
			                                        SELECT
				                                        ACTE.LearnRefNumber,
				                                        ACTE.AimSeqNumber,
				                                        LD.FundLine,
				                                        PeriodisedData = JSON_QUERY('[' + STRING_AGG(Json, ',') + ']')
			                                        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
		                                        ))
	                                        FROM Attributes_CTE ACTE
	                                        INNER JOIN Rulebase.FM35_LearningDelivery LD
	                                        ON ACTE.UKPRN = LD.UKPRN
		                                        AND ACTE.LearnRefNumber = LD.LearnRefNumber
		                                        AND ACTE.AimSeqNumber = LD.AimSeqNumber
		                                        
	                                        GROUP BY
		                                        ACTE.UKPRN,
		                                        ACTE.LearnRefNumber,
		                                        ACTE.AimSeqNumber,
		                                        LD.FundLine
                                        ),
                                        Providers_CTE AS
                                        (
	                                        SELECT
		                                        UKPRN,
		                                        Json = JSON_QUERY((
			                                        SELECT
				                                        UKPRN,
				                                        LearningDeliveries = JSON_QUERY('[' + STRING_AGG(Json, ',') + ']') 
			                                        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
		                                        ))
	                                        FROM LearningDelivery_CTE
	                                        GROUP BY UKPRN
                                        )
                                        SELECT
	                                        JSON_QUERY('[' + STRING_AGG(Json, ',') + ']') 
                                        FROM Providers_CTE";

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.Main1819_FM35);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.ILR1819);

        public Fm35Repository(Func<SqlConnection> sqlConnectionFactory)
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
