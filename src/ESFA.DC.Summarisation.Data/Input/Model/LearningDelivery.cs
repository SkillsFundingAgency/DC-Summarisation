using System.Collections.Generic;
using ESFA.DC.Summarisation.Data.Input.Interface;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class LearningDelivery : ILearningDelivery
    {
        public string LearnRefNumber { get; set; }

        public int AimSeqNumber { get; set; }

        public string Fundline { get; set; }

        public IList<IPeriodisedData> PeriodisedData { get; set; }
    }
}
