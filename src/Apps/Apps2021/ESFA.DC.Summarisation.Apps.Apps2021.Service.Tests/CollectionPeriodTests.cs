using ESFA.DC.Serialization.Json;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.Apps.Apps2021.Service.Tests
{
    public class CollectionPeriodTests
    {
        [Fact]
        public void CollectionPeriodCount()
        {
            NewProvider().Provide().Should().HaveCount(47);
        }

        [Theory]
        [InlineData(1, 2018, 201801, 6, 1718)]
        [InlineData(2, 2018, 201802, 7, 1718)]
        [InlineData(3, 2018, 201803, 8, 1718)]
        [InlineData(4, 2018, 201804, 9, 1718)]
        [InlineData(5, 2018, 201805, 10, 1718)]
        [InlineData(6, 2018, 201806, 11, 1718)]
        [InlineData(7, 2018, 201807, 12, 1718)]

        [InlineData(8, 2018, 201808, 1, 1819)]
        [InlineData(9, 2018, 201809, 2, 1819)]
        [InlineData(10, 2018, 201810, 3, 1819)]
        [InlineData(11, 2018, 201811, 4, 1819)]
        [InlineData(12, 2018, 201812, 5, 1819)]
        [InlineData(1, 2019, 201901, 6, 1819)]
        [InlineData(2, 2019, 201902, 7, 1819)]
        [InlineData(3, 2019, 201903, 8, 1819)]
        [InlineData(4, 2019, 201904, 9, 1819)]
        [InlineData(5, 2019, 201905, 10, 1819)]
        [InlineData(6, 2019, 201906, 11, 1819)]
        [InlineData(7, 2019, 201907, 12, 1819)]
        [InlineData(9, 2019, 201909, 13, 1819)]
        [InlineData(10, 2019, 201910, 14, 1819)]

        [InlineData(8, 2019, 201908, 1, 1920)]
        [InlineData(9, 2019, 201909, 2, 1920)]
        [InlineData(10, 2019, 201910, 3, 1920)]
        [InlineData(11, 2019, 201911, 4, 1920)]
        [InlineData(12, 2019, 201912, 5, 1920)]
        [InlineData(1, 2020, 202001, 6, 1920)]
        [InlineData(2, 2020, 202002, 7, 1920)]
        [InlineData(3, 2020, 202003, 8, 1920)]
        [InlineData(4, 2020, 202004, 9, 1920)]
        [InlineData(5, 2020, 202005, 10, 1920)]
        [InlineData(6, 2020, 202006, 11, 1920)]
        [InlineData(7, 2020, 202007, 12, 1920)]
        [InlineData(9, 2020, 202009, 13, 1920)]
        [InlineData(10, 2020, 202010, 14, 1920)]

        [InlineData(8, 2020, 202008, 1, 2021)]
        [InlineData(9, 2020, 202009, 2, 2021)]
        [InlineData(10, 2020, 202010, 3, 2021)]
        [InlineData(11, 2020, 202011, 4, 2021)]
        [InlineData(12, 2020, 202012, 5, 2021)]
        [InlineData(1, 2021, 202101, 6, 2021)]
        [InlineData(2, 2021, 202102, 7, 2021)]
        [InlineData(3, 2021, 202103, 8, 2021)]
        [InlineData(4, 2021, 202104, 9, 2021)]
        [InlineData(5, 2021, 202105, 10, 2021)]
        [InlineData(6, 2021, 202106, 11, 2021)]
        [InlineData(7, 2021, 202107, 12, 2021)]
        public void CollectionPeriodConfig(int calendarMonth, int calendarYear, int actualsSchemaPeriod, int collectionMonth, int collectionYear)
        {
            var collectionPeriods = NewProvider().Provide();

            collectionPeriods.Should().Contain(cp => cp.CalendarMonth == calendarMonth && cp.CalendarYear == calendarYear && cp.ActualsSchemaPeriod == actualsSchemaPeriod && cp.CollectionMonth == collectionMonth && cp.CollectionYear == collectionYear);
        }

        private static CollectionPeriodsProvider NewProvider()
        {
            return new CollectionPeriodsProvider(new JsonSerializationService());
        }
    }
}