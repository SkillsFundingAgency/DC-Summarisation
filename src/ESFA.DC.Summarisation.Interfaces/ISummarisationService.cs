using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.output.Model;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationService
    {
        IEnumerable<SummarisedActual> Summarise(FundingType fundingType, IProvider provider);
    }
}
