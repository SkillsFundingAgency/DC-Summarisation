using ESFA.DC.Summarisation.ESF.Model;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.ESF.Interfaces
{
    public interface ISummarisationInputDataProvider
    {
        string CollectionType { get; }

        Task<ICollection<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);

        Task<ICollection<int>> ProvideUkprnsAsync(CancellationToken cancellationToken);
    }
}
