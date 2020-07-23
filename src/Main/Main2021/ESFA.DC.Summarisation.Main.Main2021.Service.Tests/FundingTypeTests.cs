using System.Linq;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Main2021.Service.Providers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.Main2021.Service.Tests
{
    public class FundingTypeTests
    {
        [Fact]
        public void FundingTypesCount()
        {
            NewProvider().Provide().Should().HaveCount(5);
        }

        [Theory]
        [InlineData("Main2021_FM35", "STFIC2021", 5, "Short Term Funding Initiative 1", "ILR_FM35")]
        [InlineData("Main2021_FM35", "STFIC2021", 5, "Authorised Claims: Short Term Funding Initiative 1", "EAS")]
        [InlineData("Main2021_FM35", "STFIC2021", 6, "Short Term Funding Initiative 1", "ILR_FM35")]
        [InlineData("Main2021_FM35", "STFIC2021", 7, "Short Term Funding Initiative 2", "ILR_FM35")]
        [InlineData("Main2021_FM35", "STFIC2021", 7, "Authorised Claims: Short Term Funding Initiative 2", "EAS")]
        [InlineData("Main2021_FM35", "STFIC2021", 8, "Short Term Funding Initiative 2", "ILR_FM35")]
        [InlineData("Main2021_FM35", "STFIC2021", 9, "Short Term Funding Initiative 3", "ILR_FM35")]
        [InlineData("Main2021_FM35", "STFIC2021", 9, "Authorised Claims: Short Term Funding Initiative 3", "EAS")]
        [InlineData("Main2021_FM35", "STFIC2021", 10, "Short Term Funding Initiative 3", "ILR_FM35")]
        [InlineData("Main2021_FM35", "STFIC2021", 11, "Short Term Funding Initiative 4", "ILR_FM35")]
        [InlineData("Main2021_FM35", "STFIC2021", 11, "Authorised Claims: Short Term Funding Initiative 4", "EAS")]
        [InlineData("Main2021_FM35", "STFIC2021", 12, "Short Term Funding Initiative 4", "ILR_FM35")]

        [InlineData("Main2021_FM35", "STFI2021", 5, "Short Term Funding Initiative 1", "ILR_FM35")]
        [InlineData("Main2021_FM35", "STFI2021", 5, "Authorised Claims: Short Term Funding Initiative 1", "EAS")]
        [InlineData("Main2021_FM35", "STFI2021", 6, "Short Term Funding Initiative 1", "ILR_FM35")]
        [InlineData("Main2021_FM35", "STFI2021", 7, "Short Term Funding Initiative 2", "ILR_FM35")]
        [InlineData("Main2021_FM35", "STFI2021", 7, "Authorised Claims: Short Term Funding Initiative 2", "EAS")]
        [InlineData("Main2021_FM35", "STFI2021", 8, "Short Term Funding Initiative 2", "ILR_FM35")]
        [InlineData("Main2021_FM35", "STFI2021", 9, "Short Term Funding Initiative 3", "ILR_FM35")]
        [InlineData("Main2021_FM35", "STFI2021", 9, "Authorised Claims: Short Term Funding Initiative 3", "EAS")]
        [InlineData("Main2021_FM35", "STFI2021", 10, "Short Term Funding Initiative 3", "ILR_FM35")]
        [InlineData("Main2021_FM35", "STFI2021", 11, "Short Term Funding Initiative 4", "ILR_FM35")]
        [InlineData("Main2021_FM35", "STFI2021", 11, "Authorised Claims: Short Term Funding Initiative 4", "EAS")]
        [InlineData("Main2021_FM35", "STFI2021", 12, "Short Term Funding Initiative 4", "ILR_FM35")]

        [InlineData("Main2021_FM35", "APPS2021", 2, "Authorised Claims: 16-18 Apprenticeships", "EAS")]
        [InlineData("Main2021_FM35", "APPS2021", 2, "16-18 Apprenticeship", "ILR_FM35")]
        [InlineData("Main2021_FM35", "APPS2021", 3, "Excess Learning Support: 16-18 Apprenticeships", "EAS")]
        [InlineData("Main2021_FM35", "APPS2021", 3, "16-18 Apprenticeship", "ILR_FM35")]
        [InlineData("Main2021_FM35", "APPS2021", 11, "Authorised Claims: 19-23 Apprenticeships", "EAS")]
        [InlineData("Main2021_FM35", "APPS2021", 11, "19-23 Apprenticeship", "ILR_FM35")]
        [InlineData("Main2021_FM35", "APPS2021", 12, "Excess Learning Support: 19-23 Apprenticeships", "EAS")]
        [InlineData("Main2021_FM35", "APPS2021", 12, "19-23 Apprenticeship", "ILR_FM35")]
        [InlineData("Main2021_FM35", "APPS2021", 14, "Authorised Claims: 24+ Apprenticeships", "EAS")]
        [InlineData("Main2021_FM35", "APPS2021", 14, "24+ Apprenticeship", "ILR_FM35")]
        [InlineData("Main2021_FM35", "APPS2021", 15, "Excess Learning Support: 24+ Apprenticeships", "EAS")]
        [InlineData("Main2021_FM35", "APPS2021", 15, "24+ Apprenticeship", "ILR_FM35")]

        [InlineData("Main2021_FM35", "AEBC-ASCL2021", 3, "Authorised Claims: AEB-Other Learning", "EAS")]
        [InlineData("Main2021_FM35", "AEBC-ASCL2021", 3, "Princes Trust: AEB-Other Learning", "EAS")]
        [InlineData("Main2021_FM35", "AEBC-ASCL2021", 3, "ESFA AEB - Adult Skills (non-procured)", "ILR_FM35")]
        [InlineData("Main2021_FM35", "AEBC-ASCL2021", 4, "Excess Learning Support: AEB-Other Learning", "EAS")]
        [InlineData("Main2021_FM35", "AEBC-ASCL2021", 4, "ESFA AEB - Adult Skills (non-procured)", "ILR_FM35")]

        [InlineData("Main2021_FM35", "AEBC-19TRN2021", 2, "Authorised Claims: 19-24 Traineeships", "EAS")]
        [InlineData("Main2021_FM35", "AEBC-19TRN2021", 2, "19-24 Traineeship (non-procured)", "ILR_FM35")]
        [InlineData("Main2021_FM35", "AEBC-19TRN2021", 3, "Excess Learning Support: 19-24 Traineeships", "EAS")]
        [InlineData("Main2021_FM35", "AEBC-19TRN2021", 3, "19-24 Traineeship (non-procured)", "ILR_FM35")]

        [InlineData("Main2021_FM35", "AEB-AS2021", 2, "Authorised Claims: AEB-Other Learning (From Nov 2017)", "EAS")]
        [InlineData("Main2021_FM35", "AEB-AS2021", 2, "Princes Trust: AEB-Other Learning (From Nov 2017)", "EAS")]
        [InlineData("Main2021_FM35", "AEB-AS2021", 2, "ESFA AEB - Adult Skills (procured from Nov 2017)", "ILR_FM35")]
        [InlineData("Main2021_FM35", "AEB-AS2021", 3, "Excess Learning Support: AEB-Other Learning (From Nov 2017)", "EAS")]
        [InlineData("Main2021_FM35", "AEB-AS2021", 3, "ESFA AEB - Adult Skills (procured from Nov 2017)", "ILR_FM35")]

        [InlineData("Main2021_FM35", "AEB-19TRN2021", 2, "Authorised Claims: 19-24 Traineeships (From Nov 2017)", "EAS")]
        [InlineData("Main2021_FM35", "AEB-19TRN2021", 2, "19-24 Traineeship (procured from Nov 2017)", "ILR_FM35")]
        [InlineData("Main2021_FM35", "AEB-19TRN2021", 3, "Excess Learning Support: 19-24 Traineeships (From Nov 2017)", "EAS")]
        [InlineData("Main2021_FM35", "AEB-19TRN2021", 3, "19-24 Traineeship (procured from Nov 2017)", "ILR_FM35")]

        [InlineData("Main2021_EAS", "APPS2021", 4, "Learner Support: 16-18 Apprenticeships", "EAS")]
        [InlineData("Main2021_EAS", "APPS2021", 13, "Learner Support: 24+ Apprenticeships", "EAS")]
        [InlineData("Main2021_EAS", "APPS2021", 13, "Learner Support: 19-23 Apprenticeships", "EAS")]
        [InlineData("Main2021_EAS", "ALLBC2021", 4, "Excess Support: Advanced Learner Loans Bursary", "EAS")]
        [InlineData("Main2021_EAS", "ALLB2021", 4, "Excess Support: Advanced Learner Loans Bursary", "EAS")]
        [InlineData("Main2021_EAS", "16-18TRN2021", 3, "Vulnerable Bursary: 16-19 Traineeships Bursary", "EAS")]
        [InlineData("Main2021_EAS", "16-18TRN2021", 3, "Free Meals: 16-19 Traineeships Bursary", "EAS")]
        [InlineData("Main2021_EAS", "16-18TRN2021", 3, "Discretionary Bursary: 16-19 Traineeships Bursary", "EAS")]

        [InlineData("Main2021_FM25", "16-18TRN2021", 2, "Authorised Claims: 16-18 Traineeships", "EAS")]
        [InlineData("Main2021_FM25", "16-18TRN2021", 2, "Excess Learning Support: 16-18 Traineeships", "EAS")]
        [InlineData("Main2021_FM25", "16-18TRN2021", 2, "16-18 Traineeships (Adult funded)", "ILR_FM25")]
        [InlineData("Main2021_FM25", "16-18TRN2021", 2, "19+ Traineeships (Adult funded)", "ILR_FM25")]

        [InlineData("Main2021_ALB", "ALLBC2021", 2, "Advanced Learner Loans Bursary", "ILR_ALB")]
        [InlineData("Main2021_ALB", "ALLBC2021", 3, "Authorised Claims: Advanced Learner Loans Bursary", "EAS")]
        [InlineData("Main2021_ALB", "ALLBC2021", 3, "Advanced Learner Loans Bursary", "ILR_ALB")]
        [InlineData("Main2021_ALB", "ALLB2021", 2, "Advanced Learner Loans Bursary", "ILR_ALB")]
        [InlineData("Main2021_ALB", "ALLB2021", 3, "Authorised Claims: Advanced Learner Loans Bursary", "EAS")]
        [InlineData("Main2021_ALB", "ALLB2021", 3, "Advanced Learner Loans Bursary", "ILR_ALB")]

        [InlineData("Main2021_TBL", "APPS2021", 5, "Authorised Claims: 16-18 Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main2021_TBL", "APPS2021", 5, "16-18 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main2021_TBL", "APPS2021", 6, "Excess Learning Support: 16-18 Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main2021_TBL", "APPS2021", 6, "16-18 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main2021_TBL", "APPS2021", 7, "16-18 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main2021_TBL", "APPS2021", 16, "Authorised Claims: 19-23 Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main2021_TBL", "APPS2021", 16, "19-23 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main2021_TBL", "APPS2021", 17, "Excess Learning Support: 19-23 Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main2021_TBL", "APPS2021", 17, "19-23 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main2021_TBL", "APPS2021", 18, "19-23 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main2021_TBL", "APPS2021", 19, "Authorised Claims: 24+ Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main2021_TBL", "APPS2021", 19, "24+ Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main2021_TBL", "APPS2021", 20, "Excess Learning Support: 24+ Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main2021_TBL", "APPS2021", 20, "24+ Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main2021_TBL", "APPS2021", 21, "24+ Trailblazer Apprenticeship", "ILR_TBL")]
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
