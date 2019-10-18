﻿using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Console.Stubs
{
    public class SummarisationContextStub : ISummarisationMessage
    {
        public string ProcessType => "Fundline";

        public string CollectionType => "ILR1819";

        public string CollectionReturnCode => "R01";

        public IEnumerable<string> SummarisationTypes => new List<string> { "Main1819_FM35" };

        public int? Ukprn => 0;

        public int CollectionYear => 1819;

        public int CollectionMonth => 1;

        public bool RerunSummarisation => true;
    }
}
