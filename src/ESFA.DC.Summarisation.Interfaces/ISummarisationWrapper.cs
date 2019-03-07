using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.output.Model;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationWrapper
    {
        Task<IList<SummarisedActual>> SummariseProviders(
                IList<FundingStream> fundingStreams,
                IProviderRepository repository,
                IEnumerable<CollectionPeriod> collectionPeriods,
                IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fcsContractAllocations,
                CancellationToken cancellationToken);
    }
}
