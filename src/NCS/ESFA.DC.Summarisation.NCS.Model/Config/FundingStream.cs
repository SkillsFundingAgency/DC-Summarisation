using System.Collections.Generic;

namespace ESFA.DC.Summarisation.NCS.Model.Config
{
    public class FundingStream
    {
        public string PeriodCode { get; set; }

        public int DeliverableLineCode { get; set; }

        public List<int> OutcomeTypes { get; set; }
    }
}
