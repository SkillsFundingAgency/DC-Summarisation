using System.Linq;
using ESFA.DC.Summarisation.Model.Interface;

namespace ESFA.DC.Summarisation.Model
{
    public partial class SummarisationContext : ISummarisationContext
    {
        IQueryable<SummarisedActual> ISummarisationContext.SummarisedActuals => SummarisedActuals;

        IQueryable<CollectionReturn> ISummarisationContext.CollectionReturns => CollectionReturns;
    }
}
