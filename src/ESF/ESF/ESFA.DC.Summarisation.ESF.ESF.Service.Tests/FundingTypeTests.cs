using System.Linq;
using ESFA.DC.Serialization.Json;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.ESF.ESF.Service.Tests
{
    public class FundingTypeTests
    {
        [Fact]
        public void FundingTypesCount()
        {
            NewProvider().Provide().Should().HaveCount(3);
        }

        [Theory]
        [InlineData("ESF_SuppData", "ESF1420", 5, "AC01")]
        [InlineData("ESF_SuppData", "ESF1420", 6, "CG01")]
        [InlineData("ESF_SuppData", "ESF1420", 7, "CG02")]

        [InlineData("ESF_SuppData", "ESF1420", 8, "SD01")]
        [InlineData("ESF_SuppData", "ESF1420", 9, "SD02")]
        [InlineData("ESF_SuppData", "ESF1420", 10, "SD03")]
        [InlineData("ESF_SuppData", "ESF1420", 11, "SD04")]
        [InlineData("ESF_SuppData", "ESF1420", 12, "SD05")]
        [InlineData("ESF_SuppData", "ESF1420", 13, "SD06")]
        [InlineData("ESF_SuppData", "ESF1420", 14, "SD07")]
        [InlineData("ESF_SuppData", "ESF1420", 15, "SD08")]
        [InlineData("ESF_SuppData", "ESF1420", 16, "SD09")]
        [InlineData("ESF_SuppData", "ESF1420", 17, "SD10")]

        [InlineData("ESF_ILR_And_Supp", "ESF1420", 1, "ST01")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 4, "FS01")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 18, "PG01")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 19, "PG02")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 20, "PG03")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 21, "PG04")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 22, "PG05")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 23, "PG06")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 24, "SU01")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 25, "SU02")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 26, "SU03")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 27, "SU04")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 28, "SU05")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 29, "SU11")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 30, "SU12")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 31, "SU13")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 32, "SU14")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 33, "SU15")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 34, "SU21")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 35, "SU22")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 36, "SU23")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 37, "SU24")]

        [InlineData("ESF_ILR_And_Supp", "ESF1420", 2, "RQ01")]
        [InlineData("ESF_ILR_And_Supp", "ESF1420", 3, "NR01")]

        public void FundLineConfiguration(string summarisationType, string fspCode, int dlc, string deliverableCode)
        {
            FundingTypesProvider fundingTypesProvider = NewProvider();

            var fundingTypes = fundingTypesProvider.Provide();

            fundingTypes.Should().Contain(ft => ft.SummarisationType == summarisationType);

            var fundingStreams = fundingTypes.First(ft => ft.SummarisationType == summarisationType).FundingStreams;

            fundingStreams.Should().Contain(fs => fs.PeriodCode == fspCode && fs.DeliverableLineCode == dlc);

            var fundLines = fundingStreams.First(fs => fs.PeriodCode == fspCode && fs.DeliverableLineCode == dlc).DeliverableLines;

            fundLines.Should().Contain(fl => fl.DeliverableCode == deliverableCode);
        }

        private FundingTypesProvider NewProvider()
        {
            return new FundingTypesProvider(new JsonSerializationService());
        }
    }
}