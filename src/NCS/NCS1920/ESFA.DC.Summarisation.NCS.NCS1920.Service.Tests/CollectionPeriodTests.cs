using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.NCS.NCS1920.Service.Providers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.NCS.NCS1920.Service.Tests
{
    public class CollectionPeriodTests
    {
        [Fact]
        public void CollectionPeriodCount()
        {
            NewProvider().Provide().Should().HaveCount(12);
        }

        [Theory]
        [InlineData(1, 4, 2019, 201904, 1, 1920)]
        [InlineData(2, 5, 2019, 201905, 2, 1920)]
        [InlineData(3, 6, 2019, 201906, 3, 1920)]
        [InlineData(4, 7, 2019, 201907, 4, 1920)]
        [InlineData(5, 8, 2019, 201908, 5, 1920)]
        [InlineData(6, 9, 2019, 201909, 6, 1920)]
        [InlineData(7, 10, 2019, 201910, 7, 1920)]
        [InlineData(8, 11, 2019, 201911, 8, 1920)]
        [InlineData(9, 12, 2019, 201912, 9, 1920)]
        [InlineData(10, 1, 2020, 202001, 10, 1920)]
        [InlineData(11, 2, 2020, 202002, 11, 1920)]
        [InlineData(12, 3, 2020, 202003, 12, 1920)]
        public void CollectionPeriodConfig(int period, int calendarMonth, int calendarYear, int actualsSchemaPeriod, int collectionMonth, int collectionYear)
        {
            var collectionPeriods = NewProvider().Provide();

            collectionPeriods.Should().Contain(cp => cp.Period == period && cp.CalendarMonth == calendarMonth && cp.CalendarYear == calendarYear && cp.ActualsSchemaPeriod == actualsSchemaPeriod && cp.CollectionMonth == collectionMonth && cp.CollectionYear == collectionYear);
        }

        private CollectionPeriodsProvider NewProvider()
        {
            return new CollectionPeriodsProvider(new JsonSerializationService());
        }
    }
}
