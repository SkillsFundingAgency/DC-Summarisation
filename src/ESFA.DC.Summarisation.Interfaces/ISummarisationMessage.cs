using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationMessage
    {
        string ProcessType { get; }

        string CollectionType { get; }

        string CollectionReturnCode { get; }

        int? Ukprn { get; }

        ICollection<string> SummarisationTypes { get; }

        int CollectionYear { get; }

        int CollectionMonth { get; }

        bool PublishToBAU { get; }
    }
}
