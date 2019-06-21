using System;
using System.Linq;

namespace ESFA.DC.Summarisation.Model.Interface
{
    public interface ISummarisationContext : IDisposable
    {
        IQueryable<SummarisedActual> SummarisedActuals { get; }

        IQueryable<CollectionReturn> CollectionReturns { get; }

        IQueryable<ESF_FundingData> ESF_FundingDatas { get; }
    }
}
