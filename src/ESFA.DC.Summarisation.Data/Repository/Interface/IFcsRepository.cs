using ESFA.DC.Summarisation.Service.Model.Fcs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Data.Repository.Interface
{
    public interface IFcsRepository
    {
        Task<ICollection<FcsContractAllocation>> RetrieveAsync(CancellationToken cancellationToken);
    }
}
