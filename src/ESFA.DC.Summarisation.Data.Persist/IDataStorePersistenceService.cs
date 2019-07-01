using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Output.Model;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Data.Persist
{
    public interface IDataStorePersistenceService
    {
        Task StoreSummarisedActualsDataAsync(IList<SummarisedActual> summarisedActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}