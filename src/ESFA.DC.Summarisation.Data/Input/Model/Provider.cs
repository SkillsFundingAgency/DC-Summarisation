using ESFA.DC.Summarisation.Data.Input.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class Provider 
    {
        public int UKPRN { get; set; }

        public List<LearningDelivery> LearningDeliveries { get; set; }
    }
}
