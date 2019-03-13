﻿using ESFA.DC.Summarisation.Data.Input.Interface;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class Provider : IProvider
    {
        public int UKPRN { get; set; }

        public IList<ILearningDelivery> LearningDeliveries { get; set; }
    }
}