using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Model;

namespace ESFA.DC.Summarisation.Service
{
    public abstract class AbstractLearningProviderProvider : ISummarisationInputDataProvider<ILearningProvider>
    {
        public abstract string SummarisationType { get; }

        public abstract string CollectionType { get; }

        public async Task<ILearningProvider> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            return new LearningProvider()
            {
                UKPRN = ukprn,
                LearningDeliveries = await ProvideLearningDeliveriesAsync(ukprn, summarisationMessage, cancellationToken)
            };            
        }

        public abstract Task<IList<LearningDelivery>> ProvideLearningDeliveriesAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken);

        public abstract Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken);
    }
}
