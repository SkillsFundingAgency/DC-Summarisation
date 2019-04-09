using System.Linq;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Main1819.Service.Providers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.Main1819.Service.Tests
{
    public class FundingTypeTests
    {
        [Fact]
        public void FundingTypesCount()
        {
            NewProvider().Provide().Should().HaveCount(5);
        }

        [Theory]
        [InlineData("Main1819_FM35", "APPS1819", 2, "Authorised Claims: 16-18 Apprenticeships", "EAS")]
        [InlineData("Main1819_FM35", "APPS1819", 2, "Audit Adjustments: 16-18 Apprenticeships", "EAS")]
        [InlineData("Main1819_FM35", "APPS1819", 2, "16-18 Apprenticeship", "ILR_FM35")]
        [InlineData("Main1819_FM35", "APPS1819", 11, "Authorised Claims: 19-23 Apprenticeships", "EAS")]
        [InlineData("Main1819_FM35", "APPS1819", 11, "Audit Adjustments: 19-23 Apprenticeships", "EAS")]
        [InlineData("Main1819_FM35", "APPS1819", 11, "19-23 Apprenticeship", "ILR_FM35")]
        [InlineData("Main1819_FM35", "APPS1819", 14, "Authorised Claims: 24+ Apprenticeships", "EAS")]
        [InlineData("Main1819_FM35", "APPS1819", 14, "Audit Adjustments: 24+ Apprenticeships", "EAS")]
        [InlineData("Main1819_FM35", "APPS1819", 14, "24+ Apprenticeship", "ILR_FM35")]
        [InlineData("Main1819_FM35", "AEBC1819", 2, "Authorised Claims: 19-24 Traineeships", "EAS")]
        [InlineData("Main1819_FM35", "AEBC1819", 2, "Audit Adjustments: 19-24 Traineeships", "EAS")]
        [InlineData("Main1819_FM35", "AEBC1819", 2, "19-24 Traineeship", "ILR_FM35")]
        [InlineData("Main1819_FM35", "AEBC1819", 2, "19-24 Traineeship (non-procured)", "ILR_FM35")]
        [InlineData("Main1819_FM35", "AEBC1819", 5, "Audit Adjustments: AEB-Other Learning", "EAS")]
        [InlineData("Main1819_FM35", "AEBC1819", 5, "Authorised Claims: AEB-Other Learning", "EAS")]
        [InlineData("Main1819_FM35", "AEBC1819", 5, "Princes Trust: AEB-Other Learning", "EAS")]
        [InlineData("Main1819_FM35", "AEBC1819", 5, "AEB - Other Learning", "ILR_FM35")]
        [InlineData("Main1819_FM35", "AEBC1819", 5, "AEB - Other Learning (non-procured)", "ILR_FM35")]
        [InlineData("Main1819_FM35", "AEB-TOL1819", 2, "Authorised Claims: 19-24 Traineeships (From Nov 2017)", "EAS")]
        [InlineData("Main1819_FM35", "AEB-TOL1819", 2, "Audit Adjustments: 19-24 Traineeships (From Nov 2017)", "EAS")]
        [InlineData("Main1819_FM35", "AEB-TOL1819", 2, "19-24 Traineeship (procured from Nov 2017)", "ILR_FM35")]
        [InlineData("Main1819_FM35", "AEB-TOL1819", 5, "Audit Adjustments: AEB-Other Learning (From Nov 2017)", "EAS")]
        [InlineData("Main1819_FM35", "AEB-TOL1819", 5, "Authorised Claims: AEB-Other Learning (From Nov 2017)", "EAS")]
        [InlineData("Main1819_FM35", "AEB-TOL1819", 5, "AEB - Other Learning (procured from Nov 2017)", "ILR_FM35")]
        [InlineData("Main1819_FM35", "AEB-TOL1819", 5, "Princes Trust: AEB-Other Learning (From Nov 2017)", "EAS")]

        [InlineData("Main1819_FM35", "APPS1819", 3, "Excess Learning Support: 16-18 Apprenticeships", "EAS")]
        [InlineData("Main1819_FM35", "APPS1819", 3, "16-18 Apprenticeship", "ILR_FM35")]
        [InlineData("Main1819_FM35", "APPS1819", 12, "Excess Learning Support: 19-23 Apprenticeships", "EAS")]
        [InlineData("Main1819_FM35", "APPS1819", 12, "19-23 Apprenticeship", "ILR_FM35")]
        [InlineData("Main1819_FM35", "APPS1819", 15, "Excess Learning Support: 24+ Apprenticeships", "EAS")]
        [InlineData("Main1819_FM35", "APPS1819", 15, "24+ Apprenticeship", "ILR_FM35")]
        [InlineData("Main1819_FM35", "AEBC1819", 3, "Excess Learning Support: 19-24 Traineeships", "EAS")]
        [InlineData("Main1819_FM35", "AEBC1819", 3, "19-24 Traineeship", "ILR_FM35")]
        [InlineData("Main1819_FM35", "AEBC1819", 3, "19-24 Traineeship (non-procured)", "ILR_FM35")]
        [InlineData("Main1819_FM35", "AEBC1819", 6, "Excess Learning Support: AEB-Other Learning", "EAS")]
        [InlineData("Main1819_FM35", "AEBC1819", 6, "AEB - Other Learning", "ILR_FM35")]
        [InlineData("Main1819_FM35", "AEBC1819", 6, "AEB - Other Learning (non-procured)", "ILR_FM35")]
        [InlineData("Main1819_FM35", "AEB-TOL1819", 3, "Excess Learning Support: 19-24 Traineeships (From Nov 2017)", "EAS")]
        [InlineData("Main1819_FM35", "AEB-TOL1819", 3, "19-24 Traineeship (procured from Nov 2017)", "ILR_FM35")]
        [InlineData("Main1819_FM35", "AEB-TOL1819", 6, "Excess Learning Support: AEB-Other Learning (From Nov 2017)", "EAS")]
        [InlineData("Main1819_FM35", "AEB-TOL1819", 6, "AEB - Other Learning (procured from Nov 2017)", "ILR_FM35")]

        [InlineData("Main1819_EAS", "16-18TRN1819", 3, "Vulnerable Bursary: 16-19 Traineeships Bursary", "EAS")]
        [InlineData("Main1819_EAS", "16-18TRN1819", 3, "Free Meals: 16-19 Traineeships Bursary", "EAS")]
        [InlineData("Main1819_EAS", "16-18TRN1819", 3, "Discretionary Bursary: 16-19 Traineeships Bursary", "EAS")]
        [InlineData("Main1819_EAS", "APPS1819", 4, "Learner Support: 16-18 Apprenticeships", "EAS")]
        [InlineData("Main1819_EAS", "APPS1819", 13, "Learner Support: 24+ Apprenticeships", "EAS")]
        [InlineData("Main1819_EAS", "APPS1819", 13, "Learner Support: 19-23 Apprenticeships", "EAS")]
        [InlineData("Main1819_EAS", "AEBC1819", 4, "Learner Support: 19-24 Traineeships", "EAS")]
        [InlineData("Main1819_EAS", "AEB-TOL1819", 4, "Learner Support: 19-24 Traineeships (From Nov 2017)", "EAS")]
        [InlineData("Main1819_EAS", "ALLB1819", 4, "Excess Support: Advanced Learner Loans Bursary", "EAS")]
        [InlineData("Main1819_EAS", "ALLBC1819", 4, "Excess Support: Advanced Learner Loans Bursary", "EAS")]

        [InlineData("Main1819_FM25", "16-18TRN1819", 2, "Authorised Claims: 16-18 Traineeships", "EAS")]
        [InlineData("Main1819_FM25", "16-18TRN1819", 2, "Audit Adjustments: 16-18 Traineeships", "EAS")]
        [InlineData("Main1819_FM25", "16-18TRN1819", 2, "Excess Learning Support: 16-18 Traineeships", "EAS")]
        [InlineData("Main1819_FM25", "16-18TRN1819", 2, "16-18 Traineeships (Adult funded)", "ILR_FM25")]
        [InlineData("Main1819_FM25", "16-18TRN1819", 2, "19+ Traineeships (Adult funded)", "ILR_FM25")]

        [InlineData("Main1819_ALB", "ALLB1819", 3, "Authorised Claims: Advanced Learner Loans Bursary", "EAS")]
        [InlineData("Main1819_ALB", "ALLB1819", 3, "Advanced Learner Loans Bursary", "ILR_ALB")]
        [InlineData("Main1819_ALB", "ALLBC1819", 3, "Authorised Claims: Advanced Learner Loans Bursary", "EAS")]
        [InlineData("Main1819_ALB", "ALLBC1819", 3, "Advanced Learner Loans Bursary", "ILR_ALB")]

        [InlineData("Main1819_ALB", "ALLB1819", 2, "Advanced Learner Loans Bursary", "ILR_ALB")]
        [InlineData("Main1819_ALB", "ALLBC1819", 2, "Advanced Learner Loans Bursary", "ILR_ALB")]

        [InlineData("Main1819_TBL", "APPS1819", 5, "Audit Adjustments: 16-18 Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1819_TBL", "APPS1819", 5, "Authorised Claims: 16-18 Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1819_TBL", "APPS1819", 5, "16-18 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main1819_TBL", "APPS1819", 16, "Audit Adjustments: 19-23 Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1819_TBL", "APPS1819", 16, "Authorised Claims: 19-23 Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1819_TBL", "APPS1819", 16, "19-23 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main1819_TBL", "APPS1819", 19, "Audit Adjustments: 24+ Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1819_TBL", "APPS1819", 19, "Authorised Claims: 24+ Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1819_TBL", "APPS1819", 19, "24+ Trailblazer Apprenticeship", "ILR_TBL")]

        [InlineData("Main1819_TBL", "APPS1819", 6, "Excess Learning Support: 16-18 Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1819_TBL", "APPS1819", 6, "16-18 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main1819_TBL", "APPS1819", 17, "Excess Learning Support: 19-23 Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1819_TBL", "APPS1819", 17, "19-23 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main1819_TBL", "APPS1819", 20, "Excess Learning Support: 24+ Trailblazer Apprenticeships", "EAS")]
        [InlineData("Main1819_TBL", "APPS1819", 20, "24+ Trailblazer Apprenticeship", "ILR_TBL")]

        [InlineData("Main1819_TBL", "APPS1819", 7, "16-18 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main1819_TBL", "APPS1819", 18, "19-23 Trailblazer Apprenticeship", "ILR_TBL")]
        [InlineData("Main1819_TBL", "APPS1819", 21, "24+ Trailblazer Apprenticeship", "ILR_TBL")]

        [InlineData("Main1819_ALB", "CLP1819", 1, "Advanced Learner Loans Bursary", "ILR_ALB")]
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

        private FundingTypesProvider NewProvider()
        {
            return new FundingTypesProvider(new JsonSerializationService());
        }
    }
}
