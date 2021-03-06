﻿using ESFA.DC.Summarisation.ESF.Interfaces;
using ESFA.DC.Summarisation.ESF.Model;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.ESF.Service
{
    public class ProviderRepository : IInputDataRepository<LearningProvider>
    {
        private readonly IList<ISummarisationInputDataProvider> _providers;

        public ProviderRepository(IList<ISummarisationInputDataProvider> providers)
        {
            _providers = providers;
        }

        public async Task<LearningProvider> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var taskResults = await Task.WhenAll(_providers.Where(w => w.CollectionType == summarisationMessage.CollectionType).Select(p => p.ProvideAsync(ukprn, summarisationMessage, cancellationToken)));

            return new LearningProvider
            {
                UKPRN = ukprn,
                LearningDeliveries = taskResults.SelectMany(x => x).ToList()
            };

        }

        public async Task<ICollection<int>> GetAllIdentifiersAsync(string collectionType, CancellationToken cancellationToken)
        {
            var taskResults = await Task.WhenAll(_providers.Where(w => w.CollectionType == collectionType).Select(p => p.ProvideUkprnsAsync(cancellationToken)));

            return taskResults.SelectMany(p => p).Distinct().ToList();
        }
    }
}