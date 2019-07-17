namespace ESFA.DC.Summarisation.Data.External.FCS.Interface
{
    /// <summary>
    /// the FCS contract allocation definition
    /// </summary>
    public interface IFcsContractAllocation
    {
        /// <summary>
        /// Gets the contract allocation number.
        /// </summary>
        string ContractAllocationNumber { get; }

        /// <summary>
        /// Gets the funding stream period code.
        /// </summary>
        string FundingStreamPeriodCode { get; }

        /// <summary>
        /// Gets the UoP code.
        /// </summary>
        string UoPcode { get; }

        /// <summary>
        /// Gets the delivery ukprn.
        /// </summary>
        int? DeliveryUkprn { get; }

        /// <summary>
        /// Gets the delivery organisation.
        /// </summary>
        string DeliveryOrganisation { get; }

        /// <summary>
        /// Gets the contract start date year month.
        /// </summary>
        int ContractStartDate { get; }

        /// <summary>
        /// Gets the contract end date year month.
        /// </summary>
        int ContractEndDate { get; }
    }
}
