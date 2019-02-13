using ESFA.DC.Summarisation.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface IFundingTypesProvider
    {
        IEnumerable<FundingType> Provide();
    }
}
