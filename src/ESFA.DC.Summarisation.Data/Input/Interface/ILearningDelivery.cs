using System.Collections.Generic;
using ESFA.DC.Summarisation.Data.Input.Model;

namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface ILearningDelivery
    {
        string LearnRefNumber { get;  }

        int AimSeqNumber { get;  }

        string Fundline { get;  }

        string ConRefNumber { get; }

        string DeliverableCode { get;}

        IList<PeriodisedData> PeriodisedData { get;  }
    }
}
