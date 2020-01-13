using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IProviderContractsService
    {
        Task<IProviderFundingStreamsAllocations> GetProviderContracts(int UKPRN, string collectionType, string summarisationType, CancellationToken cancellationToken);
        //Task<List<IFcsContractAllocation>> GetProviderContracts(int UKPRN, string collectionType, string summarisationType, CancellationToken cancellationToken);
    }
}
