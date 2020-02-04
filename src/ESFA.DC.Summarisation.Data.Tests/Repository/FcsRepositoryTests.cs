using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Data.Repository;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace ESFA.DC.Summarisation.Data.Tests.Repository
{
    public class FcsRepositoryTests
    {
        [Fact]
        public async Task RetrieveContractAllocationsAsync()
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

            var fcsa = await service.RetrieveContractAllocationsAsync(FundingStreamConstants.FundingStreams, CancellationToken.None);

            fcsa.Count(a => a.FundingStreamPeriodCode.Equals("APPS1819", System.StringComparison.OrdinalIgnoreCase)).Should().Be(2);
            fcsa.Count(a => a.FundingStreamPeriodCode.Equals("APPS1920", System.StringComparison.OrdinalIgnoreCase)).Should().Be(2);
            fcsa.Count(a => a.FundingStreamPeriodCode.Equals("ESF1420", System.StringComparison.OrdinalIgnoreCase)).Should().Be(1);
            fcsa.Count(a => a.FundingStreamPeriodCode.Equals("LEVY1799", System.StringComparison.OrdinalIgnoreCase)).Should().Be(1);
        }
    }
}
