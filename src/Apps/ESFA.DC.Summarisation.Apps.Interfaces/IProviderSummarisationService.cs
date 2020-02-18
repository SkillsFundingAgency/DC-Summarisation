using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Apps.Model.Config;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;

namespace ESFA.DC.Summarisation.Apps.Interfaces
{
    public interface IProviderSummarisationService<T>
    {
        Task<ICollection<SummarisedActual>> Summarise(T inputData, ICollection<CollectionPeriod> collectionPeriods, ICollection<FundingType> fundingTypes, ICollection<FcsContractAllocation> fcsContractAllocations, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
