using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface ILearningDelivery
    {
        string LearnRefNumber { get;  }
        int AimSeqNumber { get;  }
        string Fundline { get;  }
        List<IPeriodisedData> PeriodisedData { get;  }
    }
}
