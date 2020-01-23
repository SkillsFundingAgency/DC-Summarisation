using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Apps.Interfaces
{
    public interface ISummarisationInputDataProvider<T>
    {
        string CollectionType { get; }

        Task<T> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);

        Task<ICollection<int>> ProvideUkprnsAsync(CancellationToken cancellationToken);
    }
}
