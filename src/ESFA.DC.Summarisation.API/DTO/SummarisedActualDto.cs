using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.API.DTO
{
    public class SummarisedActualDto
    {
        public string OrganisationId { get; set; }
        public string UoPCode { get; set; }
        public string FundingStreamPeriodCode { get; set; }
        public int Period { get; set; }
        public int DeliverableCode { get; set; }
        public int ActualVolume { get; set; }
        public decimal ActualValue { get; set; }
        public string PeriodTypeCode { get; set; }
        public string ContractAllocationNumber { get; set; }
    }
}
