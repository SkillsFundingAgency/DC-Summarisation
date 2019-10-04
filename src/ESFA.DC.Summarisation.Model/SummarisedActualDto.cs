using System;
using System.ComponentModel.DataAnnotations;

namespace ESFA.DC.Summarisation.Model
{
    public class SummarisedActualDto
    {
        [Key]
        public Guid Id { get; set; }

        public string CollectionReturnCode { get; set; }

        public string OrganisationId { get; set; }

        public string PeriodTypeCode { get; set; }

        public int Period { get; set; }

        public string FundingStreamPeriodCode { get; set; }

        public string CollectionType { get; set; }

        public string ContractAllocationNumber { get; set; }

        public string UopCode { get; set; }

        public int DeliverableCode { get; set; }

        public int ActualVolume { get; set; }

        public decimal ActualValue { get; set; }
    }
}
