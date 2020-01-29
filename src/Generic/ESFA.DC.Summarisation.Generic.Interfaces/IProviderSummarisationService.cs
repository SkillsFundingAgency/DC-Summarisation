using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;

namespace ESFA.DC.Summarisation.Generic.Interfaces
{
    public interface IProviderSummarisationService<T>
    {
        Task<ICollection<SummarisedActual>> Summarise(T inputData, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
