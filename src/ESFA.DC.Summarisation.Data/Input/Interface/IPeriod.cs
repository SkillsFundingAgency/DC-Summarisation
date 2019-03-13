using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface IPeriod
    {
        int PeriodId { get; }

        decimal? Value { get; }
    }
}
