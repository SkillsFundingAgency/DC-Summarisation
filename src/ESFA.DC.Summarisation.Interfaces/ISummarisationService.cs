using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.output.Model;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationService
    {
        IEnumerable<SummarisedActual> Summarise(FundingType fundingType, IProvider provider, IEnumerable<IFcsContractAllocation> allocations, IEnumerable<CollectionPeriod> collectionPeriods);

        IEnumerable<SummarisedActual> Summarise(FundingStream fundingStream, IProvider provider, IEnumerable<IFcsContractAllocation> allocations, IEnumerable<CollectionPeriod> collectionPeriods);

        IEnumerable<SummarisedActual> Summarise(List<FundingStream> fundingStreams, IProvider provider, IEnumerable<IFcsContractAllocation> allocations, IEnumerable<CollectionPeriod> collectionPeriods);
    }
}
