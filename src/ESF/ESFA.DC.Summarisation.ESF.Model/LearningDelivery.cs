using System.Collections.Generic;

namespace ESFA.DC.Summarisation.ESF.Model
{
    public class LearningDelivery 
    {
        public string ConRefNumber { get; set; }

        public IList<PeriodisedData> PeriodisedData { get; set; }
    }
}
