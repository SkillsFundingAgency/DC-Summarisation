using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Model;

namespace ESFA.DC.Summarisation.Data.Repository.Interface
{
    public interface IIlrRepository
    {
        Task<IReadOnlyCollection<Provider>> RetrieveFM35ProvidersAsync(CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Provider>> RetrieveFM35ProvidersAsync(int pageSize, int pageNumber, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Provider>> RetrieveFM35ProvidersAsync(int Ukprn, CancellationToken cancellationToken);
    }
}
