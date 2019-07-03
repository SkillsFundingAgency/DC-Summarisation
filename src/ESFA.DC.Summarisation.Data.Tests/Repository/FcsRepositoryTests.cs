using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Summarisation.Data.Population.Service;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ESFA.DC.Summarisation.Data.Tests.Repository
{
    public class FcsRepositoryTests
    {
        [Fact]
        public async Task RetrieveAsyncTest()
        {
            var allocations = new List<ContractAllocation>
            {
                new ContractAllocation
                {
                    ContractAllocationNumber = "100",
                    FundingStreamPeriodCode = "PeriodCode1",
                    UoPcode = "UoPcode1",
                    DeliveryUkprn = 1,
                    DeliveryOrganisation = "Org001"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "101",
                    FundingStreamPeriodCode = "PeriodCode2",
                    UoPcode = "UoPcode1",
                    DeliveryUkprn = 1,
                    DeliveryOrganisation = "Org001"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "103",
                    FundingStreamPeriodCode = "PeriodCode3",
                    UoPcode = "UoPcode1",
                    DeliveryUkprn = 1,
                    DeliveryOrganisation = "Org001"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "104",
                    FundingStreamPeriodCode = "PeriodCode4",
                    UoPcode = "UoPcode1",
                    DeliveryUkprn = 1,
                    DeliveryOrganisation = "Org001"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "100",
                    FundingStreamPeriodCode = "PeriodCode1",
                    UoPcode = "UoPcode1",
                    DeliveryUkprn = 2,
                    DeliveryOrganisation = "Org002"
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "101",
                    FundingStreamPeriodCode = "PeriodCode2",
                    UoPcode = "UoPcode1",
                    DeliveryUkprn = 2,
                    DeliveryOrganisation = "Org002"
                }
            }.AsQueryable().BuildMock();

            var fcsMock = new Mock<IFcsContext>();
            fcsMock
                .Setup(f => f.ContractAllocations)
                .Returns(allocations.Object);

            var service = new FcsRepository(() => fcsMock.Object);

            var fcsa = await service.RetrieveAsync(CancellationToken.None);

            fcsa["PeriodCode1"].Count.Should().Be(2);
            fcsa["PeriodCode2"].Count.Should().Be(2);
            fcsa["PeriodCode3"].Count.Should().Be(1);
            fcsa["PeriodCode4"].Count.Should().Be(1);
        }
    }
}
