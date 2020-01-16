using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.output.Model;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Model.Interface;

namespace ESFA.DC.Summarisation.Data.Persist
{
    public interface IDataStorePersistenceService
    {
        Task StoreSummarisedActualsDataAsync(IList<SummarisedActual> summarisedActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);
    }
}