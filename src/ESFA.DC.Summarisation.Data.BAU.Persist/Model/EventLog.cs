using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Data.BAU.Persist.Model
{
    public class EventLog
    {
        public string CollectionType { get; set; }

        public string CollectionReturnCode { get; set; }

        public DateTime? DateTime { get; set; }
       
    }
}
