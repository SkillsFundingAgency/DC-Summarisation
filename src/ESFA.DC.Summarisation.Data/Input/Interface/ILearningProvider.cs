using System.Collections.Generic;
using ESFA.DC.Summarisation.Data.Input.Model;

namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface ILearningProvider
    {
        int UKPRN { get;  }

        ICollection<LearningDelivery> LearningDeliveries { get;  }
    }
}
