using System.Collections.Generic;

namespace ESFA.DC.Summarisation.ESF.Model
{
    public class PeriodisedData
    {
        public string AttributeName { get; set; }

        public string DeliverableCode { get; set; }

        public IList<Period> Periods { get; set; }
    }
}
