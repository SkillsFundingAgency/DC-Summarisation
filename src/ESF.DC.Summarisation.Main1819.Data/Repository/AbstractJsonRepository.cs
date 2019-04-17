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

namespace ESF.DC.Summarisation.Main1819.Data.Repository
{
    public abstract class AbstractJsonRepository : IProviderRepository
    {
        private readonly Func<SqlConnection> _sqlConnectionFactory;

        protected abstract string countSql { get; }

        protected abstract string querySql { get; }

        public virtual string SummarisationType { get; }

        public virtual  string CollectionType { get; }

        public AbstractJsonRepository(Func<SqlConnection> sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public virtual async Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(int pageSize, int pageNumber, CancellationToken cancellationToken)
        {
            var offset = (pageNumber - 1) * pageSize;

            using (var connection = _sqlConnectionFactory.Invoke())
            {
                var json = await connection.QueryAsync<string>(querySql, new { offset, pageSize });

                var results = JsonConvert.DeserializeObject<IList<Provider>>(string.Join("", json));

                return results.ToList();
            }
        }

        public virtual async Task<int> RetrieveProviderPageCountAsync(int pageSize, CancellationToken cancellationToken)
        {
            using (var connection = _sqlConnectionFactory.Invoke())
            {
                var providerCount = await connection.ExecuteScalarAsync<int>(countSql);

                return (providerCount % pageSize) > 0
                    ? (providerCount / pageSize) + 1
                    : (providerCount / pageSize);
            }
        }
    }
}
