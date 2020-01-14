using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Interface
{
    public interface IInputDataRepository<T>
    {
        Task<T> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);

        Task<IList<int>> GetAllIdentifiersAsync(string collectionType, CancellationToken cancellationToken);
    }
}
