﻿using ESFA.DC.Summarisation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Console.Stubs
{
    public class Apps1920SummarisationContextStub : ISummarisationMessage
    {
        public string ProcessType => "Payments";

        public string CollectionType => "APPS";

        public string CollectionReturnCode => "APPS29";

        public IEnumerable<string> SummarisationTypes => new List<string> { "Apps1920_Levy", "Apps1920_NonLevy", "Apps1920_EAS" };

        public int? Ukprn => 0;

        public int CollectionYear => 1920;

        public int CollectionMonth => 2;

        public bool RerunSummarisation => true;
    }
}
