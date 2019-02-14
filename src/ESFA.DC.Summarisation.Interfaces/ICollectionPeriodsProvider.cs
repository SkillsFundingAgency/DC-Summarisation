using ESFA.DC.Summarisation.Configuration;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ICollectionPeriodsProvider
    {
        IEnumerable<CollectionPeriod> Provide();
    }
}
