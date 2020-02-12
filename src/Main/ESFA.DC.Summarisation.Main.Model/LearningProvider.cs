using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Main.Model
{
    public class LearningProvider
    {
        public int UKPRN { get; set; }

        public ICollection<LearningDelivery> LearningDeliveries { get; set; }
    }
}
