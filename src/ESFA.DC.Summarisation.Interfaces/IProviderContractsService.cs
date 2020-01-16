using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IProviderContractsService
    {
        Task<IProviderFundingStreamsAllocations> GetProviderContracts(int UKPRN, ICollection<FundingStream> fundingStreams, IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> contractAllocations, CancellationToken cancellationToken);        
    }
}
