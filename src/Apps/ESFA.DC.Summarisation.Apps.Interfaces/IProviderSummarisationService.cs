using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.output.Model;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Apps.Interfaces
{
    public interface IProviderSummarisationService<T>
    {
        Task<ICollection<SummarisedActual>> Summarise(T inputData, ICollection<CollectionPeriod> collectionPeriods, ICollection<FundingType> fundingTypes, IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fcsContractAllocations, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
