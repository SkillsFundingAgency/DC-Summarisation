using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interface;
using ESFA.DC.Summarisation.Interfaces;

namespace ESF.DC.Summarisation.Main1819.Data.Repository
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly IList<ILearningDeliveryProvider> _providers;

        public ProviderRepository(IList<ILearningDeliveryProvider> providers)
        {
            _providers = providers;
        }

        public async Task<IProvider> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            var taskResults = await Task.WhenAll(_providers.Select(p => p.ProvideAsync(ukprn, cancellationToken)));

            return new Provider
            {
                UKPRN = ukprn,
                LearningDeliveries = taskResults.SelectMany(x => x).ToList()
            };
        }

        public async Task<IList<int>> GetAllProviderIdentifiersAsync(CancellationToken cancellationToken)
        {
            var taskResults = await Task.WhenAll(_providers.Select(p => p.ProvideUkprnsAsync(cancellationToken)));

            return taskResults.SelectMany(p => p).Distinct().ToList();
        }
    }
}
