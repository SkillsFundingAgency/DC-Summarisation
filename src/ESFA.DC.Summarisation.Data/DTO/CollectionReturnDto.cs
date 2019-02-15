using System;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Data.DTO
{
    public class CollectionReturnDto
    {
        public string CollectionType { get; set; }

        public string CollectionReturnCode { get; set; }

        public DateTime DateTime { get; set; }

        public List<SummarisedActualDto> SummarisedActuals { get; set; }
    }
}
