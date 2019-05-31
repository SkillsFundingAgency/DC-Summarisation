using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationConfigProvider<T>
    {
        string CollectionType { get; }

        IEnumerable<T> Provide();
    }
}
