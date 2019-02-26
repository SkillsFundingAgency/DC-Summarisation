using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.DTO;
using ESFA.DC.Summarisation.Data.Repository.Interface;

namespace ESFA.DC.Summarisation.API.Stub
{
    public class SummarisedActualsRepositoryStub : ISummarisedActualsRepository
    {
        public async Task<IEnumerable<CollectionReturnSummaryDto>> GetCollectionReturnSummariesForAsync(DateTime? collectionsClosedSince, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await Task.FromResult(
                new List<CollectionReturnSummaryDto>()
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
                });
        }

        public async Task<IEnumerable<CollectionReturnSummaryDto>> GetCollectionReturnSummariesForAsync(string collectionType, DateTime? collectionsClosedSince, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await Task.FromResult(
                new List<CollectionReturnSummaryDto>()
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
                });
        }

        public async Task<CollectionReturnDto> GetCollectionReturnFor(string collectionType, string collectionReturn, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await Task.FromResult(
                new CollectionReturnDto()
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
                });
        }
    }
}
