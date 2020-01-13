using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Output.Model;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IProviderSummarisationService
    {
        Task<IEnumerable<SummarisedActual>> SummariseProviderData(IProvider providerData, IEnumerable<CollectionPeriod> collectionPeriods, IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fcsContractAllocations, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
