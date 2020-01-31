using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Generic.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Summarisation.Generic.Service.Tests
{
    public class SummarisationProcessTests
    {
        [Fact]
        public async Task Summarise()
        {
            var cancellationToken = CancellationToken.None;
            var collectionType = "ALLF";
            var ukprns = new List<int> { 1, 2 };
            var inputActuals = TestSummarisedActuals();
            var contractors = new List<FcsContractor>
            {
                new FcsContractor
                {
                    Ukprn = 1,
                    OrganisationIdentifier = "Org1",
                },
                new FcsContractor
                {
                    Ukprn = 2,
                    OrganisationIdentifier = "Org2",
                },
                new FcsContractor
                {
                    Ukprn = 3,
                    OrganisationIdentifier = "Org3",
                },
            };

            var summarisationMessageMock = new Mock<ISummarisationMessage>();
            summarisationMessageMock.Setup(sm => sm.CollectionType).Returns(collectionType);

            var genericCollectionRepositoryMock = new Mock<IGenericCollectionRepository>();
            genericCollectionRepositoryMock.Setup(x => x.RetrieveAsync(collectionType, cancellationToken)).ReturnsAsync(inputActuals);

            var fcsMock = new Mock<IFcsRepository>();
            fcsMock.Setup(x => x.RetrieveContractorForUkprnAsync(ukprns, cancellationToken)).ReturnsAsync(contractors);

            var providerSummarisationServiceMock = new Mock<IProviderSummarisationService<IEnumerable<SummarisedActual>>>();
            providerSummarisationServiceMock
                .Setup(x => x.Summarise(It.IsAny<ICollection<SummarisedActual>>(), summarisationMessageMock.Object, contractors, cancellationToken))
                .ReturnsAsync(inputActuals);

            var result = await NewService(
                genericCollectionRepositoryMock.Object,
                providerSummarisationServiceMock.Object,
                fcsRepository: fcsMock.Object).CollateAndSummariseAsync(summarisationMessageMock.Object, cancellationToken);

            result.Should().BeEquivalentTo(inputActuals);

            genericCollectionRepositoryMock.VerifyAll();
            providerSummarisationServiceMock.VerifyAll();
        }

        private ICollection<SummarisedActual> TestSummarisedActuals()
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
                     OrganisationId = "1",
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
                     OrganisationId = "1",
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
                     OrganisationId = "2",
                     ActualValue = 2,
                     ActualVolume = 2,
                 },
            };
        }

        private SummarisationProcess NewService(
            IGenericCollectionRepository genericCollectionRepository = null,
            IProviderSummarisationService<IEnumerable<SummarisedActual>> providerSummarisationService = null,
            ISummarisationDataOptions dataOptions = null,
            IFcsRepository fcsRepository = null,
            ILogger logger = null)
        {
            return new SummarisationProcess(
                genericCollectionRepository ?? Mock.Of<IGenericCollectionRepository>(),
                providerSummarisationService ?? Mock.Of<IProviderSummarisationService<IEnumerable<SummarisedActual>>>(),
                dataOptions ?? Mock.Of<ISummarisationDataOptions>(),
                fcsRepository ?? Mock.Of<IFcsRepository>(),
                logger ?? Mock.Of<ILogger>());
        }
    }
}
