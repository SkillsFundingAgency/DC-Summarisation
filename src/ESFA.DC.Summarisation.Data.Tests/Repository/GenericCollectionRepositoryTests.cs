using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.GenericCollection.EF;
using ESFA.DC.GenericCollection.EF.Interface;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Data.Repository;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ESFA.DC.Summarisation.Data.Tests.Repository
{
    public class GenericCollectionRepositoryTests
    {
        [Fact]
        public async Task RetrieveAsync()
        {
            var gcSummarisedActuals = new List<GenericCollection.EF.SummarisedActual>
            {
                new SummarisedActual
                {
                    Id = 1,
                    CollectionReturnCode = "A01",
                    CollectionType = "ALLF",
                    ContractAllocationNumber = "Contract1",
                    FundingStreamPeriodCode = "FSPCode1",
                    DeliverableCode = 1,
                    Period = 1,
                    PeriodTypeCode = "PType1",
                    OrganisationId = "Org1",
                    ActualValue = 1,
                    ActualVolume = 1,
                },
                new SummarisedActual
                {
                    Id = 2,
                    CollectionReturnCode = "A02",
                    CollectionType = "ALLF",
                    ContractAllocationNumber = "Contract2",
                    FundingStreamPeriodCode = "FSPCode2",
                    DeliverableCode = 2,
                    Period = 2,
                    PeriodTypeCode = "PType2",
                    OrganisationId = "Org2",
                    ActualValue = 2,
                    ActualVolume = 2,
                },
                new SummarisedActual
                {
                    Id = 3,
                    CollectionReturnCode = "A02",
                    CollectionType = "NOTALLF",
                    ContractAllocationNumber = "Contract2",
                    FundingStreamPeriodCode = "FSPCode2",
                    DeliverableCode = 2,
                    Period = 2,
                    PeriodTypeCode = "PType2",
                    OrganisationId = "Org3",
                    ActualValue = 2,
                    ActualVolume = 2,
                },
            }.AsQueryable().BuildMockDbSet();

            var efContextMock = new Mock<IGenericCollectionContext>();
            efContextMock
                .Setup(f => f.SummarisedActuals)
                .Returns(gcSummarisedActuals.Object);

            var service = new GenericCollectionRepository(() => efContextMock.Object);

            var result = await service.RetrieveAsync(CollectionTypeConstants.ALLF, CancellationToken.None);

            result.Should().HaveCount(2);
            result.Select(x => x.OrganisationId).Should().Contain("Org1", "Org2");
            result.Select(x => x.OrganisationId).Should().NotContain("Org3");
        }
    }
}
