using ESFA.DC.Summarisation.Model;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Data.Mapper.Interface
{
    public interface ISummarisedActualsMapper
    {
        IEnumerable<SummarisedActual> MapSummarisedActuals(IEnumerable<output.Model.SummarisedActual> actuals);
    }
}
