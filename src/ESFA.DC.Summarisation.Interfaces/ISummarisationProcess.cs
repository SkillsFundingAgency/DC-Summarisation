using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.output.Model;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationProcess
    {
        string ProcessType { get; }

        Task<ICollection<SummarisedActual>> CollateAndSummariseAsync(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
