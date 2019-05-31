using ESFA.DC.Summarisation.Data.External.FCS.Interface;

namespace ESFA.DC.Summarisation.Data.External.FCS.Model
{
    public class FcsContractAllocation : IFcsContractAllocation
    {
        public int Id { get; set; }

        public string ContractAllocationNumber { get; set; }

        public string FundingStreamPeriodCode { get; set; }

        public string UoPcode { get; set; }

        public int? DeliveryUkprn { get; set; }

        public string DeliveryOrganisation { get; set; }

        public int ContractStartDate { get; set; }

        public int ContractEndDate { get; set; }
    }
}