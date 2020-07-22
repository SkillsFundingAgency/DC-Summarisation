using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.NCS.Interfaces;
using ESFA.DC.Summarisation.NCS.Model;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.Summarisation.NCS.Service.Tests
{
    public class ProviderRepositoryTests
    {
        [Fact]
        public async Task TestGetAllIdentifiersAsync()
        {
            var cancellationToken = CancellationToken.None;
            var collectionType = "NCS1920";

            var summarisationMessageMock = new Mock<ISummarisationMessage>();
            summarisationMessageMock.Setup(sm => sm.CollectionType).Returns(collectionType);

            var touchpointProviders = TestTouchpointProviders();

            var inputDataProvider = new Mock<ISummarisationInputDataProvider>();
            inputDataProvider.Setup(x => x.ProvideUkprnsAsync(1920, cancellationToken)).ReturnsAsync(touchpointProviders);

            var inputDataProviders = new List<ISummarisationInputDataProvider>()
            {
                inputDataProvider.Object
            };

            var result = await NewService(
               inputDataProviders).GetAllIdentifiersAsync(1920, cancellationToken);

            result.Should().BeEquivalentTo(touchpointProviders);

        }

        [Fact]
        public async Task ProvideAsyncTest()
        {
            var cancellationToken = CancellationToken.None;
            var collectionType = "NCS1920";

            var summarisationMessageMock = new Mock<ISummarisationMessage>();
            summarisationMessageMock.Setup(sm => sm.CollectionType).Returns(collectionType);

            var touchpointProviderFundingData = TestTouchpointProviderFundingData();

            var inputDataProvider = new Mock<ISummarisationInputDataProvider>();
            //inputDataProvider.Setup(x => x.CollectionType).Returns(collectionType);
            inputDataProvider.Setup(x => x.ProvideAsync(touchpointProviderFundingData.Provider, summarisationMessageMock.Object, cancellationToken)).ReturnsAsync(touchpointProviderFundingData.FundingValues);

            var inputDataProviders = new List<ISummarisationInputDataProvider>()
            {
                inputDataProvider.Object
            };

            var result = await NewService(
               inputDataProviders).ProvideAsync(touchpointProviderFundingData.Provider, summarisationMessageMock.Object, cancellationToken);

            result.Provider.Should().BeEquivalentTo(touchpointProviderFundingData.Provider);

            result.FundingValues.Should().BeEquivalentTo(touchpointProviderFundingData.FundingValues);
        }

        private ProviderRepository NewService(IList<ISummarisationInputDataProvider> inputDataProviders = null)
        {
            return new ProviderRepository(
                inputDataProviders ?? Mock.Of<IList<ISummarisationInputDataProvider>>());
        }

        private List<TouchpointProvider> TestTouchpointProviders()
        {
            return new List<TouchpointProvider>
            {
                new TouchpointProvider{ TouchpointId = "101", UKPRN = 101},
                new TouchpointProvider{ TouchpointId = "102", UKPRN = 102}
            };
        }

        private TouchpointProviderFundingData TestTouchpointProviderFundingData()
        {
            return new TouchpointProviderFundingData
            {
                 Provider = new TouchpointProvider{ TouchpointId = "101", UKPRN = 101},
                 
                 FundingValues = new List<FundingValue>()
                 {
                     new FundingValue {CalendarMonth = 4, CollectionYear = 1920, OutcomeType = 1, Value = 100 }
                 }
            };
        }
    }
}
