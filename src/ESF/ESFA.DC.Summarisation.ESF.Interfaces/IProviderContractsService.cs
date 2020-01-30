using ESFA.DC.Summarisation.ESF.Model;
using ESFA.DC.Summarisation.Service.Model.Config;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.ESF.Interfaces
{
    public interface IProviderContractsService
    {
        Task<ProviderFundingStreamsAllocations> GetProviderContracts(int UKPRN, ICollection<FundingStream> fundingStreams, ICollection<FcsContractAllocation> contractAllocations, CancellationToken cancellationToken);
    }
}
