using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.API.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Summarisation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionReturnsController : ControllerBase
    {
        // GET: api/CollectionReturns
        [HttpGet]
        public IEnumerable<CollectionReturnSummaryDto> Get(DateTime? collectionsClosedSince = null, int pageNumber = 1, int pageSize = 100)
        {
            return new List<CollectionReturnSummaryDto>()
            {
                new CollectionReturnSummaryDto()
                {
                    CollectionReturnCode = "R01",
                    CollectionType = "ILR",
                    DateTime = new DateTime(2017, 1, 2)
                },
                new CollectionReturnSummaryDto()
                {
                    CollectionReturnCode = "R02",
                    CollectionType = "ILR",
                    DateTime = new DateTime(2017, 2, 3),
                },
                new CollectionReturnSummaryDto()
                {
                    CollectionReturnCode = "N01",
                    CollectionType = "NCS",
                    DateTime = new DateTime(2018, 1, 1)
                }
            };
        }

        // GET: api/CollectionReturns/ILR/R01
        [HttpGet("{collectionType}", Name = "GetCollectionReturnsByType")]
        public IEnumerable<CollectionReturnSummaryDto> Get(string collectionType, DateTime? collectionsClosedSince = null, int pageNumber = 1, int pageSize = 100)
        {
            return new List<CollectionReturnSummaryDto>()
            {
                new CollectionReturnSummaryDto()
                {
                    CollectionReturnCode = "R01",
                    CollectionType = "ILR",
                    DateTime = new DateTime(2017, 1, 2)
                },
                new CollectionReturnSummaryDto()
                {
                    CollectionReturnCode = "R02",
                    CollectionType = "ILR",
                    DateTime = new DateTime(2017, 2, 3),
                }
            };
        }

        // GET: api/CollectionReturns/ILR/R01
        [HttpGet("{collectionType}/{collectionReturnCode}", Name = "GetCollectionReturnByReturnCode")]
        public CollectionReturnDto Get(string collectionType, string collectionReturnCode, int pageNumber = 1, int pageSize = 100)
        {
            return new CollectionReturnDto()
            {
                CollectionReturnCode = "R01",
                CollectionType = "ILR",
                DateTime = new DateTime(2017, 1, 2),
                SummarisedActuals = new List<SummarisedActualDto>()
                {
                    new SummarisedActualDto() { ActualValue = 1.1m, ActualVolume = 1, ContractAllocationNumber = "ContractAllocationNumber1", DeliverableCode = 1, FundingStreamPeriodCode = "FundingStreamPeriodCode1", OrganisationId = "OrganidationId1", Period = 1, PeriodTypeCode = "PeriodTypeCode1", UoPCode = "UoPCode1"},
                    new SummarisedActualDto() { ActualValue = 1.2m, ActualVolume = 2, ContractAllocationNumber = "ContractAllocationNumber2", DeliverableCode = 2, FundingStreamPeriodCode = "FundingStreamPeriodCode2", OrganisationId = "OrganidationId2", Period = 2, PeriodTypeCode = "PeriodTypeCode2", UoPCode = "UoPCode2"},
                    new SummarisedActualDto() { ActualValue = 1.3m, ActualVolume = 3, ContractAllocationNumber = "ContractAllocationNumber3", DeliverableCode = 3, FundingStreamPeriodCode = "FundingStreamPeriodCode3", OrganisationId = "OrganidationId3", Period = 3, PeriodTypeCode = "PeriodTypeCode3", UoPCode = "UoPCode3"},
                    new SummarisedActualDto() { ActualValue = 1.4m, ActualVolume = 4, ContractAllocationNumber = "ContractAllocationNumber4", DeliverableCode = 4, FundingStreamPeriodCode = "FundingStreamPeriodCode4", OrganisationId = "OrganidationId4", Period = 4, PeriodTypeCode = "PeriodTypeCode4", UoPCode = "UoPCode4"},
                    new SummarisedActualDto() { ActualValue = 1.5m, ActualVolume = 5, ContractAllocationNumber = "ContractAllocationNumber5", DeliverableCode = 5, FundingStreamPeriodCode = "FundingStreamPeriodCode5", OrganisationId = "OrganidationId5", Period = 5, PeriodTypeCode = "PeriodTypeCode5", UoPCode = "UoPCode5"},
                    new SummarisedActualDto() { ActualValue = 1.6m, ActualVolume = 6, ContractAllocationNumber = "ContractAllocationNumber6", DeliverableCode = 6, FundingStreamPeriodCode = "FundingStreamPeriodCode6", OrganisationId = "OrganidationId6", Period = 6, PeriodTypeCode = "PeriodTypeCode6", UoPCode = "UoPCode6"}
                }
            };
        }
    }
}
