using System.Collections.Generic;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;

namespace ESFA.DC.Summarisation.Generic.Interfaces
{
    public interface ISummarisationService
    {
       ICollection<SummarisedActual> Summarise(ICollection<FcsContractor> fcsContractors, ICollection<SummarisedActual> summarisedActuals);
    }
}
