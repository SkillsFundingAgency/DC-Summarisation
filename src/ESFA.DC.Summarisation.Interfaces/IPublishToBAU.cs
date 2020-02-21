using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IPublishToBAU
    {
       Task PublishAsync(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
