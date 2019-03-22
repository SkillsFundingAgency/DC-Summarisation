using ESFA.DC.Summarisation.Data.Input.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class SummarisationMessage : ISummarisationMessage
    {
        public string CollectionType { get; set; }

        public string CollectionReturnCode { get; set; }

        public IEnumerable<string> FundModels { get; set; }
    }
}
