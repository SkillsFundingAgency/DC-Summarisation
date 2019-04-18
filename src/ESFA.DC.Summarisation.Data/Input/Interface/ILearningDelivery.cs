using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface ILearningDelivery
    {
        string LearnRefNumber { get;  }

        int AimSeqNumber { get;  }

        string Fundline { get;  }

        string ConRefNumber { get; set; }

        IList<IPeriodisedData> PeriodisedData { get;  }
    }
}
