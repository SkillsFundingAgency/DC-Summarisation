using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationInputDataProvider<T>
    {
        string SummarisationType { get; }

        string CollectionType { get; }

        Task<T> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);

        Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken);
    }
}
