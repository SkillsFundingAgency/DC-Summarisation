﻿using System.Collections.Generic;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Console.Stubs
{
    public class AppsSummarisationContextStub : ISummarisationMessage
    {
        public string ProcessType => "Fundline";

        public string CollectionType => "Apps1819";

        public string CollectionReturnCode => "R01";

        public ICollection<string> SummarisationTypes => new List<string> { "Apps1819_Levy", "Apps1819_NonLevy" };

        public int? Ukprn => 0;

        public int CollectionYear => 1819;

        public int CollectionMonth => 1;

        public bool RerunSummarisation => true;

        public bool PublishToBAU => false;
    }
}
