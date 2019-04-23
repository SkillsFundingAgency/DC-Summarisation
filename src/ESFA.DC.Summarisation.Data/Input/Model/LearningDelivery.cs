using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Summarisation.Data.Input.Interface;
using Newtonsoft.Json;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class LearningDelivery : ILearningDelivery
    {
        public string LearnRefNumber { get; set; }

        public int AimSeqNumber { get; set; }

        public string Fundline { get; set; }

        public IList<PeriodisedData> PeriodisedData { get; set; }
    }
}
