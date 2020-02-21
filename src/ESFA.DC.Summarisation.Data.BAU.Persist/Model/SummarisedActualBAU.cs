namespace ESFA.DC.Summarisation.Data.BAU.Persist.Model
{
    public class SummarisedActualBAU
    {
        public string CollectionReturnCode { get; set; }

        public string OrganisationId { get; set; }

        public string PeriodTypeCode { get; set; }

        public int Period { get; set; }

        public string FundingStreamPeriodCode { get; set; }

        public string CollectionType { get; set; }

        public string ContractAllocationNumber { get; set; }

        public string UoPCode { get; set; }

        public int DeliverableCode { get; set; }

        public int ActualVolume { get; set; }

        public decimal ActualValue { get; set; }
        
    }
}
