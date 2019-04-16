using System.Collections.Generic;
using ESFA.DC.Summarisation.Data.Input.Model;

namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface IProvider
    {
        int UKPRN { get;  }

        IList<LearningDelivery> LearningDeliveries { get;  }
    }
}
