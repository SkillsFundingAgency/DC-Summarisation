using ESFA.DC.Summarisation.Main.Model;
using ESFA.DC.Summarisation.Service.Model.Config;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Main.Interfaces
{
    public interface IProviderContractsService
    {
        Task<ProviderFundingStreamsAllocations> GetProviderContracts(int UKPRN, ICollection<FundingStream> fundingStreams, ICollection<FcsContractAllocation> contractAllocations, CancellationToken cancellationToken);
    }
}
