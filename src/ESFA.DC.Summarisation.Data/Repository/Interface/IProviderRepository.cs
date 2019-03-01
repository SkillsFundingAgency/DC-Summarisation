﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.Input.Interface;

namespace ESFA.DC.Summarisation.Data.Repository.Interface
{
    public interface IProviderRepository
    {
        FundModel fundModel { get; }

        Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(CancellationToken cancellationToken);

        Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(int pageSize, int pageNumber, CancellationToken cancellationToken);

        Task<int> RetrieveProviderPageCountAsync(int pageSize, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(int Ukprn, CancellationToken cancellationToken);
    }
}
