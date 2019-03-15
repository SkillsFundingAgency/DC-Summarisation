using ESFA.DC.Summarisation.Configuration.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Configuration
{
    public class SummarisationDataOptions : ISummarisationDataOptions
    {
        public string FCSConnectionString { get; set; }

        public string ILR1819ConnectionString { get; set; }
    }
}
