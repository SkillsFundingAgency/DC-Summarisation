using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Service.Model
{
    public class CollectionReturn
    {
        public int Id { get; set; }
        public string CollectionType { get; set; }
        public string CollectionReturnCode { get; set; }
        public DateTime? DateTime { get; set; }
    }
}
