using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;

namespace ESFA.DC.Summarisation.Generic.Interfaces
{
    public interface IProviderSummarisationService<T>
    {
        Task<ICollection<SummarisedActual>> Summarise(T inputData, ISummarisationMessage summarisationMessage, ICollection<FcsContractor> fcsContractors, CancellationToken cancellationToken);
    }
}
