using System.Collections.Generic;
using ESFA.DC.Summarisation.Data.BAU.Persist.Model;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Data.BAU.Persist.Interfaces
{
    public interface ISummarisedActualsPersistBAU
    {
        Task Save(IEnumerable<SummarisedActualBAU> summarisedActuals, SqlConnection sqlConnection, SqlTransaction sqlTransaction, CancellationToken cancellationToken);
    }
}
