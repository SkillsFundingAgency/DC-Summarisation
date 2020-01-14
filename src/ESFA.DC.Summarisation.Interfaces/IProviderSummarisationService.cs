using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Output.Model;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IProviderSummarisationService<T>
    {
        Task<IEnumerable<SummarisedActual>> Summarise(T inputData, IEnumerable<CollectionPeriod> collectionPeriods, IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fcsContractAllocations, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
