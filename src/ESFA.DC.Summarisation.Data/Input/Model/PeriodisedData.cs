using System.Collections.Generic;
using ESFA.DC.Summarisation.Data.Input.Interface;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class PeriodisedData : IPeriodisedData
    {
        public string AttributeName { get; set; }

        public IList<IPeriod> Periods { get; set; }
    }
}
