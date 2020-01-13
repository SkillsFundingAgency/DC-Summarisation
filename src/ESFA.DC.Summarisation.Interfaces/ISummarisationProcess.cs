using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Output.Model;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationProcess
    {
        Task<IEnumerable<SummarisedActual>> CollateAndSummariseAsync(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
