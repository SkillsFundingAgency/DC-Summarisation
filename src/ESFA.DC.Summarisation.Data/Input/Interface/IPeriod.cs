using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface IPeriod
    {
        int Id { get; }

        decimal? Value { get; }
    }
}
