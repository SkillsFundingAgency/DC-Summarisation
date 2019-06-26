using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist.Mapper.Interface
{
    public interface ICollectionReturnMapper
    {
        CollectionReturn MapCollectionReturn(ISummarisationMessage summarisationContext );
    }
}