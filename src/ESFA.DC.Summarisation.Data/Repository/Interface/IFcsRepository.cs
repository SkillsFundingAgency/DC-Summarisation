using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;

namespace ESFA.DC.Summarisation.Data.Repository.Interface
{
    public interface IFcsRepository
    {
        Task<IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>>> RetrieveAsync(CancellationToken cancellationToken);
    }
}
