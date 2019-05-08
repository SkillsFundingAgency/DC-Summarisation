using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.DTO;
using ESFA.DC.Summarisation.Data.Output.Model;

namespace ESFA.DC.Summarisation.Data.Repository.Interface
{
    public interface ISummarisedActualsRepository
    {
        Task<IEnumerable<CollectionReturnSummaryDto>> GetCollectionReturnSummariesForAsync(DateTime? collectionsClosedSince, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<CollectionReturnSummaryDto>> GetCollectionReturnSummariesForAsync(string collectionType, DateTime? collectionsClosedSince, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<CollectionReturnDto> GetCollectionReturnFor(string collectionType, string collectionReturn, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<SummarisedActual>> GetLatestSummarisedActualsAsync(string collectionType, CancellationToken cancellationToken);
    }
}
