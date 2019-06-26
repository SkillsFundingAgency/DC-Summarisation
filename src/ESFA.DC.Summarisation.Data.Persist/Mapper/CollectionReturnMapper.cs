using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist.Mapper
{
    public class CollectionReturnMapper : ICollectionReturnMapper
    {
        public CollectionReturn MapCollectionReturn(ISummarisationMessage summarisationMessage)
        {
            return new CollectionReturn
            {
                CollectionReturnCode = summarisationMessage.CollectionReturnCode,
                CollectionType = summarisationMessage.CollectionType
            };
        }
    }
}
