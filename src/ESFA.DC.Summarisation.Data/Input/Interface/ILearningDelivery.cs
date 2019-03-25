using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface ILearningDelivery
    {
        string LearnRefNumber { get;  }

        int AimSeqNumber { get;  }

        string Fundline { get;  }

        IList<IPeriodisedData> PeriodisedData { get;  }
    }
}
