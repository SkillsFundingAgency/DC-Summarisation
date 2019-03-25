using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Data.Persist.Persist.Interface;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist.Persist
{
    public class CollectionReturnPersist : ICollectionReturnPersist
    {
        private readonly ICollectionReturnMapper _collectionReturnMapper;
        private readonly SummarisationContext _summarisationContext;

        public CollectionReturnPersist(ICollectionReturnMapper collectionReturnMapper, SummarisationContext summarisationContext)
        {
            _collectionReturnMapper = collectionReturnMapper;
            _summarisationContext = summarisationContext;
        }

        public async Task<CollectionReturn> Save(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var mappedCollectionReturn = _collectionReturnMapper.MapCollectionReturn(summarisationMessage);
            await _summarisationContext.AddAsync(mappedCollectionReturn, cancellationToken);
            await _summarisationContext.SaveChangesAsync(cancellationToken);

            return mappedCollectionReturn;
        }
    }
}