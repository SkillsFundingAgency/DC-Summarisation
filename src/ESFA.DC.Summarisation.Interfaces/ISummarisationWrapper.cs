using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Output.Model;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationWrapper
    {
        Task<IEnumerable<SummarisedActual>> Summarise(IEnumerable<string> fundModels,
                            ISummarisationMessage summarisationMessage, 
                            CancellationToken cancellationToken);
    }
}
