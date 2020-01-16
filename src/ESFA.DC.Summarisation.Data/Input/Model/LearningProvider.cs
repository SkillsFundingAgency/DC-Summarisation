using System.Collections.Generic;
using ESFA.DC.Summarisation.Data.Input.Interface;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class LearningProvider : ILearningProvider
    {
        public int UKPRN { get; set; }

        public ICollection<LearningDelivery> LearningDeliveries { get; set; }
    }
}
