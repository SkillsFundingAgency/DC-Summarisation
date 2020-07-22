using System.Linq;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Main1920.Service.Providers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.Main1920.Service.Tests
{
    public class FundingTypeTests
    {
        [Fact]
        public void FundingTypesCount()
        {
            NewProvider().Provide().Should().HaveCount(5);
        }

        [Theory]
        [InlineData("Main1920_FM35", "APPS1920", 2, "Authorised Claims: 16-18 Apprenticeships", "EAS")]
        [InlineData("Main1920_FM35", "APPS1920", 2, "16-18 Apprenticeship", "ILR_FM35")]
        [InlineData("Main1920_FM35", "APPS1920", 11, "Authorised Claims: 19-23 Apprenticeships", "EAS")]
        [InlineData("Main1920_FM35", "APPS1920", 11, "19-23 Apprenticeship", "ILR_FM35")]
        [InlineData("Main1920_FM35", "APPS1920", 14, "Authorised Claims: 24+ Apprenticeships", "EAS")]
        [InlineData("Main1920_FM35", "APPS1920", 14, "24+ Apprenticeship", "ILR_FM35")]
        [InlineData("Main1920_FM35", "AEBC-19TRN1920", 2, "Authorised Claims: 19-24 Traineeships", "EAS")]
        [InlineData("Main1920_FM35", "AEBC-19TRN1920", 2, "19-24 Traineeship (non-procured)", "ILR_FM35")]
        [InlineData("Main1920_FM35", "AEBC-ASCL1920", 3, "Authorised Claims: AEB-Other Learning", "EAS")]
        [InlineData("Main1920_FM35", "AEBC-ASCL1920", 3, "Princes Trust: AEB-Other Learning", "EAS")]
        [InlineData("Main1920_FM35", "AEBC-ASCL1920", 3, "ESFA AEB - Adult Skills (non-procured)", "ILR_FM35")]
        [InlineData("Main1920_FM35", "AEB-19TRN1920", 2, "Authorised Claims: 19-24 Traineeships (From Nov 2017)", "EAS")]
        [InlineData("Main1920_FM35", "AEB-19TRN1920", 2, "19-24 Traineeship (procured from Nov 2017)", "ILR_FM35")]
        [InlineData("Main1920_FM35", "AEB-AS1920", 2, "Authorised Claims: AEB-Other Learning (From Nov 2017)", "EAS")]
        [InlineData("Main1920_FM35", "AEB-AS1920", 2, "ESFA AEB - Adult Skills (procured from Nov 2017)", "ILR_FM35")]
        [InlineData("Main1920_FM35", "AEB-AS1920", 2, "Princes Trust: AEB-Other Learning (From Nov 2017)", "EAS")]

        [InlineData("Main1920_FM35", "APPS1920", 3, "Excess Learning Support: 16-18 Apprenticeships", "EAS")]
        [InlineData("Main1920_FM35", "APPS1920", 3, "16-18 Apprenticeship", "ILR_FM35")]
        [InlineData("Main1920_FM35", "APPS1920", 12, "Excess Learning Support: 19-23 Apprenticeships", "EAS")]
        [InlineData("Main1920_FM35", "APPS1920", 12, "19-23 Apprenticeship", "ILR_FM35")]
        [InlineData("Main1920_FM35", "APPS1920", 15, "Excess Learning Support: 24+ Apprenticeships", "EAS")]
        [InlineData("Main1920_FM35", "APPS1920", 15, "24+ Apprenticeship", "ILR_FM35")]
        [InlineData("Main1920_FM35", "AEBC-19TRN1920", 3, "Excess Learning Support: 19-24 Traineeships", "EAS")]
        [InlineData("Main1920_FM35", "AEBC-19TRN1920", 3, "19-24 Traineeship (non-procured)", "ILR_FM35")]
        [InlineData("Main1920_FM35", "AEBC-ASCL1920", 4, "Excess Learning Support: AEB-Other Learning", "EAS")]
        [InlineData("Main1920_FM35", "AEBC-ASCL1920", 4, "ESFA AEB - Adult Skills (non-procured)", "ILR_FM35")]
        [InlineData("Main1920_FM35", "AEB-19TRN1920", 3, "Excess Learning Support: 19-24 Traineeships (From Nov 2017)", "EAS")]
        [InlineData("Main1920_FM35", "AEB-19TRN1920", 3, "19-24 Traineeship (procured from Nov 2017)", "ILR_FM35")]
        [InlineData("Main1920_FM35", "AEB-AS1920", 3, "Excess Learning Support: AEB-Other Learning (From Nov 2017)", "EAS")]
        [InlineData("Main1920_FM35", "AEB-AS1920", 3, "ESFA AEB - Adult Skills (procured from Nov 2017)", "ILR_FM35")]

        [InlineData("Main1920_EAS", "16-18TRN1920", 3, "Vulnerable Bursary: 16-19 Traineeships Bursary", "EAS")]
        [InlineData("Main1920_EAS", "16-18TRN1920", 3, "Free Meals: 16-19 Traineeships Bursary", "EAS")]
        [InlineData("Main1920_EAS", "16-18TRN1920", 3, "Discretionary Bursary: 16-19 Traineeships Bursary", "EAS")]
        [InlineData("Main1920_EAS", "APPS1920", 4, "Learner Support: 16-18 Apprenticeships", "EAS")]
        [InlineData("Main1920_EAS", "APPS1920", 13, "Learner Support: 24+ Apprenticeships", "EAS")]
        [InlineData("Main1920_EAS", "APPS1920", 13, "Learner Support: 19-23 Apprenticeships", "EAS")]
        [InlineData("Main1920_EAS", "ALLB1920", 4, "Excess Support: Advanced Learner Loans Bursary", "EAS")]
        [InlineData("Main1920_EAS", "ALLBC1920", 4, "Excess Support: Advanced Learner Loans Bursary", "EAS")]

        [InlineData("Main1920_FM25", "16-18TRN1920", 2, "Authorised Claims: 16-18 Traineeships", "EAS")]
        [InlineData("Main1920_FM25", "16-18TRN1920", 2, "Excess Learning Support: 16-18 Traineeships", "EAS")]
        [InlineData("Main1920_FM25", "16-18TRN1920", 2, "16-18 Traineeships (Adult funded)", "ILR_FM25")]
        [InlineData("Main1920_FM25", "16-18TRN1920", 2, "19+ Traineeships (Adult funded)", "ILR_FM25")]

        [InlineData("Main1920_ALB", "ALLB1920", 3, "Authorised Claims: Advanced Learner Loans Bursary", "EAS")]
        [InlineData("Main1920_ALB", "ALLB1920", 3, "Advanced Learner Loans Bursary", "ILR_ALB")]
        [InlineData("Main1920_ALB", "ALLBC1920", 3, "Authorised Claims: Advanced Learner Loans Bursary", "EAS")]
        [InlineData("Main1920_ALB", "ALLBC1920", 3, "Advanced Learner Loans Bursary", "ILR_ALB")]

        [InlineData("Main1920_ALB", "ALLB1920", 2, "Advanced Learner Loans Bursary", "ILR_ALB")]
        [InlineData("Main1920_ALB", "ALLBC1920", 2, "Advanced Learner Loans Bursary", "ILR_ALB")]

        [InlineData("Main1920_TBL", "APPS1920", 5, "Authorised Claims: 16-18 Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1920_TBL", "APPS1920", 5, "16-18 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main1920_TBL", "APPS1920", 16, "Authorised Claims: 19-23 Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1920_TBL", "APPS1920", 16, "19-23 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main1920_TBL", "APPS1920", 19, "Authorised Claims: 24+ Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1920_TBL", "APPS1920", 19, "24+ Trailblazer Apprenticeship", "ILR_TBL")]

        [InlineData("Main1920_TBL", "APPS1920", 6, "Excess Learning Support: 16-18 Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1920_TBL", "APPS1920", 6, "16-18 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main1920_TBL", "APPS1920", 17, "Excess Learning Support: 19-23 Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1920_TBL", "APPS1920", 17, "19-23 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main1920_TBL", "APPS1920", 20, "Excess Learning Support: 24+ Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1920_TBL", "APPS1920", 20, "24+ Trailblazer Apprenticeship", "ILR_TBL")]

        [InlineData("Main1920_TBL", "APPS1920", 7, "16-18 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main1920_TBL", "APPS1920", 18, "19-23 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main1920_TBL", "APPS1920", 21, "24+ Trailblazer Apprenticeship", "ILR_TBL")]
        public void FundLineConfiguration(string summarisationType, string fspCode, int dlc, string fundLine, string lineType)
        {
            FundingTypesProvider fundingTypesProvider = NewProvider();

            var fundingTypes = fundingTypesProvider.Provide();

            fundingTypes.Should().Contain(ft => ft.SummarisationType == summarisationType);

            var fundingStreams = fundingTypes.First(ft => ft.SummarisationType == summarisationType).FundingStreams;

            fundingStreams.Should().Contain(fs => fs.PeriodCode == fspCode && fs.DeliverableLineCode == dlc);

            var fundLines = fundingStreams.First(fs => fs.PeriodCode == fspCode && fs.DeliverableLineCode == dlc).FundLines;

            fundLines.Should().Contain(fl => fl.Fundline == fundLine && fl.LineType == lineType);
        }

        private static FundingTypesProvider NewProvider()
        {
            return new FundingTypesProvider(new JsonSerializationService());
        }
    }
}
