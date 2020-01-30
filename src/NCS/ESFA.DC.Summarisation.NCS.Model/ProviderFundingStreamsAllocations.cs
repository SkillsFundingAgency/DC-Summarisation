using ESFA.DC.Summarisation.NCS.Model.Config;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Service.Model
{
    public class ProviderFundingStreamsAllocations 
    {
        public List<FundingStream> FundingStreams  { get; set; }

        public List<FcsContractAllocation> FcsContractAllocations { get; set; }

    }
}
