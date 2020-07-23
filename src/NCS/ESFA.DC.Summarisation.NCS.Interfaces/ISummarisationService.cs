using System.Collections.Generic;
using ESFA.DC.Summarisation.NCS.Model;
using ESFA.DC.Summarisation.NCS.Model.Config;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;

namespace ESFA.DC.Summarisation.NCS.Interfaces
{
    public interface ISummarisationService
    {
       ICollection<SummarisedActual> Summarise(ICollection<FundingStream> fundingStreams, TouchpointProviderFundingData provider, ICollection<FcsContractAllocation> allocations, ICollection<CollectionPeriod> collectionPeriods);
    }
}
