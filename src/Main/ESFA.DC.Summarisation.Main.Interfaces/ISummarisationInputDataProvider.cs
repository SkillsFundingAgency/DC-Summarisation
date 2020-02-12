using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Main.Interfaces
{
    public interface ISummarisationInputDataProvider
    {
        string CollectionType { get; }

        Task<ICollection<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);

        Task<ICollection<int>> ProvideUkprnsAsync(CancellationToken cancellationToken);
    }
}
