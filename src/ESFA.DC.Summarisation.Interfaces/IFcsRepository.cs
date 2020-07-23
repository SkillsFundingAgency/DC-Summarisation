using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Service.Model.Fcs;

namespace ESFA.DC.Summarisation.Data.Repository.Interface
{
    public interface IFcsRepository
    {
        Task<ICollection<FcsContractAllocation>> RetrieveContractAllocationsAsync(IEnumerable<string> fundingStreamPeriodCodes, CancellationToken cancellationToken);
    }
}
