using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Main1920.Service.Providers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.Main1920.Service.Tests
{
    public class CollectionPeriodTests
    {
        [Fact]
        public void CollectionPeriodCount()
        {
            NewProvider().Provide().Should().HaveCount(12);
        }

        [Theory]
        [InlineData(1, 8, 2019, 201908)]
        [InlineData(2, 9, 2019, 201909)]
        [InlineData(3, 10, 2019, 201910)]
        [InlineData(4, 11, 2019, 201911)]
        [InlineData(5, 12, 2019, 201912)]
        [InlineData(6, 1, 2020, 202001)]
        [InlineData(7, 2, 2020, 202002)]
        [InlineData(8, 3, 2020, 202003)]
        [InlineData(9, 4, 2020, 202004)]
        [InlineData(10, 5, 2020, 202005)]
        [InlineData(11, 6, 2020, 202006)]
        [InlineData(12, 7, 2020, 202007)]
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
