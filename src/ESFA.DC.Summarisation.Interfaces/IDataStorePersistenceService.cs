using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Service.Model;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IDataStorePersistenceService
    {
        Task StoreSummarisedActualsDataAsync(IList<SummarisedActual> summarisedActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}