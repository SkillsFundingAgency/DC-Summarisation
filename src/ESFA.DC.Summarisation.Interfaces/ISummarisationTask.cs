using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationTask
    {
        string TaskName { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
