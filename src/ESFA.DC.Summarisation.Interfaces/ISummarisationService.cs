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

        IEnumerable<SummarisedActual> Summarise(List<FundingStream> fundingStreams, ILearningProvider provider, IEnumerable<IFcsContractAllocation> allocations, IEnumerable<CollectionPeriod> collectionPeriods, ISummarisationMessage summarisationMessage);
    }
}
