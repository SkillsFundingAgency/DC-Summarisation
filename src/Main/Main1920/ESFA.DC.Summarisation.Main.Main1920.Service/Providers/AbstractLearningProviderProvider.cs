using ESFA.DC.Summarisation.Main.Model;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Main1920.Service.Providers
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
