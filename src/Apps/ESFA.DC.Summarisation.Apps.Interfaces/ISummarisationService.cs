﻿using System.Collections.Generic;
using ESFA.DC.Summarisation.Apps.Model;
using ESFA.DC.Summarisation.Apps.Model.Config;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;

namespace ESFA.DC.Summarisation.Apps.Interfaces
{
    public interface ISummarisationService
    {
        ICollection<SummarisedActual> Summarise(ICollection<FundingStream> fundingStreams, LearningProvider provider, ICollection<FcsContractAllocation> allocations, ICollection<CollectionPeriod> collectionPeriods, ISummarisationMessage summarisationMessage);
    }
}
