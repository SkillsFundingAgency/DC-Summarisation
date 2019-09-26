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
                    FundingStreamPeriodCode = "APPS1819",
                    UoPcode = "UoPcode1",
                    DeliveryUkprn = 1,
                    DeliveryOrganisation = "Org001",
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "101",
                    FundingStreamPeriodCode = "APPS1920",
                    UoPcode = "UoPcode1",
                    DeliveryUkprn = 1,
                    DeliveryOrganisation = "Org001",
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "103",
                    FundingStreamPeriodCode = "ESF1420",
                    UoPcode = "UoPcode1",
                    DeliveryUkprn = 1,
                    DeliveryOrganisation = "Org001",
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "104",
                    FundingStreamPeriodCode = "LEVY1799",
                    UoPcode = "UoPcode1",
                    DeliveryUkprn = 1,
                    DeliveryOrganisation = "Org001",
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "100",
                    FundingStreamPeriodCode = "APPS1819",
                    UoPcode = "UoPcode1",
                    DeliveryUkprn = 2,
                    DeliveryOrganisation = "Org002",
                },
                new ContractAllocation
                {
                    ContractAllocationNumber = "101",
                    FundingStreamPeriodCode = "APPS1920",
                    UoPcode = "UoPcode1",
                    DeliveryUkprn = 2,
                    DeliveryOrganisation = "Org002",
                },
            }.AsQueryable().BuildMock();

            var fcsMock = new Mock<IFcsContext>();
            fcsMock
                .Setup(f => f.ContractAllocations)
                .Returns(allocations.Object);

            var service = new FcsRepository(() => fcsMock.Object);

            var fcsa = await service.RetrieveAsync(CancellationToken.None);

            fcsa["APPS1819"].Count.Should().Be(2);
            fcsa["APPS1920"].Count.Should().Be(2);
            fcsa["ESF1420"].Count.Should().Be(1);
            fcsa["LEVY1799"].Count.Should().Be(1);
        }
    }
}
