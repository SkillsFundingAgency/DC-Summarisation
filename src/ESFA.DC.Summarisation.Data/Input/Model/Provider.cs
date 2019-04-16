using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Summarisation.Data.Input.Interface;
using Newtonsoft.Json;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class Provider : IProvider
    {
        public int UKPRN { get; set; }

        public IList<LearningDelivery> LearningDeliveries { get; set; }
    }
}
