using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interface;
using ESFA.DC.Summarisation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Service
{
    //public class ProviderRepository : IProviderRepository<IProvider, int>
    public class ProviderRepository : IProviderRepository
    {
        private readonly IList<ISummarisationInputDataProvider<IList<LearningDelivery>>> _providers;

        public ProviderRepository(IList<ISummarisationInputDataProvider<IList<LearningDelivery>>> providers)
        {
            _providers = providers;
        }

        public async Task<ILearningProvider> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var taskResults = await Task.WhenAll(_providers.Where(w => w.CollectionType == summarisationMessage.CollectionType).Select(p => p.ProvideAsync(ukprn, summarisationMessage, cancellationToken)));

            return new LearningProvider
            {
                UKPRN = ukprn,
                LearningDeliveries = taskResults.SelectMany(x => x).ToList()
            };
        }

        public async Task<IList<int>> GetAllProviderIdentifiersAsync(string collectionType, CancellationToken cancellationToken)
        {
            var taskResults = await Task.WhenAll(_providers.Where(w => w.CollectionType == collectionType).Select(p => p.ProvideUkprnsAsync(cancellationToken)));

            return taskResults.SelectMany(p => p).Distinct().ToList();
        }
    }
}