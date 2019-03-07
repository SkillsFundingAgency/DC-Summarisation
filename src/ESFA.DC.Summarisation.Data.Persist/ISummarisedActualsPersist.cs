using System.Collections.Generic;
using ESFA.DC.Summarisation.Data.Output.Model;

namespace ESFA.DC.Summarisation.Data.Persist
{
    public interface ISummarisedActualsPersist
    {
        void Save(List<SummarisedActual> summarisedActuals);
    }
}
