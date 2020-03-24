using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.NCS.NCS2021.Service.Providers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.NCS.NCS2021.Service.Tests
{
    public class CollectionPeriodTests
    {
        [Fact]
        public void CollectionPeriodCount()
        {
            NewProvider().Provide().Should().HaveCount(12);
        }

        [Theory]
        [InlineData(1, 4, 2020, 202004, 1, 2021)]
        [InlineData(2, 5, 2020, 202005, 2, 2021)]
        [InlineData(3, 6, 2020, 202006, 3, 2021)]
        [InlineData(4, 7, 2020, 202007, 4, 2021)]
        [InlineData(5, 8, 2020, 202008, 5, 2021)]
        [InlineData(6, 9, 2020, 202009, 6, 2021)]
        [InlineData(7, 10, 2020, 202010, 7, 2021)]
        [InlineData(8, 11, 2020, 202011, 8, 2021)]
        [InlineData(9, 12, 2020, 202012, 9, 2021)]
        [InlineData(10, 1, 2021, 202101, 10, 2021)]
        [InlineData(11, 2, 2021, 202102, 11, 2021)]
        [InlineData(12, 3, 2021, 202103, 12, 2021)]
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
