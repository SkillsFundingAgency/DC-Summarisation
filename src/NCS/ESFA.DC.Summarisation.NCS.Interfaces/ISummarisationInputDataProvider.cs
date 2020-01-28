using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.NCS.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.NCS.Interfaces
{
    public interface ISummarisationInputDataProvider
    {
        string CollectionType { get; }

        Task<ICollection<FundingValue>> ProvideAsync(TouchpointProvider touchpointProvider, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);

        Task<ICollection<TouchpointProvider>> ProvideUkprnsAsync(CancellationToken cancellationToken);
    }
}
