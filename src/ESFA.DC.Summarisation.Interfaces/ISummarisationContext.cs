using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationContext
    {
        string CollectionType { get; }

        string CollectionReturnCode { get; }

        IEnumerable<string> FundModels { get; }
        
    }
}
