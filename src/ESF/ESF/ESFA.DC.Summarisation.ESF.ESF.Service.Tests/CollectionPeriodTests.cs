using ESFA.DC.Serialization.Json;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.ESF.ESF.Service.Tests
{
    public class CollectionPeriodTests
    {
        [Fact]
        public void CollectionPeriodCount()
        {
            NewProvider().Provide().Should().HaveCount(67);
        }

        private static CollectionPeriodsProvider NewProvider()
        {
            return new CollectionPeriodsProvider(new JsonSerializationService());
        }
    }
}