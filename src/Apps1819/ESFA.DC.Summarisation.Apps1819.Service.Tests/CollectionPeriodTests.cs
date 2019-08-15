using ESFA.DC.Serialization.Json;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.Apps1819.Service.Tests
{
    public class CollectionPeriodTests
    {
        [Fact]
        public void CollectionPeriodCount()
        {
            NewProvider().Provide().Should().HaveCount(19);
        }

        [Theory]
        [InlineData(1, 1, 2018, 201801)]
        [InlineData(2, 2, 2018, 201802)]
        [InlineData(3, 3, 2018, 201803)]
        [InlineData(4, 4, 2018, 201804)]
        [InlineData(5, 5, 2018, 201805)]
        [InlineData(6, 6, 2018, 201806)]
        [InlineData(7, 7, 2018, 201807)]
        [InlineData(8, 8, 2018, 201808)]
        [InlineData(9, 9, 2018, 201809)]
        [InlineData(10, 10, 2018, 201810)]
        [InlineData(11, 11, 2018, 201811)]
        [InlineData(12, 12, 2018, 201812)]
        [InlineData(13, 1, 2019, 201901)]
        [InlineData(14, 2, 2019, 201902)]
        [InlineData(15, 3, 2019, 201903)]
        [InlineData(16, 4, 2019, 201904)]
        [InlineData(17, 5, 2019, 201905)]
        [InlineData(18, 6, 2019, 201906)]
        [InlineData(19, 7, 2019, 201907)]
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