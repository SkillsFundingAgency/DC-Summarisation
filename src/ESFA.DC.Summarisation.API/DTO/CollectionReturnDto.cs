using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.API.DTO
{
    public class CollectionReturnDto
    {
        public string CollectionType { get; set; }

        public string CollectionReturnCode { get; set; }

        public DateTime DateTime { get; set; }

        public List<SummarisedActualDto> SummarisedActuals { get; set; }
    }
}
