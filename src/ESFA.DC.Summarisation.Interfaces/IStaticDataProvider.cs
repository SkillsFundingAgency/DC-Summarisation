using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IStaticDataProvider<T>
    {
        IEnumerable<T> Provide();
    }
}
