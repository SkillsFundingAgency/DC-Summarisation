using ESFA.DC.Summarisation.Data.Input.Interface;
using System.Collections.Generic;
using ESFA.DC.Summarisation.Data.Input.Model;

namespace ESFA.DC.Summarisation.Main1920.Service.Providers
{
    public abstract class AbstractLearningProviderProvider
    {
        protected ILearningProvider BuildLearningProvider(int ukprn, IList<LearningDelivery> learningDeliveries)
        {
            return new LearningProvider()
            {
                UKPRN = ukprn,
                LearningDeliveries = learningDeliveries
            };
        }
    }
}
