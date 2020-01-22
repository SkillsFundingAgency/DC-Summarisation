﻿using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IProviderFundingStreamsAllocations
    {
        List<FundingStream> FundingStreams { get; set; }

        List<IFcsContractAllocation> FcsContractAllocations { get; set; }
    }
}
