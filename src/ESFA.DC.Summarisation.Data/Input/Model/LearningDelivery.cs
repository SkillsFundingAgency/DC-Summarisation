using ESFA.DC.Summarisation.Data.Input.Interface;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class LearningDelivery : ILearningDelivery
    {
        public string LearnRefNumber { get; set; }
        public int AimSeqNumber { get; set; }
        public string Fundline { get; set; }
        public List<IPeriodisedData> PeriodisedData { get; set; }

    }
}
