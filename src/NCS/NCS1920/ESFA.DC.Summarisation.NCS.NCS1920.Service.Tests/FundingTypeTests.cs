using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.NCS.Model.Config;
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
            var fundingTypes = FundingTypesConfigured();

            fundingTypes.Count.Should().Be(1);

            fundingTypes.First().SummarisationType.Should().Be("NCS1920_C");
        }

        [Theory]
        [InlineData("NCS-C1920", 2, "1")]
        [InlineData("NCS-C1920", 4, "2")]
        [InlineData("NCS-C1920", 5, "3,4,5")]
        public void FundLineConfiguration(string fspCode, int dlc, string outcomes)
        {

            var fundingStreams = FundingTypesConfigured().SelectMany(x => x.FundingStreams);

            var fudningStream = fundingStreams.First(w => w.PeriodCode == fspCode && w.DeliverableLineCode == dlc);

            List<int> outcomeTypes = outcomes.Split(',').Select(int.Parse).ToList();

            fudningStream.OutcomeTypes.Should().BeEquivalentTo(outcomeTypes);
        }

        private ICollection<FundingType> FundingTypesConfigured()
        {
           var FundingTypesProvider =  new FundingTypesProvider(new JsonSerializationService());

           return FundingTypesProvider.Provide();

        }
    }
}
