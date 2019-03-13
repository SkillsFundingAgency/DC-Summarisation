using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface IProvider
    {
        int UKPRN { get;  }
        IList<ILearningDelivery> LearningDeliveries { get;  }
    }
}
