using System.Collections.Generic;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist.Mapper.Interface
{
    public interface ISummarisedActualsMapper
    {
        IEnumerable<SummarisedActual> MapSummarisedActuals(IEnumerable<Output.Model.SummarisedActual> actuals, CollectionReturn collectionReturn);

        IEnumerable<SummarisedActual> MapSummarisedActuals(IEnumerable<Output.Model.SummarisedActual> actuals, int collectionReturnId);
    }
}
