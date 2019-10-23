using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Model;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ILearningDeliveryProvider
    {
        string SummarisationType { get; }

        string CollectionType { get; }

        Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);

        Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken);
    }
}
