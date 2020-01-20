using System.Collections.Generic;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.output.Model;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Apps.Interfaces
{
    public interface ISummarisationService
    {
        ICollection<SummarisedActual> Summarise(ICollection<FundingStream> fundingStreams, ILearningProvider provider, ICollection<IFcsContractAllocation> allocations, ICollection<CollectionPeriod> collectionPeriods, ISummarisationMessage summarisationMessage);
    }
}
