using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist.Mapper.Interface
{
    public interface ICollectionReturnMapper
    {
        CollectionReturn MapCollectionReturn(Output.Model.CollectionReturn collectionReturn);
    }
}