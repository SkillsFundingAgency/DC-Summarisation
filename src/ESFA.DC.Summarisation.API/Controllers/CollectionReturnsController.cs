using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.DTO;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Summarisation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionReturnsController : ControllerBase
    {
        private readonly ISummarisedActualsRepository _summarisedActualsRepository;

        public CollectionReturnsController(ISummarisedActualsRepository summarisedActualsRepository)
        {
            _summarisedActualsRepository = summarisedActualsRepository;
        }

        // GET: api/CollectionReturns
        [HttpGet]
        public async Task<IEnumerable<CollectionReturnSummaryDto>> GetAsync(CancellationToken cancellationToken, DateTime? collectionsClosedSince = null, int pageNumber = 1, int pageSize = 100)
        {
            return await _summarisedActualsRepository.GetCollectionReturnSummariesForAsync(collectionsClosedSince, pageNumber, pageSize, cancellationToken);
        }

        // GET: api/CollectionReturns/ILR/R01
        [HttpGet("{collectionType}", Name = "GetCollectionReturnsByType")]
        public async Task<IEnumerable<CollectionReturnSummaryDto>> GetAsync(CancellationToken cancellationToken, string collectionType, DateTime? collectionsClosedSince = null, int pageNumber = 1, int pageSize = 100)
        {
            return await _summarisedActualsRepository.GetCollectionReturnSummariesForAsync(collectionType, collectionsClosedSince, pageNumber, pageSize, cancellationToken);
        }

        // GET: api/CollectionReturns/ILR/R01
        [HttpGet("{collectionType}/{collectionReturnCode}", Name = "GetCollectionReturnByReturnCode")]
        public async Task<CollectionReturnDto> GetAsync(CancellationToken cancellationToken, string collectionType, string collectionReturn, int pageNumber = 1, int pageSize = 100)
        {
            return await _summarisedActualsRepository.GetCollectionReturnFor(collectionType, collectionReturn, pageNumber, pageSize, cancellationToken);
        }
    }
}
