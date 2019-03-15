using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist.Mapper.Interface
{
    public interface ICollectionReturnMapper
    {
        CollectionReturn MapCollectionReturn(ISummarisationMessage summarisationMessage);
    }
}