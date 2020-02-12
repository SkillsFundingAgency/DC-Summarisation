using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.External.FCS.Model;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Data.Output.Model;
using ESFA.DC.Summarisation.Data.Persist;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main1920.Service.Providers;
using ESFA.DC.Summarisation.Model;
using ESFA.DC.Summarisation.Service;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Summarisation.Main1920.Service.Tests
{
    public class SummarisationWrapperTests
    {
        [Theory]
        [InlineData(10000001, "ILR1920", "R01", "Main1920_FM35", "ILR_FM35", FundModel.FM35, "Fundline")]
        [InlineData(10000001, "ILR1920", "R01", "Main1920_EAS", "EAS", FundModel.EAS, "Fundline")]
        [InlineData(10000001, "ILR1920", "R01", "Main1920_FM25", "ILR_FM25", FundModel.FM25, "Fundline")]
        [InlineData(10000001, "ILR1920", "R01", "Main1920_ALB", "ILR_ALB", FundModel.ALB, "Fundline")]
        [InlineData(10000001, "ILR1920", "R01", "Main1920_TBL", "ILR_TBL", FundModel.TBL, "Fundline")]
        public async Task SummmariseProviders(int ukprn, string collectionType, string collectionReturnCode, string summarisationType, string lineType, FundModel fundModel, string processType)
        {
            var cancellationToken = CancellationToken.None;

            var repositoryMock = new Mock<IProviderRepository>();

            var summarisationContextMock = new Mock<ISummarisationMessage>();

            summarisationContextMock.SetupGet(s => s.CollectionType).Returns(collectionType);
            summarisationContextMock.SetupGet(s => s.CollectionReturnCode).Returns(collectionReturnCode);
            summarisationContextMock.SetupGet(s => s.SummarisationTypes).Returns(new List<string>() { summarisationType });
            summarisationContextMock.SetupGet(s => s.ProcessType).Returns(processType);

            repositoryMock.Setup(r => r.ProvideAsync(ukprn, summarisationContextMock.Object, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetTestProviderData(ukprn, lineType)));

            repositoryMock.Setup(r => r.GetAllProviderIdentifiersAsync(collectionType, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetProviderList(ukprn)));

            Func<IProviderRepository> providerRepositoryFunc = () =>
            {
                return repositoryMock.Object;
            };

            var fcsRepositoryMock = new Mock<IFcsRepository>();

            fcsRepositoryMock.Setup(r => r.RetrieveAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetContractAllocations(null)));

            var summarisedActualsRepositoryMock = new Mock<ISummarisedActualsProcessRepository>();
            summarisedActualsRepositoryMock.Setup(r => r.GetLastCollectionReturnForCollectionTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetLatestCollectionReturn()));
            summarisedActualsRepositoryMock.Setup(r => r.GetSummarisedActualsForCollectionReturnAndOrganisationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetSummarisedActuals()));

            var dataStorePersistenceServiceMock = new Mock<IDataStorePersistenceService>();

            List<ISummarisationConfigProvider<FundingType>> fundingTypesProviders = new List<ISummarisationConfigProvider<FundingType>>();

            var fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());
            var fundingStreams = fundingTypesProvider.Provide().SelectMany(x => x.FundingStreams.Where(y => y.FundModel == fundModel)).ToList();

            fundingTypesProviders.Add(fundingTypesProvider);

            var collectionPeriodsProvider = new[] { new CollectionPeriodsProvider(new JsonSerializationService()) };

            var dataOptions = new SummarisationDataOptions { DataRetrievalMaxConcurrentCalls = "4" };

            var collectionPeriods = collectionPeriodsProvider[0].Provide().ToList();

            List<ISummarisationService> summarisationServices = new List<ISummarisationService>()
            {
                new SummarisationFundlineProcess()
            };

            var logger = new Mock<ILogger>();

            var wrapper = new SummarisationWrapper(
                fcsRepositoryMock.Object,
                summarisedActualsRepositoryMock.Object,
                fundingTypesProviders,
                collectionPeriodsProvider,
                summarisationServices,
                dataStorePersistenceServiceMock.Object,
                providerRepositoryFunc,
                dataOptions,
                logger.Object);

            var result = await wrapper.Summarise(summarisationContextMock.Object, cancellationToken);

            if (fundModel == FundModel.FM35)
            {
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 11 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 14 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEBC-19TRN1920" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEBC-ASCL1920" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEB-19TRN1920" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEB-AS1920" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(12);
            }
            else if (fundModel == FundModel.EAS)
            {
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "16-18TRN1920" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 4 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 13 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLB1920" && s.DeliverableCode == 4 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLBC1920" && s.DeliverableCode == 4 && s.ActualValue != 0).Should().Be(12);
            }
            else if (fundModel == FundModel.FM25)
            {
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "16-18TRN1920" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(12);
            }
            else if (fundModel == FundModel.ALB)
            {
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLB1920" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLBC1920" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(12);

                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLB1920" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLBC1920" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(12);
            }
            else if (fundModel == FundModel.TBL)
            {
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 5 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 16 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 19 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 7 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 18 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 21 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 6 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 17 && s.ActualValue != 0).Should().Be(12);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 20 && s.ActualValue != 0).Should().Be(12);
            }
        }

        [Theory]
        [InlineData(10000001, "ILR1920", "R01", "Main1920_FM35", "ILR_FM35", FundModel.FM35, "Fundline")]
        [InlineData(10000001, "ILR1920", "R01", "Main1920_EAS", "EAS", FundModel.EAS, "Fundline")]
        [InlineData(10000001, "ILR1920", "R01", "Main1920_FM25", "ILR_FM25", FundModel.FM25, "Fundline")]
        [InlineData(10000001, "ILR1920", "R01", "Main1920_ALB", "ILR_ALB", FundModel.ALB, "Fundline")]
        [InlineData(10000001, "ILR1920", "R01", "Main1920_TBL", "ILR_TBL", FundModel.TBL, "Fundline")]
        public async Task SummmariseProviders_Nocontract(int ukprn, string collectionType, string collectionReturnCode, string summarisationType, string lineType, FundModel fundModel, string processType)
        {
            var cancellationToken = CancellationToken.None;

            var repositoryMock = new Mock<IProviderRepository>();

            var summarisationContextMock = new Mock<ISummarisationMessage>();

            summarisationContextMock.SetupGet(s => s.CollectionType).Returns(collectionType);
            summarisationContextMock.SetupGet(s => s.CollectionReturnCode).Returns(collectionReturnCode);
            summarisationContextMock.SetupGet(s => s.SummarisationTypes).Returns(new List<string>() { summarisationType });
            summarisationContextMock.SetupGet(s => s.ProcessType).Returns(processType);

            repositoryMock.Setup(r => r.ProvideAsync(ukprn, summarisationContextMock.Object, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetTestProviderData(ukprn, lineType)));

            repositoryMock.Setup(r => r.GetAllProviderIdentifiersAsync(collectionType, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetProviderList(ukprn)));

            Func<IProviderRepository> providerRepositoryFunc = () =>
            {
                return repositoryMock.Object;
            };

            var fspCodes = new HashSet<string>();

            var fcsRepositoryMock = new Mock<IFcsRepository>();

            fcsRepositoryMock.Setup(r => r.RetrieveAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetContractAllocations(fspCodes)));

            var summarisedActualsRepositoryMock = new Mock<ISummarisedActualsProcessRepository>();

            summarisedActualsRepositoryMock.Setup(r => r.GetLastCollectionReturnForCollectionTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetLatestCollectionReturn()));

            summarisedActualsRepositoryMock.Setup(r => r.GetSummarisedActualsForCollectionReturnAndOrganisationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetSummarisedActuals()));

            var dataStorePersistenceServiceMock = new Mock<IDataStorePersistenceService>();

            List<ISummarisationConfigProvider<FundingType>> fundingTypesProviders = new List<ISummarisationConfigProvider<FundingType>>();

            var fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());
            var fundingStreams = fundingTypesProvider.Provide().SelectMany(x => x.FundingStreams.Where(y => y.FundModel == fundModel)).ToList();

            fundingTypesProviders.Add(fundingTypesProvider);

            var collectionPeriodsProvider = new[] { new CollectionPeriodsProvider(new JsonSerializationService()) };

            var dataOptions = new SummarisationDataOptions { DataRetrievalMaxConcurrentCalls = "4" };

            var collectionPeriods = collectionPeriodsProvider[0].Provide().ToList();

            List<ISummarisationService> summarisationServices = new List<ISummarisationService>()
            {
                new SummarisationFundlineProcess()
            };

            var logger = new Mock<ILogger>();

            var wrapper = new SummarisationWrapper(
                fcsRepositoryMock.Object,
                summarisedActualsRepositoryMock.Object,
                fundingTypesProviders,
                collectionPeriodsProvider,
                summarisationServices,
                dataStorePersistenceServiceMock.Object,
                providerRepositoryFunc,
                dataOptions,
                logger.Object);
            var result = await wrapper.Summarise(summarisationContextMock.Object, cancellationToken);

            if (fundModel == FundModel.FM35)
            {
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 11 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 14 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEBC-19TRN1920" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEBC-ASCL1920" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEB-19TRN1920" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEB-AS1920" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(0);
            }
            else if (fundModel == FundModel.EAS)
            {
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "16-18TRN1920" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 4 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 13 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLB1920" && s.DeliverableCode == 4 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLBC1920" && s.DeliverableCode == 4 && s.ActualValue != 0).Should().Be(0);
            }
            else if (fundModel == FundModel.FM25)
            {
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "16-18TRN1920" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(0);
            }
            else if (fundModel == FundModel.ALB)
            {
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLB1920" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLBC1920" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(0);

                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLB1920" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLBC1920" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(0);
            }
            else if (fundModel == FundModel.TBL)
            {
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 5 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 16 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 19 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 7 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 18 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 21 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 6 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 17 && s.ActualValue != 0).Should().Be(0);
                result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1920" && s.DeliverableCode == 20 && s.ActualValue != 0).Should().Be(0);
            }
        }

        [Fact]
        public async void FundingDataRemoved_Test()
        {
            var repositoryMock = new Mock<IProviderRepository>();

            Func<IProviderRepository> providerRepositoryFunc = () =>
            {
                return repositoryMock.Object;
            };

            var fcsRepositoryMock = new Mock<IFcsRepository>();

            var summarisedActualsRepositoryMock = new Mock<ISummarisedActualsProcessRepository>();

            summarisedActualsRepositoryMock.Setup(s =>
                    s.GetSummarisedActualsForCollectionReturnAndOrganisationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(GetSummarisedActuals());

            var dataStorePersistenceServiceMock = new Mock<IDataStorePersistenceService>();

            List<ISummarisationConfigProvider<FundingType>> fundingTypesProviders = new List<ISummarisationConfigProvider<FundingType>>();

            var fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());

            fundingTypesProviders.Add(fundingTypesProvider);

            var collectionPeriodsProvider = new[] { new CollectionPeriodsProvider(new JsonSerializationService()) };

            List<ISummarisationService> summarisationServices = new List<ISummarisationService>()
            {
                new SummarisationFundlineProcess()
            };

            var logger = new Mock<ILogger>();

            var dataOptions = new SummarisationDataOptions { DataRetrievalMaxConcurrentCalls = "4" };
            var summarisationMesageMock = new Mock<ISummarisationMessage>();

            var wrapper = new SummarisationWrapper(
                fcsRepositoryMock.Object,
                summarisedActualsRepositoryMock.Object,
                fundingTypesProviders,
                collectionPeriodsProvider,
                summarisationServices,
                dataStorePersistenceServiceMock.Object,
                providerRepositoryFunc,
                dataOptions,
                logger.Object);

            var summarisedActuals = new List<Summarisation.Data.Output.Model.SummarisedActual>();

            var result = await wrapper.GetFundingDataRemoved(1, "Org1", summarisedActuals, CancellationToken.None);

            result.Count().Should().Be(2);
        }

        [Fact]
        public async void FundingDataRemovedPartial_Test()
        {
            var repositoryMock = new Mock<IProviderRepository>();

            Func<IProviderRepository> providerRepositoryFunc = () =>
            {
                return repositoryMock.Object;
            };

            var fcsRepositoryMock = new Mock<IFcsRepository>();

            var summarisedActualsRepositoryMock = new Mock<ISummarisedActualsProcessRepository>();

            summarisedActualsRepositoryMock.Setup(s =>
                    s.GetSummarisedActualsForCollectionReturnAndOrganisationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(GetSummarisedActuals());

            var dataStorePersistenceServiceMock = new Mock<IDataStorePersistenceService>();

            List<ISummarisationConfigProvider<FundingType>> fundingTypesProviders = new List<ISummarisationConfigProvider<FundingType>>();

            var fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());

            fundingTypesProviders.Add(fundingTypesProvider);

            var collectionPeriodsProvider = new[] { new CollectionPeriodsProvider(new JsonSerializationService()) };

            List<ISummarisationService> summarisationServices = new List<ISummarisationService>()
            {
                new SummarisationFundlineProcess()
            };

            var logger = new Mock<ILogger>();

            var dataOptions = new SummarisationDataOptions { DataRetrievalMaxConcurrentCalls = "4" };

            var summarisationMessageMock = new Mock<ISummarisationMessage>();

            var wrapper = new SummarisationWrapper(
                fcsRepositoryMock.Object,
                summarisedActualsRepositoryMock.Object,
                fundingTypesProviders,
                collectionPeriodsProvider,
                summarisationServices,
                dataStorePersistenceServiceMock.Object,
                providerRepositoryFunc,
                dataOptions,
                logger.Object);

            var summarisedActuals = GetSummarisedActuals().Where(x => x.FundingStreamPeriodCode == "APPS1920");

            var result = await wrapper.GetFundingDataRemoved(1, "Org1", summarisedActuals, CancellationToken.None);

            result.Count().Should().Be(1);
        }

        private IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> GetContractAllocations(HashSet<string> fspCodes)
        {
            Dictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fspContractAllocations = new Dictionary<string, IReadOnlyCollection<IFcsContractAllocation>>();

            if (fspCodes == null)
            {
                fspCodes = GetFundingStreamPeriodCodes();
            }

            foreach (var fspCode in fspCodes)
            {
                List<IFcsContractAllocation> allocations = new List<IFcsContractAllocation>();

                foreach (var ukprn in GetTestProviders())
                {
                    var fcsContractAllocation = new FcsContractAllocation
                    {
                        ContractAllocationNumber = $"Alloc{ukprn}",
                        DeliveryUkprn = ukprn,
                        DeliveryOrganisation = $"Org{ukprn}",
                        FundingStreamPeriodCode = fspCode
                    };

                    allocations.Add(fcsContractAllocation);
                }

                fspContractAllocations.Add(fspCode, allocations);
            }

            return fspContractAllocations;
        }

        private CollectionReturn GetLatestCollectionReturn()
        {
            return new CollectionReturn()
            {
                Id = 1,
                CollectionType = "ILR1920",
                CollectionReturnCode = "R01"
            };
        }

        private IEnumerable<Summarisation.Data.Output.Model.SummarisedActual> GetSummarisedActuals()
        {
            return new List<Summarisation.Data.Output.Model.SummarisedActual>()
            {
                new Summarisation.Data.Output.Model.SummarisedActual()
                {
                    OrganisationId = "Org1",
                    UoPCode = null,
                    FundingStreamPeriodCode = "APPS1920",
                    Period = 201901,
                    DeliverableCode = 2,
                    ActualVolume = 0,
                    ActualValue = 100,
                    PeriodTypeCode = "AY",
                    ContractAllocationNumber = "CA-1111"
                },
                new Summarisation.Data.Output.Model.SummarisedActual()
                {
                    OrganisationId = "Org1",
                    UoPCode = string.Empty,
                    FundingStreamPeriodCode = "AEB-AS1920",
                    Period = 201901,
                    DeliverableCode = 2,
                    ActualVolume = 0,
                    ActualValue = 200,
                    PeriodTypeCode = "AY",
                    ContractAllocationNumber = "CA-2222"
                }
            };
        }

        private IReadOnlyCollection<IProvider> GetTestProvidersData(string lineType)
        {
            List<IProvider> providersData = new List<IProvider>();

            HashSet<int> providers = GetTestProviders();

            foreach (var ukprn in providers)
            {
                providersData.Add(GetTestProviderData(ukprn, lineType));
            }

            return providersData;
        }

        private IReadOnlyCollection<IProvider> GetTestProvidersData(int pageSize, int pageNumber, string lineType)
        {
            var providersData = GetTestProvidersData(lineType).OrderBy(x => x.UKPRN).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return providersData;
        }

        private IProvider GetTestProviderData(int ukprn, string lineType)
        {
            return new Provider()
            {
                UKPRN = ukprn,
                LearningDeliveries = GetLearningDeliveries(lineType)
            };
        }

        private List<LearningDelivery> GetLearningDeliveries(string lineType)
        {
            List<LearningDelivery> learningDeliveries = new List<LearningDelivery>();

            foreach (var item in GetFundLines(lineType))
            {
                for (int i = 1; i <= 2; i++)
                {
                    LearningDelivery learningDelivery = new LearningDelivery
                    {
                        LearnRefNumber = "learnref" + i,
                        AimSeqNumber = i,
                        Fundline = item.Fundline,
                        PeriodisedData = item.LineType == "EAS" ? GetPeriodisedDataNoAttributes(1) : GetPeriodisedData(1)
                    };

                    learningDeliveries.Add(learningDelivery);
                }
            }

            return learningDeliveries;
        }

        private List<PeriodisedData> GetPeriodisedData(int lotSize)
        {
            HashSet<string> attributes = GetAllAttributes();

            List<PeriodisedData> periodisedDatas = new List<PeriodisedData>();

            for (int j = 1; j <= lotSize; j++)
            {
                foreach (var item in attributes)
                {
                    PeriodisedData periodisedData = new PeriodisedData()
                    {
                        AttributeName = item,
                        Periods = GetPeriodsData(1)
                    };

                    periodisedDatas.Add(periodisedData);
                }
            }

            return periodisedDatas;
        }

        private List<PeriodisedData> GetPeriodisedDataNoAttributes(int lotSize)
        {
            List<PeriodisedData> periodisedDatas = new List<PeriodisedData>();

            for (int j = 1; j <= lotSize; j++)
            {
                PeriodisedData periodisedData = new PeriodisedData()
                {
                    Periods = GetPeriodsData(1)
                };

                periodisedDatas.Add(periodisedData);
            }

            return periodisedDatas;
        }

        private List<Period> GetPeriodsData(int lotSize)
        {
            List<Period> periods = new List<Period>();
            for (int j = 1; j <= lotSize; j++)
            {
                for (int i = 1; i <= 12; i++)
                {
                    Period period = new Period() { PeriodId = i, Value = i * 10 };
                    periods.Add(period);
                }
            }

            return periods;
        }

        private List<FundLine> GetFundLines(string lineType)
        {
            List<FundLine> fundLines = new List<FundLine>
            {
                new FundLine { Fundline = "16-18 Apprenticeship", LineType = "ILR_FM35" },
                new FundLine { Fundline = "19-23 Apprenticeship", LineType = "ILR_FM35" },
                new FundLine { Fundline = "19-24 Traineeship (non-procured)", LineType = "ILR_FM35" },
                new FundLine { Fundline = "24+ Apprenticeship", LineType = "ILR_FM35" },
                new FundLine { Fundline = "19-24 Traineeship (procured from Nov 2017)", LineType = "ILR_FM35" },
                new FundLine { Fundline = "AEB - Other Learning", LineType = "ILR_FM35" },
                new FundLine { Fundline = "AEB - Other Learning (non-procured)", LineType = "ILR_FM35" },
                new FundLine { Fundline = "AEB - Other Learning (procured from Nov 2017)", LineType = "ILR_FM35" },
                new FundLine { Fundline = "ESFA AEB - Adult Skills (non-procured)", LineType = "ILR_FM35" },
                new FundLine { Fundline = "ESFA AEB - Adult Skills (procured from Nov 2017)", LineType = "ILR_FM35" },

                new FundLine { Fundline = "Vulnerable Bursary: 16-19 Traineeships Bursary", LineType = "EAS" },
                new FundLine { Fundline = "Free Meals: 16-19 Traineeships Bursary", LineType = "EAS" },
                new FundLine { Fundline = "Discretionary Bursary: 16-19 Traineeships Bursary", LineType = "EAS" },
                new FundLine { Fundline = "Learner Support: 16-18 Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Learner Support: 24+ Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Learner Support: 19-23 Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Excess Support: Advanced Learner Loans Bursary", LineType = "EAS" },

                new FundLine { Fundline = "Authorised Claims: 16-18 Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 19-23 Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 24+ Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 19-24 Traineeships", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: AEB-Other Learning", LineType = "EAS" },
                new FundLine { Fundline = "Princes Trust: AEB-Other Learning", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 19-24 Traineeships (From Nov 2017)", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: AEB-Other Learning (From Nov 2017)", LineType = "EAS" },

                new FundLine { Fundline = "16-18 Traineeships (Adult funded)", LineType = "ILR_FM25" },
                new FundLine { Fundline = "19+ Traineeships (Adult funded)", LineType = "ILR_FM25" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Traineeships", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 16-18 Traineeships", LineType = "EAS" },

                new FundLine { Fundline = "Advanced Learner Loans Bursary", LineType = "ILR_ALB" },
                new FundLine { Fundline = "Authorised Claims: Advanced Learner Loans Bursary", LineType = "EAS" },

                new FundLine { Fundline = "16-18 Trailblazer Apprenticeship", LineType = "ILR_TBL" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Trailblazer Apprenticeships", LineType = "EAS" },

                new FundLine { Fundline = "19-23 Trailblazer Apprenticeship", LineType = "ILR_TBL" },
                new FundLine { Fundline = "Authorised Claims: 19-23 Trailblazer Apprenticeships", LineType = "EAS" },

                new FundLine { Fundline = "24+ Trailblazer Apprenticeship", LineType = "ILR_TBL" },
                new FundLine { Fundline = "Authorised Claims: 24+ Trailblazer Apprenticeships", LineType = "EAS" },

                new FundLine { Fundline = "Excess Learning Support: 16-18 Trailblazer Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 19-23 Trailblazer Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 24+ Trailblazer Apprenticeships", LineType = "EAS" },
            };

            if (string.IsNullOrEmpty(lineType))
            {
                return fundLines;
            }
            else
            {
                return fundLines.Where(fl => fl.LineType == lineType).ToList();
            }
        }

        private HashSet<string> GetAllAttributes()
        {
            return new HashSet<string> { "AchievePayment",
                                        "BalancePayment",
                                        "OnProgPayment",
                                        "EmpOutcomePay",
                                        "LearnSuppFundCash",
                                        "LnrOnProgPay",
                                        "AreaUpliftBalPayment",
                                        "AreaUpliftOnProgPayment",
                                        "ALBSupportPayment",
                                        "CoreGovContPayment",
                                        "MathEngBalPayment",
                                        "MathEngOnProgPayment",
                                        "SmallBusPayment",
                                        "YoungAppPayment",
                                        "AchPayment"};
        }

        private HashSet<int> GetTestProviders()
        {
            return new HashSet<int> { 10000001, 10000002, 10000003 };
        }

        private IList<int> GetProviderList(int ukprn)
        {
            return new List<int> { ukprn };
        }

        private HashSet<string> GetFundingStreamPeriodCodes()
        {
            return new HashSet<string>
            {
                "APPS1920",
                "AEBC-19TRN1920",
                "AEBC-ASCL1920",
                "AEB-19TRN1920",
                "AEB-AS1920",

                "16-18TRN1920",
                "ALLB1920",
                "ALLBC1920"
            };
        }
    }
}
