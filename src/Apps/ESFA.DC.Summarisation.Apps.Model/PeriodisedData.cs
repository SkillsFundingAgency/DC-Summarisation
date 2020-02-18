using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Apps.Model
{
    public class PeriodisedData
    {
        public int ApprenticeshipContractType { get; set; }

        public int FundingSource { get; set; }

        public int TransactionType { get; set; }

        public IList<Period> Periods { get; set; }
    }
}
