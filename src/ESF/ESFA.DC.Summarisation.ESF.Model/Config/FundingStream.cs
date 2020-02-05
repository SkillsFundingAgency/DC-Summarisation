using System.Collections.Generic;

namespace ESFA.DC.Summarisation.ESF.Model.Config
{
    public class FundingStream
    {
        public string PeriodCode { get; set; }

        public int DeliverableLineCode { get; set; }

        public int ApprenticeshipContractType { get; set; }

        public List<int> FundingSources { get; set; }

        public List<int> TransactionTypes { get; set; }

        public FundModel FundModel { get; set; }

        public List<FundLine> FundLines { get; set; }
    }
}
