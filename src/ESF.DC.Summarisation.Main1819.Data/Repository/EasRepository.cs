using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Interface;
using Dapper;

namespace ESF.DC.Summarisation.Main1819.Data.Repository
{
    public class EasRepository : IProviderRepository
    {
        public string FundModel { get; }

        private readonly Func<SqlConnection> _sqlConnectionFactory;

        public EasRepository(Func<SqlConnection> sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(int pageSize, int pageNumber, CancellationToken cancellationToken)
        {
            using (var connection = _sqlConnectionFactory.Invoke())
            {
            }
        }

        public Task<int> RetrieveProviderPageCountAsync(int pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
