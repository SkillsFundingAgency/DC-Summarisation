using ESFA.DC.Summarisation.Data.Persist.Mapper;
using ESFA.DC.Summarisation.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Summarisation.Data.Persist.Tests.MapperTests
{
    public class CollectionReturnMapperTests
    {
        [Fact]
        public void CollectionReturnMapper()
        {
            var summarisationContextMock = new Mock<ISummarisationContext>();

            summarisationContextMock.SetupGet(s => s.CollectionType).Returns("ILR");
            summarisationContextMock.SetupGet(s => s.CollectionReturnCode).Returns("R01");

            var collectionReturn = Mapper().MapCollectionReturn(summarisationContextMock.Object);

            collectionReturn.CollectionReturnCode.Should().Be("R01");
            collectionReturn.CollectionType.Should().Be("ILR");
        }

        private CollectionReturnMapper Mapper() => new CollectionReturnMapper();
    }
}
