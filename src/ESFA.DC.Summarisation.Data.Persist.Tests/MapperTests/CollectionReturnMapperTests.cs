using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Data.Persist.Mapper;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.Data.Persist.Tests.MapperTests
{
    public class CollectionReturnMapperTests
    {
        [Fact]
        public void CollectionReturnMapper()
        {
            var summarisationMessage = new SummarisationMessage { CollectionReturnCode = "R01", CollectionType = "ILR" };

            var collectionReturn = Mapper().MapCollectionReturn(summarisationMessage);

            collectionReturn.CollectionReturnCode.Should().Be("R01");
            collectionReturn.CollectionType.Should().Be("ILR");
        }

        private CollectionReturnMapper Mapper() => new CollectionReturnMapper();
    }
}
