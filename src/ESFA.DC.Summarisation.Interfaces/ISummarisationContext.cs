using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationContext
    {
        string ProcessType { get; }

        string CollectionType { get; }

        string CollectionReturnCode { get; }

        string Ukprn { get; }

        IEnumerable<string> SummarisationTypes { get; }
    }
}
