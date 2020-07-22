using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Main2021.Service.Providers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.Main2021.Service.Tests
{
    public class CollectionPeriodTests
    {
        [Fact]
        public void CollectionPeriodCount()
        {
            NewProvider().Provide().Should().HaveCount(12);
        }

        [Theory]
        [InlineData(1, 8, 2020, 202008)]
        [InlineData(2, 9, 2020, 202009)]
        [InlineData(3, 10, 2020, 202010)]
        [InlineData(4, 11, 2020, 202011)]
        [InlineData(5, 12, 2020, 202012)]
        [InlineData(6, 1, 2021, 202101)]
        [InlineData(7, 2, 2021, 202102)]
        [InlineData(8, 3, 2021, 202103)]
        [InlineData(9, 4, 2021, 202104)]
        [InlineData(10, 5, 2021, 202105)]
        [InlineData(11, 6, 2021, 202106)]
        [InlineData(12, 7, 2021, 202107)]
        public void CollectionPeriodConfig(int period, int calendarMonth, int calendarYear, int actualsSchemaPeriod)
        {
            var collectionPeriods = NewProvider().Provide();

            collectionPeriods.Should().Contain(cp => cp.Period == period && cp.CalendarMonth == calendarMonth && cp.CalendarYear == calendarYear && cp.ActualsSchemaPeriod == actualsSchemaPeriod);
        }

        private static CollectionPeriodsProvider NewProvider()
        {
            return new CollectionPeriodsProvider(new JsonSerializationService());
        }
    }
}
