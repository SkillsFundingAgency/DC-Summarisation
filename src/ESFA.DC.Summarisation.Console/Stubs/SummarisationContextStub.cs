using ESFA.DC.Summarisation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Console.Stubs
{
    public class SummarisationContextStub : ISummarisationContext
    {
        public string CollectionType => "ILR1819";

        public string CollectionReturnCode => "R01";

        public IEnumerable<string> FundModels => new List<string> { "FM35" };
    }
}
