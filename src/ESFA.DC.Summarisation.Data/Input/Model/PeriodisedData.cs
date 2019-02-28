using ESFA.DC.Summarisation.Data.Input.Interface;
using  System.Collections.Generic;

namespace ESFA.DC.Summarisation.Data.Input.Model
{
    public class PeriodisedData 
    {
        public string AttributeName { get; set; }

        public List<Period> Periods { get; set; }

    }
}
