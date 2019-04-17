using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationConfigProvider<T>
    {
        IEnumerable<T> Provide();
    }
}
