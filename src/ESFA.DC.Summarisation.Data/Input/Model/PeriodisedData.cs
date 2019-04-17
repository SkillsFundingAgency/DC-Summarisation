using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Summarisation.Data.Input.Interface;
using Newtonsoft.Json;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class PeriodisedData : IPeriodisedData
    {
        public string AttributeName { get; set; }

        public IList<Period> Periods { get; set; }
    }
}
