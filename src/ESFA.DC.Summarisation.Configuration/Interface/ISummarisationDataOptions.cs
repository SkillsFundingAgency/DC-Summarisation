using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Configuration.Interface
{
    public interface ISummarisationDataOptions
    {
        string FCSConnectionString { get; }

        string ILR1819ConnectionString { get; }

        string SummarisedActualsConnectionString { get; }

    }
}
