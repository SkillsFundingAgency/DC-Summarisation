using ESFA.DC.Summarisation.Service.Model.Fcs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Data.Repository.Interface
{
    public interface IFcsRepository
    {
        Task<ICollection<FcsContractAllocation>> RetrieveContractAllocationsAsync(IEnumerable<string> fundingStreamPeriodCodes, CancellationToken cancellationToken);

        Task<ICollection<FcsContractor>> RetrieveContractorForUkprnAsync(IEnumerable<int> ukprns, CancellationToken cancellationToken);
    }
}
