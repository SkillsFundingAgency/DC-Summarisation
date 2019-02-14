using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace ESFA.DC.Summarisation.Main1819.Service.Tests
{
    public class CollectionPeriodTests
    {
        [Fact]
        public void CollectionPeriodCount()
        {
            NewProvider().Provide().Should().HaveCount(12);
        }

        [Theory]
        [InlineData(1, 8, 2018, 201808)]
        [InlineData(2, 9, 2018, 201809)]
        [InlineData(3, 10, 2018, 201810)]
        [InlineData(4, 11, 2018, 201811)]
        [InlineData(5, 12, 2018, 201812)]
        [InlineData(6, 1, 2019, 201901)]
        [InlineData(7, 2, 2019, 201902)]
        [InlineData(8, 3, 2019, 201903)]
        [InlineData(9, 4, 2019, 201904)]
        [InlineData(10, 5, 2019, 201905)]
        [InlineData(11, 6, 2019, 201906)]
        [InlineData(12, 7, 2019, 201907)]
        public void CollectionPeriodConfig( int period, int calendarMonth, int calendarYear,int actualsSchemaPeriod)
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
