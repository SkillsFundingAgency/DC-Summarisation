using ESFA.DC.Summarisation.Data.BAU.Persist.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Data.BAU.Persist.Interfaces
{
    public interface IDataStorePersistenceServiceBAU
    {
        Task StoreSummarisedActualsDataAsync(IEnumerable<SummarisedActualBAU> summarisedActuals, EventLog eventLog, CancellationToken cancellationToken);
    }
}
