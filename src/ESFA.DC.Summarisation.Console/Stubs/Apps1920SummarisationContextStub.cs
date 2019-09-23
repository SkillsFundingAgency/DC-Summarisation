using ESFA.DC.Summarisation.Interfaces;
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

        public string CollectionType => "Apps1920";

        public string CollectionReturnCode => "Apps28";

        //public IEnumerable<string> SummarisationTypes => new List<string> { "Apps1920_Levy", "Apps1920_NonLevy", "Apps1920_EAS" };

        public IEnumerable<string> SummarisationTypes => new List<string> { "Apps1920_NonLevy" };

        public string Ukprn => string.Empty;

        public int CollectionYear => 1920;

        public int CollectionMonth => 2;

        public bool RerunSummarisation => true;
    }
}
