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
    public class ProviderRepository : IProviderRepository
    {
        private readonly IList<ILearningDeliveryProvider> _providers;

        public ProviderRepository(IList<ILearningDeliveryProvider> providers)
        {
            _providers = providers;
        }

        public async Task<IProvider> ProvideAsync(int ukprn, string collectionType, CancellationToken cancellationToken)
        {
            var taskResults = await Task.WhenAll(_providers.Where(w => w.CollectionType == collectionType).Select(p => p.ProvideAsync(ukprn, cancellationToken)));

            return new Provider
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