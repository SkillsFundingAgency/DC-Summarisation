using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Persist;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Model;
using SummarisedActual = ESFA.DC.Summarisation.Data.Output.Model.SummarisedActual;

namespace ESFA.DC.Summarisation.Modules.Stubs
{
    public class DataStorePersistenceServiceStub : IDataStorePersistenceService
    {
        public Task<CollectionReturn> StoreCollectionReturnAsync(ISummarisationContext summarisationContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CollectionReturn());
        }

        public Task StoreSummarisedActualsDataAsync(IList<SummarisedActual> summarisedActuals, ISummarisationContext summarisationContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}