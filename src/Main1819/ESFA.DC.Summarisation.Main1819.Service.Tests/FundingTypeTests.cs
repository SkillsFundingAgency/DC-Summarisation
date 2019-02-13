using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.Summarisation.Main1819.Service.Tests
{
    public class FundingTypeTests 
    {
        private IJsonSerializationService jsonSerializationService;

        public FundingTypeTests()
        {
            jsonSerializationService = new JsonSerializationService();
        }

        [Fact]
        public void FundingTypesCount()
        {

            FundingTypesProvider fundingTypesProvider = new FundingTypesProvider(jsonSerializationService);

            var count = fundingTypesProvider.Provide().ToList().Count();

            Assert.Equal(10, count);
        }

        [Theory]
        [InlineData("ProgFundingFM35andEAS", "APPS1819", 2, "Authorised Claims: 16-18 Apprenticeships", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "APPS1819", 2, "Audit Adjustments: 16-18 Apprenticeships", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "APPS1819", 2, "16-18 Apprenticeship", "ILR_FM35")]

        [InlineData("ProgFundingFM35andEAS", "APPS1819", 11, "Authorised Claims: 19-23 Apprenticeships", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "APPS1819", 11, "Audit Adjustments: 19-23 Apprenticeships", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "APPS1819", 11, "19-23 Apprenticeship", "ILR_FM35")]

        [InlineData("ProgFundingFM35andEAS", "APPS1819", 14, "Authorised Claims: 24+ Apprenticeships", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "APPS1819", 14, "Audit Adjustments: 24+ Apprenticeships", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "APPS1819", 14, "24+ Apprenticeship", "ILR_FM35")]

        [InlineData("ProgFundingFM35andEAS", "AEBC1819", 2, "Authorised Claims: 19-24 Traineeships", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "AEBC1819", 2, "Audit Adjustments: 19-24 Traineeships", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "AEBC1819", 2, "19-24 Traineeship", "ILR_FM35")]
        [InlineData("ProgFundingFM35andEAS", "AEBC1819", 2, "19-24 Traineeship (non-procured)", "ILR_FM35")]

        [InlineData("ProgFundingFM35andEAS", "AEBC1819", 5, "Audit Adjustments: AEB-Other Learning", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "AEBC1819", 5, "Authorised Claims: AEB-Other Learning", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "AEBC1819", 5, "Princes Trust: AEB-Other Learning", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "AEBC1819", 5, "AEB - Other Learning", "ILR_FM35")]
        [InlineData("ProgFundingFM35andEAS", "AEBC1819", 5, "AEB - Other Learning (non-procured)", "ILR_FM35")]

        [InlineData("ProgFundingFM35andEAS", "AEB-TOL1819", 2, "Authorised Claims: 19-24 Traineeships (From Nov 2017)", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "AEB-TOL1819", 2, "Audit Adjustments: 19-24 Traineeships (From Nov 2017)", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "AEB-TOL1819", 2, "19-24 Traineeship (procured from Nov 2017)", "ILR_FM35")]

        [InlineData("ProgFundingFM35andEAS", "AEB-TOL1819", 5, "Audit Adjustments: AEB-Other Learning (From Nov 2017)", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "AEB-TOL1819", 5, "Authorised Claims: AEB-Other Learning (From Nov 2017)", "EAS")]
        [InlineData("ProgFundingFM35andEAS", "AEB-TOL1819", 5, "AEB - Other Learning (procured from Nov 2017)", "ILR_FM35")]
        [InlineData("ProgFundingFM35andEAS", "AEB-TOL1819", 5, "Princes Trust: AEB-Other Learning (From Nov 2017)", "EAS")]
        public void FundLineExist_True(string fundingTypeKey, string fspCode, int dlc, string fundLine, string lineType)
        {

            FundingTypesProvider fundingTypesProvider = NewProvider();

            var fundingTypes = fundingTypesProvider.Provide();

            fundingTypes.Should().Contain(ft => ft.Key == fundingTypeKey);

            var fundingStreams = fundingTypes.First(ft => ft.Key == fundingTypeKey).FundingStreams;

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
