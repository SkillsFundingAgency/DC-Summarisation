using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Apps.Interfaces;
using ESFA.DC.Summarisation.Apps.Model;
using ESFA.DC.Summarisation.Apps.Model.Config;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace ESFA.DC.Summarisation.Apps.Service.Tests
{
    public class ProviderSummarisationServiceTests
    {
        [Fact]
        public async void SummariseTest()
        {
            var cancellationToken = CancellationToken.None;

            var summarisationTypes = new List<string> { "Apps1920_Levy" };
            var processType = "Payments";
            var collectionType = "APPS";

            var collectionPeriods = Array.Empty<CollectionPeriod>();
            var fundingTypes = Array.Empty<FundingType>();
            var fcsAllocations = Array.Empty<FcsContractAllocation>();

            var summarisationMessageMock = new Mock<ISummarisationMessage>();
            summarisationMessageMock.Setup(sm => sm.SummarisationTypes).Returns(summarisationTypes);
            summarisationMessageMock.Setup(sm => sm.ProcessType).Returns(processType);

            var providerContractsServiceMock = new Mock<IProviderContractsService>();

            providerContractsServiceMock
               .Setup(x => x.GetProviderContracts(It.IsAny<int>(), It.IsAny<ICollection<FundingStream>>(), It.IsAny<ICollection<FcsContractAllocation>>(), cancellationToken))
               .ReturnsAsync(TestProviderFundingStreamsAllocations);

            var summarisationDeliverableProcessMock = new Mock<ISummarisationService>();

            var testActuals = TestSummarisedActuals();

            summarisationDeliverableProcessMock
                .Setup(x => x.Summarise(It.IsAny<ICollection<FundingStream>>(), It.IsAny<LearningProvider>(), It.IsAny<ICollection<FcsContractAllocation>>(), It.IsAny<ICollection<CollectionPeriod>>(), summarisationMessageMock.Object))
                .Returns(testActuals);

            var providerFundingDataRemovedServiceMock = new Mock<IProviderFundingDataRemovedService>();

            var fundingDataremoved = TestFundingDataRemoved();
           
            providerFundingDataRemovedServiceMock
                .Setup(x => x.FundingDataRemovedAsync(It.IsAny<string>(), It.IsAny<List<SummarisedActual>>(), summarisationMessageMock.Object, cancellationToken))
                .ReturnsAsync(fundingDataremoved);

            var providerSummarisationServiceMock = new Mock<IProviderSummarisationService<LearningProvider>>();

            var result = await NewService(
               summarisationDeliverableProcessMock.Object,
               null,
               providerContractsServiceMock.Object,
               providerFundingDataRemovedServiceMock.Object).Summarise(TestESFProvider(), collectionPeriods, fundingTypes, fcsAllocations, summarisationMessageMock.Object, cancellationToken);

            testActuals.AddRange(fundingDataremoved);

            result.Should().BeEquivalentTo(testActuals);

        }

        [Fact]
        public async void Summarise_BR05_FundingData_RemovedRule_ApplicableTo_ACT2_Only()
        {
            var cancellationToken = CancellationToken.None;
            var summarisationTypes = new List<string>();
            var collectionPeriods = Array.Empty<CollectionPeriod>();
            var fundingTypes = Array.Empty<FundingType>();
            var fcsAllocations = Array.Empty<FcsContractAllocation>();
            var summarisationMessageMock = new Mock<ISummarisationMessage>();
            summarisationMessageMock.Setup(sm => sm.SummarisationTypes).Returns(summarisationTypes);
           
            var providerContractsServiceMock = new Mock<IProviderContractsService>();
            var summarisationDeliverableProcessMock = new Mock<ISummarisationService>();
            var providerFundingDataRemovedServiceMock = new Mock<IProviderFundingDataRemovedService>();

            var fundingDataremoved = TestFundingDataRemoved();
            var act1FundingDataToBeRemoved = TestACT1FundingDataToBeRemoved();
            fundingDataremoved.AddRange(act1FundingDataToBeRemoved);

            providerFundingDataRemovedServiceMock
                .Setup(x => x.FundingDataRemovedAsync(It.IsAny<string>(), It.IsAny<List<SummarisedActual>>(), summarisationMessageMock.Object, cancellationToken))
                .ReturnsAsync(fundingDataremoved);

            var result = await NewService(
               summarisationDeliverableProcessMock.Object,
               null,
               providerContractsServiceMock.Object,
               providerFundingDataRemovedServiceMock.Object).Summarise(TestESFProvider(), collectionPeriods, fundingTypes, fcsAllocations, summarisationMessageMock.Object, cancellationToken);

            result.Should().BeEquivalentTo(fundingDataremoved.Except(act1FundingDataToBeRemoved));
        }

        private static ProviderSummarisationService NewService(
           ISummarisationService summarisationService = null,
            ILogger logger = null,
            IProviderContractsService providerContractsService = null,
            IProviderFundingDataRemovedService providerFundingDataRemovedService = null)
        {
            return new ProviderSummarisationService(
                summarisationService ?? Mock.Of<ISummarisationService>(),
                logger ?? Mock.Of<ILogger>(),
                providerContractsService ?? Mock.Of<IProviderContractsService>(),
                providerFundingDataRemovedService ?? Mock.Of<IProviderFundingDataRemovedService>());
        }

        private ProviderFundingStreamsAllocations TestProviderFundingStreamsAllocations()
        {
            return new ProviderFundingStreamsAllocations
            {
                FundingStreams = new List<FundingStream> { },
                FcsContractAllocations = new List<FcsContractAllocation> { }
            };
        }

        private static LearningProvider TestESFProvider()
        {
            return new LearningProvider() { UKPRN = 101, LearningDeliveries = new List<LearningDelivery>() { } };
        }

        private static List<SummarisedActual> TestSummarisedActuals()
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

        private static List<SummarisedActual> TestFundingDataRemoved()
        {
            return new List<SummarisedActual>
            {
                 new SummarisedActual
                 {
                     CollectionReturnId = 101,
                     ContractAllocationNumber = "Contract4",
                     FundingStreamPeriodCode = "FSPCode4",
                     DeliverableCode = 1,
                     Period = 1,
                     PeriodTypeCode = "PType1",
                     OrganisationId = "Org1",
                     ActualValue = 0,
                     ActualVolume = 0,
                 },
                 new SummarisedActual
                 {
                     CollectionReturnId = 101,
                     ContractAllocationNumber = "Contract5",
                     FundingStreamPeriodCode = "FSPCode5",
                     DeliverableCode = 1,
                     Period = 1,
                     PeriodTypeCode = "PType5",
                     OrganisationId = "Org5",
                     ActualValue = 0,
                     ActualVolume = 0,
                 }
            };
        }

        private static List<SummarisedActual> TestACT1FundingDataToBeRemoved()
        {
            return new List<SummarisedActual>
            {
               new SummarisedActual
                {
                    CollectionReturnId = 101,
                    ContractAllocationNumber = "Contract4",
                    FundingStreamPeriodCode = "LEVY1799",
                    DeliverableCode = 1,
                    Period = 1,
                    PeriodTypeCode = "PType1",
                    OrganisationId = "Org1",
                    ActualValue = 0,
                    ActualVolume = 0,
                },
                new SummarisedActual
                {
                    CollectionReturnId = 101,
                    ContractAllocationNumber = "Contract4",
                    FundingStreamPeriodCode = "NONLEVY2019",
                    DeliverableCode = 1,
                    Period = 1,
                    PeriodTypeCode = "PType1",
                    OrganisationId = "Org1",
                    ActualValue = 0,
                    ActualVolume = 0,
                },
            };
        }
    }
}
