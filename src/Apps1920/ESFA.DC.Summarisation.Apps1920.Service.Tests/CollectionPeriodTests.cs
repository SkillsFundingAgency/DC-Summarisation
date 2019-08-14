using ESFA.DC.Serialization.Json;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.Apps1920.Service.Tests
{
    public class CollectionPeriodTests
    {
        [Fact]
        public void CollectionPeriodCount()
        {
            NewProvider().Provide().Should().HaveCount(31);
        }

        [Theory]
        [InlineData(20, 8, 2019, 201908)]
        [InlineData(21, 9, 2019, 201909)]
        [InlineData(22, 10, 2019, 201910)]
        [InlineData(23, 11, 2019, 201911)]
        [InlineData(24, 12, 2019, 201912)]
        [InlineData(25, 1, 2020, 202001)]
        [InlineData(26, 2, 2020, 202002)]
        [InlineData(27, 3, 2020, 202003)]
        [InlineData(28, 4, 2020, 202004)]
        [InlineData(29, 5, 2020, 202005)]
        [InlineData(30, 6, 2020, 202006)]
        [InlineData(31, 7, 2020, 202007)]
        public void CollectionPeriodConfig(int period, int calendarMonth, int calendarYear, int actualsSchemaPeriod)
        {
            var collectionPeriods = NewProvider().Provide();

            collectionPeriods.Should().Contain(cp => cp.Period == period && cp.CalendarMonth == calendarMonth && cp.CalendarYear == calendarYear && cp.ActualsSchemaPeriod == actualsSchemaPeriod);
        }

        private CollectionPeriodsProvider NewProvider()
        {
            return new CollectionPeriodsProvider(new JsonSerializationService());
        }
    }
}