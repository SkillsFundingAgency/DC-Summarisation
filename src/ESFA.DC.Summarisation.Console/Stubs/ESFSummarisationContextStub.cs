using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Console.Stubs
{
    public class ESFSummarisationContextStub : ISummarisationMessage
    {
        public string ProcessType => "Deliverable";

        public string CollectionType => "ESF";

        public string CollectionReturnCode => "ESF01";

        public IEnumerable<string> SummarisationTypes => new List<string> { "ESF_SuppData", "ESF_ILRData" };

        public string Ukprn => string.Empty;

        public int CollectionYear => 1819;

        public int CollectionMonth => 1;

        public bool RerunSummarisation => true;
    }
}
