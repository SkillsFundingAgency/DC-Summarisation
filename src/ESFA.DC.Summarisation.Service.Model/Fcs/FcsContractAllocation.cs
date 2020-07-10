namespace ESFA.DC.Summarisation.Service.Model.Fcs
{
    public class FcsContractAllocation
    {
        public int Id { get; set; }

        public string ContractAllocationNumber { get; set; }

        public string FundingStreamPeriodCode { get; set; }

        public string UoPcode { get; set; }

        public int? DeliveryUkprn { get; set; }

        public string DeliveryOrganisation { get; set; }

        public int ContractStartDate { get; set; }

        public int ContractEndDate { get; set; }

        public int ActualsSchemaPeriodStart { get; set; }

        public int ActualsSchemaPeriodEnd { get; set; }
    }
}