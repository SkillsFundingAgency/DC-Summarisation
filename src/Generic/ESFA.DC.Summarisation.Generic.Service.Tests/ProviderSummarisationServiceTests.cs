using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Generic.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Summarisation.Generic.Service.Tests
{
    public class ProviderSummarisationServiceTests
    {
        [Fact]
        public async Task Summarise()
        {
            var cancellationToken = CancellationToken.None;
            var summarisationMessage = Mock.Of<ISummarisationMessage>();
            var inputActuals = TestSummarisedActuals();
            var contractAllocations = new List<FcsContractAllocation>
            {
                new FcsContractAllocation
                {
                    DeliveryUkprn = 1,
                    DeliveryOrganisation = "Org1",
                    FundingStreamPeriodCode = "FSPCode1",
                },
                new FcsContractAllocation
                {
                    DeliveryUkprn = 2,
                    DeliveryOrganisation = "Org2",
                    FundingStreamPeriodCode = "FSPCode2",
                },
                new FcsContractAllocation
                {
                    DeliveryUkprn = 3,
                    DeliveryOrganisation = "Org3",
                    FundingStreamPeriodCode = "FSPCode3",
                },
            };

            var mockReturnOne = inputActuals.Take(2).ToList();
            var mockReturnTwo = inputActuals.Skip(2).Take(1).ToList();

            var summarisationServiceMock = new Mock<ISummarisationService>();
            summarisationServiceMock
                .Setup(x => x.Summarise(contractAllocations, It.IsAny<ICollection<SummarisedActual>>()))
                .Returns(inputActuals);

            var providerFundingDataRemovedServiceMock = new Mock<IProviderFundingDataRemovedService>();
            providerFundingDataRemovedServiceMock
                .Setup(x => x.FundingDataRemovedAsync(It.IsAny<string>(), It.IsAny<ICollection<SummarisedActual>>(), summarisationMessage, cancellationToken))
                .ReturnsAsync(new List<SummarisedActual>());

            var result = await NewService(summarisationServiceMock.Object, providerFundingDataRemovedServiceMock.Object).Summarise(inputActuals, summarisationMessage, contractAllocations, cancellationToken);

            result.Should().BeEquivalentTo(inputActuals);

            providerFundingDataRemovedServiceMock.VerifyAll();
        }

        private static ICollection<SummarisedActual> TestSummarisedActuals()
        {
            return new List<SummarisedActual>
            {
                 new SummarisedActual
                 {
                     CollectionReturnId = 101,
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
                     CollectionReturnId = 101,
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
                     CollectionReturnId = 101,
                     ContractAllocationNumber = "Contract2",
                     FundingStreamPeriodCode = "FSPCode2",
                     DeliverableCode = 2,
                     Period = 2,
                     PeriodTypeCode = "PType2",
                     OrganisationId = "Org2",
                     ActualValue = 2,
                     ActualVolume = 2,
                 },
            };
        }

        private static ProviderSummarisationService NewService(ISummarisationService summarisationService, IProviderFundingDataRemovedService providerFundingDataRemovedService = null)
        {
            return new ProviderSummarisationService(
                Mock.Of<ILogger>(),
                summarisationService ?? Mock.Of<ISummarisationService>(),
                providerFundingDataRemovedService ?? Mock.Of<IProviderFundingDataRemovedService>());
        }
    }
}
