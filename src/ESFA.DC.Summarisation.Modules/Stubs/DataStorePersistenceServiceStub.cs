using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Modules.Stubs
{
    public class DataStorePersistenceServiceStub : IDataStorePersistenceService
    {
        public Task<CollectionReturn> StoreCollectionReturnAsync(ISummarisationMessage summarisationContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(new CollectionReturn());
        }

        public Task StoreSummarisedActualsDataAsync(IList<Service.Model.SummarisedActual> summarisedActuals, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}