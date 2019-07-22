using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Console.Stubs
{
    public class ILR1920SummarisationContextStub : ISummarisationMessage
    {
        public string ProcessType => "Fundline";

        public string CollectionType => "ILR1920";

        public string CollectionReturnCode => "R01";

        public IEnumerable<string> SummarisationTypes => new List<string> { "Main1920_FM35" };

        public string Ukprn => string.Empty;

        public int CollectionYear => 1920;

        public int CollectionMonth => 1;

        public bool RerunSummarisation => true;
    }
}
