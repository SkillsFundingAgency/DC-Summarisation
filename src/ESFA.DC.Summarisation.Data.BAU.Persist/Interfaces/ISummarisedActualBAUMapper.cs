using System.Collections.Generic;
using ESFA.DC.Summarisation.Service.Model;

namespace ESFA.DC.Summarisation.Data.BAU.Persist.Interfaces
{
    public interface ISummarisedActualBAUMapper
    {
        ICollection<Model.SummarisedActualBAU> Map(IEnumerable<SummarisedActual> summarisedActuals, string collectionType, string collectionReturnCode);
    }
}
