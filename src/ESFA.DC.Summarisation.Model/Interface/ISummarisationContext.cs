using System.Linq;

namespace ESFA.DC.Summarisation.Model.Interface
{
    public interface ISummarisationContext
    {
        IQueryable<SummarisedActual> SummarisedActuals { get; }

        IQueryable<CollectionReturn> CollectionReturns { get; }
    }
}
