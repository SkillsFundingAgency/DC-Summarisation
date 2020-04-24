using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.ESF.Interfaces;
using ESFA.DC.Summarisation.ESF.Model;
using ESFA.DC.Summarisation.ESF.Model.Config;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.Summarisation.ESF.Service.Tests
{
    public class SummarisationProcessTests
    {
        [Fact]
        public async Task CollateAndSummariseAsyncTest()
        {
            var cancellationToken = CancellationToken.None;

            var summarisationTypes = new List<string> { "ESF_ILRData", "ESF_SuppData", "ESF_ILR_And_Supp" };
            var processType = "Deliverable";
            var collectionType = "ESF";

            var collectionPeriods = Array.Empty<CollectionPeriod>();

            var summarisationMessageMock = new Mock<ISummarisationMessage>();
            summarisationMessageMock.Setup(sm => sm.SummarisationTypes).Returns(summarisationTypes);
            summarisationMessageMock.Setup(sm => sm.ProcessType).Returns(processType);
            summarisationMessageMock.Setup(sm => sm.CollectionType).Returns(collectionType);

            var collectionPeriodProviderMock = new Mock<ISummarisationConfigProvider<CollectionPeriod>>();

            collectionPeriodProviderMock.SetupGet(p => p.CollectionType).Returns(collectionType);
            collectionPeriodProviderMock.Setup(p => p.Provide()).Returns(collectionPeriods);

            var fundingTypes = Array.Empty<FundingType>();
            var fundingTypesProviderMock = new Mock<ISummarisationConfigProvider<FundingType>>();
            fundingTypesProviderMock.SetupGet(p => p.CollectionType).Returns(collectionType);
            fundingTypesProviderMock.Setup(p => p.Provide()).Returns(fundingTypes);

            var collectionPeriodsProviders = new List<ISummarisationConfigProvider<CollectionPeriod>>()
           {
                collectionPeriodProviderMock.Object
           };

            var fundingTypesProviders = new List<ISummarisationConfigProvider<FundingType>>()
           {
                fundingTypesProviderMock.Object
           };

            var contractAllocations = new List<FcsContractAllocation>();
            var fcsRepositoryMock = new Mock<IFcsRepository>();
            fcsRepositoryMock
                .Setup(x => x.RetrieveContractAllocationsAsync(It.IsAny<IEnumerable<string>>(), cancellationToken))
                .ReturnsAsync(contractAllocations);

            var providerSummarisationServiceMock = new Mock<IProviderSummarisationService<LearningProvider>>();
            providerSummarisationServiceMock
                .Setup(x => x.Summarise(It.IsAny<LearningProvider>(),
                                        collectionPeriods,
                                        fundingTypes,
                                        It.IsAny<ICollection<FcsContractAllocation>>(),
                                        summarisationMessageMock.Object,
                                        cancellationToken))
                .ReturnsAsync(TestSummarisedActuals());

            var dataRetrievalMaxConcurrentCalls = "4";
            var dataOptions = new Mock<ISummarisationDataOptions>();
            dataOptions.SetupGet(x => x.DataRetrievalMaxConcurrentCalls).Returns(dataRetrievalMaxConcurrentCalls);

            var repositoryFactory = new Mock<IInputDataRepository<LearningProvider>>();

            repositoryFactory
                    .Setup(x => x.GetAllIdentifiersAsync(It.IsAny<string>(), cancellationToken))
                    .ReturnsAsync(TestESFProviders());

            repositoryFactory
                    .Setup(x => x.ProvideAsync(It.IsAny<int>(), summarisationMessageMock.Object, cancellationToken))
                    .ReturnsAsync(TestProvider());

            var testAcutals = TestSummarisedActuals();

            var carryActuals = Array.Empty<SummarisedActual>();

            var fundingDataRemoved = new Mock<IFundingDataRemovedService>();
            fundingDataRemoved.Setup(x => x.FundingDataRemovedAsync(testAcutals, summarisationMessageMock.Object, cancellationToken))
                .ReturnsAsync(carryActuals);

            var result = await NewService(
              fcsRepositoryMock.Object,
              collectionPeriodsProviders,
              fundingTypesProviders,
              () => repositoryFactory.Object,
              dataOptions.Object,
              null,
              providerSummarisationServiceMock.Object, 
              fundingDataRemoved.Object).CollateAndSummariseAsync(summarisationMessageMock.Object, cancellationToken);


            result.Should().BeEquivalentTo(testAcutals);
        }

        private SummarisationProcess NewService(
                    IFcsRepository fcsRepository = null,
                    IEnumerable<ISummarisationConfigProvider<CollectionPeriod>> collectionPeriodsProviders = null,
                    IEnumerable<ISummarisationConfigProvider<FundingType>> fundingTypesProviders = null,
                    Func<IInputDataRepository<LearningProvider>> repositoryFactory = null,
                    ISummarisationDataOptions dataOptions = null,
                    ILogger logger = null,
                    IProviderSummarisationService<LearningProvider> providerSummarisationService = null,
                    IFundingDataRemovedService fundingDataRemovedService = null)
        {
            return new SummarisationProcess(
                fcsRepository ?? Mock.Of<IFcsRepository>(),
                collectionPeriodsProviders ?? Mock.Of<IEnumerable<ISummarisationConfigProvider<CollectionPeriod>>>(),
                fundingTypesProviders ?? Mock.Of<IEnumerable<ISummarisationConfigProvider<FundingType>>>(),
                repositoryFactory ?? Mock.Of<Func<IInputDataRepository<LearningProvider>>>(),
                dataOptions ?? Mock.Of<ISummarisationDataOptions>(),
                logger ?? Mock.Of<ILogger>(),
                providerSummarisationService ?? Mock.Of<IProviderSummarisationService<LearningProvider>>(),
                fundingDataRemovedService ?? Mock.Of<IFundingDataRemovedService>());
        }

        private ICollection<int> TestESFProviders()
        {
            return new List<int>
            {
               101 
            };
        }

        private LearningProvider TestProvider()
        {
            return new LearningProvider()
            {
                UKPRN = 101,
                LearningDeliveries = new List<LearningDelivery>()
                {
                    new LearningDelivery(){ ConRefNumber = "ConRef-1"},
                    new LearningDelivery(){ ConRefNumber = "ConRef-2"}
                }
            };
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
    }
}
