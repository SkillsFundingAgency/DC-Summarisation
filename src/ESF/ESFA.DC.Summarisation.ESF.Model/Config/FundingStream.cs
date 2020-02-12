using System.Collections.Generic;

namespace ESFA.DC.Summarisation.ESF.Model.Config
{
    public class FundingStream
    {
        public string PeriodCode { get; set; }

        public int DeliverableLineCode { get; set; }

        public List<DeliverableLine> DeliverableLines { get; set; }
    }
}
