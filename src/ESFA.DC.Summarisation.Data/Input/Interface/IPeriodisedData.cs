using System.Collections.Generic;
using ESFA.DC.Summarisation.Data.Input.Model;

namespace ESFA.DC.Summarisation.Data.Input.Interface
{
    public interface IPeriodisedData
    {
        string AttributeName { get; }

        string DeliverableCode { get; }

        int ApprenticeshipContractType { get; }

        int FundingSource { get; }

        int TransactionType { get; }

        IList<Period> Periods { get;  }
    }
}
