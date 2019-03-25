using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface IProvider
    {
        int UKPRN { get;  }

        IList<ILearningDelivery> LearningDeliveries { get;  }
    }
}
