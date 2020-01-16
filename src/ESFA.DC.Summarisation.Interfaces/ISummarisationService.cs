using System.Collections.Generic;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Output.Model;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationService
    {
        string ProcessType { get; }

        ICollection<SummarisedActual> Summarise(ICollection<FundingStream> fundingStreams, ILearningProvider provider, ICollection<IFcsContractAllocation> allocations, ICollection<CollectionPeriod> collectionPeriods, ISummarisationMessage summarisationMessage);
    }
}
