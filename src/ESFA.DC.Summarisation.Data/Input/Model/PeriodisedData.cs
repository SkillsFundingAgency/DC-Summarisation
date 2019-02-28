using ESFA.DC.Summarisation.Data.Input.Interface;
using  System.Collections.Generic;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class PeriodisedData : IPeriodisedData
    {
        public string AttributeName { get; set; }

        public List<IPeriod> Periods { get; set; }

    }
}
