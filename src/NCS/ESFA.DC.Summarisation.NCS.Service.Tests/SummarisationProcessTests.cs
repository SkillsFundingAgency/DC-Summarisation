using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.NCS.Interfaces;
using ESFA.DC.Summarisation.NCS.Model;
using ESFA.DC.Summarisation.NCS.Model.Config;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.Summarisation.NCS.Service.Tests
{
    public class SummarisationProcessTests
    {
        [Fact]
        public async Task CollateAndSummariseAsyncTest()
        {
            var cancellationToken = CancellationToken.None;

            var summarisationTypes = new List<string> { "NCS1920_C" };
            var processType = "NCS";
            var collectionType = "NCS1920";
            var collectionYear = 1920;

            var collectionPeriods = Array.Empty<CollectionPeriod>();

            var summarisationMessageMock = new Mock<ISummarisationMessage>();
            summarisationMessageMock.Setup(sm => sm.SummarisationTypes).Returns(summarisationTypes);
            summarisationMessageMock.Setup(sm => sm.ProcessType).Returns(processType);
            summarisationMessageMock.Setup(sm => sm.CollectionType).Returns(collectionType);
            summarisationMessageMock.Setup(sm => sm.CollectionYear).Returns(collectionYear);

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

            var providerSummarisationServiceMock = new Mock<IProviderSummarisationService<TouchpointProviderFundingData>>();
            providerSummarisationServiceMock
                .Setup(x => x.Summarise(It.IsAny<TouchpointProviderFundingData>(),
                                        collectionPeriods,
                                        fundingTypes,
                                        It.IsAny<ICollection<FcsContractAllocation>>(),
                                        summarisationMessageMock.Object,
                                        cancellationToken))
                .ReturnsAsync(TestSummarisedActuals());

            var dataRetrievalMaxConcurrentCalls = "4";
            var dataOptions = new Mock<ISummarisationDataOptions>();
            dataOptions.SetupGet(x => x.DataRetrievalMaxConcurrentCalls).Returns(dataRetrievalMaxConcurrentCalls); 

            var repositoryFactory = new Mock<IInputDataRepository<TouchpointProviderFundingData>>();

            repositoryFactory
                    .Setup(x => x.GetAllIdentifiersAsync(collectionYear, cancellationToken))
                    .ReturnsAsync(TouchpointProviders());

            repositoryFactory
                    .Setup(x => x.ProvideAsync(It.IsAny<TouchpointProvider>(), summarisationMessageMock.Object, cancellationToken))
                    .ReturnsAsync(TestTouchpointProviderFundingData());

            var result = await NewService(
              fcsRepositoryMock.Object,
              collectionPeriodsProviders,
              fundingTypesProviders,
              () => repositoryFactory.Object,
              dataOptions.Object,
              null,
              providerSummarisationServiceMock.Object).CollateAndSummariseAsync(summarisationMessageMock.Object, cancellationToken);

            var testAcutals = TestSummarisedActuals();

            result.Should().BeEquivalentTo(testAcutals);
        }

        private static SummarisationProcess NewService(
            IFcsRepository fcsRepository = null,
            IEnumerable<ISummarisationConfigProvider<CollectionPeriod>> collectionPeriodsProviders = null,
            IEnumerable<ISummarisationConfigProvider<FundingType>> fundingTypesProviders = null,
            Func<IInputDataRepository<TouchpointProviderFundingData>> repositoryFactory = null,
            ISummarisationDataOptions dataOptions = null,
            ILogger logger = null,
            IProviderSummarisationService<TouchpointProviderFundingData> providerSummarisationService = null)
        {
            return new SummarisationProcess(
                fcsRepository ?? Mock.Of<IFcsRepository>(),
                collectionPeriodsProviders ?? Mock.Of<IEnumerable<ISummarisationConfigProvider<CollectionPeriod>>>(),
                fundingTypesProviders ?? Mock.Of<IEnumerable<ISummarisationConfigProvider<FundingType>>>(),
                repositoryFactory ?? Mock.Of<Func<IInputDataRepository<TouchpointProviderFundingData>>>(),
                dataOptions ?? Mock.Of<ISummarisationDataOptions>(),
                logger ?? Mock.Of<ILogger>(),
                providerSummarisationService ?? Mock.Of<IProviderSummarisationService<TouchpointProviderFundingData>>());
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

        private static ICollection<TouchpointProvider> TouchpointProviders()
        {
            return new List<TouchpointProvider>
            {
                new TouchpointProvider{ TouchpointId = "101", UKPRN = 101},
            };
        }

        private static TouchpointProviderFundingData TestTouchpointProviderFundingData()
        {
            return new TouchpointProviderFundingData
            {
                Provider = new TouchpointProvider { TouchpointId = "101", UKPRN = 101 },
                FundingValues = new List<FundingValue> { new FundingValue() { CalendarMonth= 4, CollectionYear = 1920, OutcomeType = 1, Value = 200  } }
            };
        }


    }
}
