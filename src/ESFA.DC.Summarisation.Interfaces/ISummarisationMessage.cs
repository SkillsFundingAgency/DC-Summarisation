using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationMessage
    {
        string ProcessType { get; }

        string CollectionType { get; }

        string CollectionReturnCode { get; }

        string Ukprn { get; }

        IEnumerable<string> SummarisationTypes { get; }

        int CollectionYear { get; }

        int CollectionMonth { get; }

        bool RerunSummarisation { get; }
    }
}
