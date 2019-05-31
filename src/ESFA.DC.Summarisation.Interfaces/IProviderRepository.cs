using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Interface;

namespace ESFA.DC.Summarisation.Interface
{
    public interface IProviderRepository
    {
        Task<IProvider> ProvideAsync(int ukprn, string collectionType, CancellationToken cancellationToken);

        Task<IList<int>> GetAllProviderIdentifiersAsync(string collectionType, CancellationToken cancellationToken);
    }
}
