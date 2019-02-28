using ESFA.DC.Summarisation.Data.Input.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class LearningDelivery 
    {
        public string LearnRefNumber { get; set; }
        public int AimSeqNumber { get; set; }
        public string Fundline { get; set; }
        public List<PeriodisedData> PeriodisedData { get; set; }

    }
}
