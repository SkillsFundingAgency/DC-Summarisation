using System.Collections.Generic;
using ESFA.DC.Summarisation.Data.Input.Interface;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class SummarisationMessage : ISummarisationMessage
    {
        public string CollectionType { get; set; }

        public string CollectionReturnCode { get; set; }

        public IEnumerable<string> FundModels { get; set; }
    }
}
