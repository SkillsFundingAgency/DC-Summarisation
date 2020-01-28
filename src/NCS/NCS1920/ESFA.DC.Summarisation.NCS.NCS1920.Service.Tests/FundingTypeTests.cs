using System.Linq;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.NCS.NCS1920.Service.Providers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.NCS.NCS1920.Service.Tests
{
    public class FundingTypeTests
    {
        [Fact]
        public void FundingTypesCount()
        {
            NewProvider().Provide().Should().HaveCount(1);
        }

        [Theory]
        [InlineData("NCS1920", "NCS-C1920", 2)]
        [InlineData("NCS1920", "NCS-C1920", 4)]
        [InlineData("NCS1920", "NCS-C1920", 5)]
        public void FundLineConfiguration(string summarisationType, string fspCode, int dlc)
        {
            FundingTypesProvider fundingTypesProvider = NewProvider();

            var fundingTypes = fundingTypesProvider.Provide();

            fundingTypes.Should().Contain(ft => ft.SummarisationType == summarisationType);

            var fundingStreams = fundingTypes.First(ft => ft.SummarisationType == summarisationType).FundingStreams;

            fundingStreams.Should().Contain(fs => fs.PeriodCode == fspCode && fs.DeliverableLineCode == dlc);
        }

        private FundingTypesProvider NewProvider()
        {
            return new FundingTypesProvider(new JsonSerializationService());
        }
    }
}
