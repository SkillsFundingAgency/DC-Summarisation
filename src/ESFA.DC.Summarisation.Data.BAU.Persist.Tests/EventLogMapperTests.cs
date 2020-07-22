using ESFA.DC.Summarisation.Data.BAU.Persist.Interfaces;
using ESFA.DC.Summarisation.Data.BAU.Persist.Mapper;
using ESFA.DC.Summarisation.Service.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.Data.BAU.Persist.Tests
{
    public class EventLogMapperTests
    {
        [Fact]
        public void MapTest()
        {
            var collectionReturnMock = new CollectionReturn()
            {
                CollectionType = "ILR",
                CollectionReturnCode = "R01"
            };

            var result = Mapper().Map(collectionReturnMock);

            result.CollectionType.Should().BeEquivalentTo("ILR");
            result.CollectionReturnCode.Should().BeEquivalentTo("R01");

        }

        private IEventLogMapper Mapper() => new EventLogMapper();

    }
}
