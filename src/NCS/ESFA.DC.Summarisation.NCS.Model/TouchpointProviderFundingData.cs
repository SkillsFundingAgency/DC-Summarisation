using System.Collections.Generic;

namespace ESFA.DC.Summarisation.NCS.Model
{
    public class TouchpointProviderFundingData
    {
        public TouchpointProvider Provider { get; set; }

        public ICollection<FundingValue> FundingValues { get; set; }
    }
}
