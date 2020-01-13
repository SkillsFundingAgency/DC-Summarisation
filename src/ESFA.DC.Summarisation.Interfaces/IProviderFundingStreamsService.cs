using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IProviderFundingStreamsService
    {
        Task<List<FundingStream>> GetProviderFundingStreams(int UKPRN, string collectionType, string summarisationType, List<IFcsContractAllocation> providerAllocations, CancellationToken cancellationToken);
    }
}
