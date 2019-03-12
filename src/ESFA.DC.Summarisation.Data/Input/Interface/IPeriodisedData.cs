using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface IPeriodisedData
    {
        string AttributeName { get; }

        IList<IPeriod> Periods { get;  }
    }
}
