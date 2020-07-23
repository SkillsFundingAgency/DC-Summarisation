using ESFA.DC.Summarisation.Apps.Interfaces;
using ESFA.DC.Summarisation.Apps.Model;
using ESFA.DC.Summarisation.Interfaces;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.Summarisation.Apps.Service.Tests
{
    public class ProviderRepositoryTests
    {

        [Fact]
        public async Task TestGetAllIdentifiersAsync()
        {
            var cancellationToken = CancellationToken.None;
            var collectionType = "APPS";

            var summarisationMessageMock = new Mock<ISummarisationMessage>();
            summarisationMessageMock.Setup(sm => sm.CollectionType).Returns(collectionType);

            var esfProviders = TestAppsProviders();

            var inputDataProvider = new Mock<ISummarisationInputDataProvider>();
            inputDataProvider.Setup(x => x.CollectionType).Returns(collectionType);
            inputDataProvider.Setup(x => x.ProvideUkprnsAsync(cancellationToken)).ReturnsAsync(esfProviders);

            var inputDataProviders = new List<ISummarisationInputDataProvider>()
            {
                inputDataProvider.Object
            };

            var result = await NewService(
               inputDataProviders).GetAllIdentifiersAsync(collectionType, cancellationToken);

            result.Should().BeEquivalentTo(esfProviders);

        }

        [Fact]
        public async Task ProvideAsyncTest()
        {
            var cancellationToken = CancellationToken.None;
            var collectionType = "APPS";

            var summarisationMessageMock = new Mock<ISummarisationMessage>();
            summarisationMessageMock.Setup(sm => sm.CollectionType).Returns(collectionType);

            var providerLearningDeliveries = ProviderLearningDeliveries();

            var inputDataProvider = new Mock<ISummarisationInputDataProvider>();
            inputDataProvider.Setup(x => x.CollectionType).Returns(collectionType);
            inputDataProvider.Setup(x => x.ProvideAsync(It.IsAny<int>(), summarisationMessageMock.Object, cancellationToken)).ReturnsAsync(providerLearningDeliveries);

            var inputDataProviders = new List<ISummarisationInputDataProvider>()
            {
                inputDataProvider.Object
            };

            var result = await NewService(
               inputDataProviders).ProvideAsync(101, summarisationMessageMock.Object, cancellationToken);

            result.LearningDeliveries.Should().BeEquivalentTo(providerLearningDeliveries);
        }

        private static ProviderRepository NewService(IList<ISummarisationInputDataProvider> inputDataProviders = null)
        {
            return new ProviderRepository(
                inputDataProviders ?? Mock.Of<IList<ISummarisationInputDataProvider>>());
        }

        private static ICollection<int> TestAppsProviders()
        {
            return new List<int>
            {
               101 ,
               102
            };
        }

        private static ICollection<LearningDelivery> ProviderLearningDeliveries()
        {
            return new List<LearningDelivery>
            {
                new LearningDelivery(){ Fundline = "Fundline-1"},
                new LearningDelivery(){ Fundline = "Fundline-2"}
            };
        }
    }
}
