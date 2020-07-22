using ESFA.DC.Summarisation.Data.BAU.Persist.Interfaces;
using ESFA.DC.Summarisation.Data.BAU.Persist.Mapper;
using ESFA.DC.Summarisation.Data.BAU.Persist.Model;
using ESFA.DC.Summarisation.Service.Model;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.Summarisation.Data.BAU.Persist.Tests
{
    public class SummarisedActualsBAUMapperTests
    {
        [Fact]
        public void MapTest()
        {
            var sldSummarisedActuals = TestSummarisedActuals();
            var bauSummarisedActuals = TestSummarisedActualsBAU();

            var mapperMock = new Mock<ISummarisedActualBAUMapper>();
            mapperMock.Setup(s => s.Map(sldSummarisedActuals, It.IsAny<string>(), It.IsAny<string>())).Returns(bauSummarisedActuals);

            var result = Mapper().Map(sldSummarisedActuals, "CollectionType", "CollectionReturnCode");

            result.Should().BeEquivalentTo(bauSummarisedActuals);

        }

        private static ISummarisedActualBAUMapper Mapper() => new SummarisedActualBAUMapper();

        private static IEnumerable<SummarisedActual> TestSummarisedActuals()
        {
            return new List<SummarisedActual>
            {
                new SummarisedActual { CollectionReturnId = 1},
                new SummarisedActual { CollectionReturnId = 2}
            };
        }

        private static ICollection<SummarisedActualBAU> TestSummarisedActualsBAU()
        {
            return new List<SummarisedActualBAU>
            {
                new SummarisedActualBAU { CollectionType = "CollectionType", CollectionReturnCode = "CollectionReturnCode"},
                new SummarisedActualBAU { CollectionType = "CollectionType", CollectionReturnCode = "CollectionReturnCode"}
            };
        }
    }
}
