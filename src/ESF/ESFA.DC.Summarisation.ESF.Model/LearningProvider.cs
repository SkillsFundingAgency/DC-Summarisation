using System.Collections.Generic;

namespace ESFA.DC.Summarisation.ESF.Model
{
    public class LearningProvider
    {
        public int UKPRN { get; set; }

        public ICollection<LearningDelivery> LearningDeliveries { get; set; }
    }
}
