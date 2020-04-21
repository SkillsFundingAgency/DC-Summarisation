using System.Collections.Generic;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Console.Stubs
{
    public class ILR1920SummarisationContextStub : ISummarisationMessage
    {
        public string ProcessType => "Fundline";

        public string CollectionType => "ILR1920";

        public string CollectionReturnCode => "R01";

        public ICollection<string> SummarisationTypes => new List<string> { "Main1920_FM35", "Main1920_EAS" };

        public int? Ukprn => 0;

        public int CollectionYear => 1920;

        public int CollectionMonth => 1;

        public bool RerunSummarisation => true;

        public bool PublishToBAU => false;
    }
}
