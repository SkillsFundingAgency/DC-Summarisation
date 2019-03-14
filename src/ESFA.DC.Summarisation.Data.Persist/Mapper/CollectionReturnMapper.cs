using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist.Mapper
{
    public class CollectionReturnMapper : ICollectionReturnMapper
    {
        public CollectionReturn MapCollectionReturn(Output.Model.CollectionReturn collectionReturn)
        {
            return new CollectionReturn
            {
                CollectionReturnCode = collectionReturn.CollectionReturnCode,
                CollectionType = collectionReturn.CollectionType,
                Id = collectionReturn.Id
            };
        }
    }
}
