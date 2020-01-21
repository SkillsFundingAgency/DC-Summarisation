using System.Collections.Generic;
using ESFA.DC.Summarisation.Data.Input.Model;

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