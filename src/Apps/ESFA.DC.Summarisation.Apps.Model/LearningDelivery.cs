using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Apps.Model
{
    public class LearningDelivery 
    {
        public string Fundline { get; set; }

        public IList<PeriodisedData> PeriodisedData { get; set; }
    }
}
