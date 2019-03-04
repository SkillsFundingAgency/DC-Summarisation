using ESFA.DC.Summarisation.Data.output.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Data.Repository.Interface
{
    public interface ISummarisedActualsPersist
    {
        void Save(List<SummarisedActual> summarisedActuals);
    }
}
