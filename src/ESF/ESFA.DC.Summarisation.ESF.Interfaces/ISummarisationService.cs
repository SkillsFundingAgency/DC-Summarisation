using System.Collections.Generic;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Data.output.Model;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.ESF.Interfaces
{
    public interface ISummarisationService
    {
        ICollection<SummarisedActual> Summarise(ICollection<FundingStream> fundingStreams, LearningProvider provider, ICollection<IFcsContractAllocation> allocations, ICollection<CollectionPeriod> collectionPeriods, ISummarisationMessage summarisationMessage);
    }
}
