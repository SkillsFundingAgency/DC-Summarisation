using System;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Model
{
    public partial class CollectionReturn
    {
        public CollectionReturn()
        {
            SummarisedActuals = new HashSet<SummarisedActual>();
        }

        public int Id { get; set; }
        public string CollectionType { get; set; }
        public string CollectionReturnCode { get; set; }

        public virtual ICollection<SummarisedActual> SummarisedActuals { get; set; }
    }
}
