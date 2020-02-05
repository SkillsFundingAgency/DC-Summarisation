﻿using System.Collections.Generic;

namespace ESFA.DC.Summarisation.ESF.Model.Config
{
    public class FundingType
    {
        public string SummarisationType { get; set; }

        public List<FundingStream> FundingStreams { get; set; }
    }
}