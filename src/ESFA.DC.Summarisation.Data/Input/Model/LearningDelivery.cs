using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class LearningDelivery 
    {
        public string LearnRefNumber { get; set; }

        public int AimSeqNumber { get; set; }

        public string Fundline { get; set; }

        public string ConRefNumber { get; set; }

        public string DeliverableCode { get; set; }

        public IList<PeriodisedData> PeriodisedData { get; set; }
    }
}
