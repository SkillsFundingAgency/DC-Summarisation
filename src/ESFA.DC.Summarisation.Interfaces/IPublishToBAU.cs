using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IPublishToBAU
    {
       Task Publish(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}
