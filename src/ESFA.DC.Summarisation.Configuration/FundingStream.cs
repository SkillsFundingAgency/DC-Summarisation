using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Configuration
{
    public class FundingStream
    {
        public string PeriodCode { get; set; }

        public int DeliverableLineCode { get; set; }

        public string DeliverableCode { get; set; }

        public FundModel FundModel { get; set; }

        public List<FundLine> FundLines { get; set; }
    }
}
