﻿using System.Collections.Generic;
using ESFA.DC.Summarisation.ESF.Model;
using ESFA.DC.Summarisation.ESF.Model.Config;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;

namespace ESFA.DC.Summarisation.ESF.Interfaces
{
    public interface ISummarisationService
    {
        ICollection<SummarisedActual> Summarise(ICollection<FundingStream> fundingStreams, LearningProvider provider, ICollection<FcsContractAllocation> allocations, ICollection<CollectionPeriod> collectionPeriods, ISummarisationMessage summarisationMessage);
    }
}
