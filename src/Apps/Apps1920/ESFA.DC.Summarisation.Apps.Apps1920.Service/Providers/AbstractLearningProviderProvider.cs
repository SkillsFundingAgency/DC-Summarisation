using ESFA.DC.Summarisation.Apps.Model;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Apps.Apps1920.Service.Providers
{
    public abstract class AbstractLearningProviderProvider
    {
        protected LearningProvider BuildLearningProvider(int ukprn, IList<LearningDelivery> learningDeliveries)
        {
            return new LearningProvider()
            {
                UKPRN = ukprn,
                LearningDeliveries = learningDeliveries
            };
        }
    }
}