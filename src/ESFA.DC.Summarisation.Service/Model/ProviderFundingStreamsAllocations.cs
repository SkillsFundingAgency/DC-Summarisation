using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Service.Model
{
    public class ProviderFundingStreamsAllocations : IProviderFundingStreamsAllocations
    {
        public List<FundingStream> FundingStreams  { get; set; }

        public List<IFcsContractAllocation> FcsContractAllocations { get; set; }

    }
}
