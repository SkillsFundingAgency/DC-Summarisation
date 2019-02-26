﻿using ESFA.DC.Summarisation.Data.Input.Model;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Interfaces;
using Xunit;
using FluentAssertions;
using ESFA.DC.Summarisation.Main1819.Service.Providers;
using ESFA.DC.Summarisation.Configuration;

namespace ESFA.DC.Summarisation.Main1819.Service.Tests
{
    public class SummarisationServiceTests
    {
        [Fact]
        public void SummariseByPeriods()
        {
            var task = new SummarisationService();

            var result = task.SummariseByPeriods(GetPeriodsData(5));

            result.Count().Should().Be(12);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(5 * item.Period * 10);
            }

        }

        [Fact]
        public void SummariseByAttributes()
        {
            HashSet<string> attributesInterested = new HashSet<string> { "AchievePayment","BalancePayment" };

            var task = new SummarisationService();

            var result = task.SummariseByAttribute(GetPeriodisedData(5),attributesInterested);

            result.Count().Should().Be(12);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(5 * 2 * item.Period * 10);
            }

        }

        [Theory]
        [InlineData("APPS1819", 2)]
        [InlineData("APPS1819", 11)]
        [InlineData("APPS1819", 14)]
        [InlineData("AEBC1819", 2)]
        [InlineData("AEBC1819", 5)]
        [InlineData("AEB-TOL1819", 2)]
        [InlineData("AEB-TOL1819", 5)]
        [InlineData("APPS1819", 3)]
        [InlineData("APPS1819", 12)]
        [InlineData("APPS1819", 15)]
        [InlineData("AEBC1819", 3)]
        [InlineData("AEBC1819", 6)]
        [InlineData("AEB-TOL1819", 3)]
        [InlineData("AEB-TOL1819", 6)]
        [InlineData("16-18TRN1819", 3)]
        [InlineData("APPS1819", 4)]
        [InlineData("APPS1819", 13)]
        [InlineData("AEBC1819", 4)]
        [InlineData("AEB-TOL1819", 4)]
        [InlineData("ALLB1819", 4)]
        [InlineData("ALLBC1819", 4)]
        [InlineData("16-18TRN1819", 2)]
        [InlineData("ALLB1819", 3)]
        [InlineData("ALLBC1819", 3)]
        [InlineData("CLP1819", 1)]
        [InlineData("ALLB1819", 2)]
        [InlineData("ALLBC1819", 2)]
        [InlineData("APPS1819", 5)]
        [InlineData("APPS1819", 7)]
        [InlineData("APPS1819", 16)]
        [InlineData("APPS1819", 18)]
        [InlineData("APPS1819", 19)]
        [InlineData("APPS1819", 21)]
        [InlineData("APPS1819", 6)]
        [InlineData("APPS1819", 17)]
        [InlineData("APPS1819", 20)]
        public void SummariseByFundingStream(string fspCode, int dlc)
        {
            var fungingTypes = GetFundingTypes();

            FundingStream fundingStream = fungingTypes.SelectMany(ft => ft.FundingStreams).Where(fs => fs.PeriodCode == fspCode && fs.DeliverableLineCode == dlc).First();

            var easfundlinesCount = fundingStream.FundLines.Where(fl => fl.LineType =="EAS").Count();

            var ilrfundlinesCount = fundingStream.FundLines.Where(fl => fl.LineType != "EAS").Count();

            int attributescount = 0;

            if (fundingStream.FundLines.Where(flW => flW.LineType != "EAS").Select(fl => fl.Attributes).Any())
            {
                attributescount = fundingStream.FundLines.Where(flW => flW.LineType != "EAS").Select(fl => fl.Attributes).First().Count();
            }

            var task = new SummarisationService();

            var results = task.SummariseByFundingStream(fundingStream, GetTestProvider()).OrderBy(x=>x.Period).ToList();

            results.Count().Should().Be(12);

            int i = 1;

            foreach (var item in results)
            {
                item.ActualValue.Should().Be((2 * ilrfundlinesCount * attributescount * i * 10) + (2 * easfundlinesCount * i * 10 ));

                i++;
            }
        }

        [Theory]
        [InlineData("ProgFundingFM35andEAS")]
        [InlineData("LearningSupportFM35andEAS")]
        [InlineData("EASOnlydeliverables")]
        [InlineData("ProgFundingFM25andEAS")]
        [InlineData("LoansBursaryProgFunding")]
        [InlineData("LoansBursarySupportFunding")]
        [InlineData("ProgFundingTrailblazer")]
        [InlineData("LearningSupportTrailblazer")]
        [InlineData("ProgFundingTrailblazerILR")]
        [InlineData("LoansBursaryProgFundingCLP")]
        public void SummariseByFundingType(string key)
        {
            var task = new SummarisationService();

            var fundingType = GetFundingType(key);

            var results = task.Summarise(fundingType, GetTestProvider());

            Dictionary<string, int> fspdlc = new Dictionary<string, int>();

            var fundingStreams = fundingType.FundingStreams.Select(fs => new {fs.PeriodCode, fs.DeliverableLineCode }).ToList();

            foreach (var item in fundingStreams)
            {
                results.Where(r => r.FundingStreamPeriodCode == item.PeriodCode && r.DeliverableCode == item.DeliverableLineCode).Count().Should().Be(12);

                SummariseByFundingStream(item.PeriodCode, item.DeliverableLineCode);
            }
            
        }

        [Theory]
        [InlineData("ProgFundingFM35andEAS")]
        [InlineData("LearningSupportFM35andEAS")]
        [InlineData("EASOnlydeliverables")]
        [InlineData("ProgFundingFM25andEAS")]
        [InlineData("LoansBursaryProgFunding")]
        [InlineData("LoansBursarySupportFunding")]
        [InlineData("ProgFundingTrailblazer")]
        [InlineData("LearningSupportTrailblazer")]
        [InlineData("ProgFundingTrailblazerILR")]
        [InlineData("LoansBursaryProgFundingCLP")]
        public void PerformanceTest(string key)
        {
            Provider provider = GetTestProvider();

            for (int i = 0; i < 17; i++)
            {
                provider.LearningDeliveries.AddRange(provider.LearningDeliveries);
            }

            FundingType fundingType = GetFundingType(key);

            var task = new SummarisationService();

            var results = task.Summarise(fundingType, provider).ToList();

            var fundingStreams = fundingType.FundingStreams.Select(fs => new { fs.PeriodCode, fs.DeliverableLineCode }).ToList();

            foreach (var item in fundingStreams)
            {
                results.Where(r => r.FundingStreamPeriodCode == item.PeriodCode && r.DeliverableCode == item.DeliverableLineCode).Count().Should().Be(12);
            }
        }

        private FundingTypesProvider NewFundingTypeProvider()
        {
            return new FundingTypesProvider(new JsonSerializationService());
        }

        private Provider GetTestProvider()
        {
            return new Provider()
            {
                UKPRN = 10000001,
                LearningDeliveries = GetLearningDeliveries()
            };
        }
        private List<LearningDelivery> GetLearningDeliveries()
        {
            List<LearningDelivery> learningDeliveries = new List<LearningDelivery>();

            foreach (var item in GetFundLines())
            {
                for (int i = 1; i <= 2; i++)
                {
                    LearningDelivery learningDelivery = new LearningDelivery()
                    {
                        LearnRefNumber = "learnref" + i,
                        AimSeqNumber = i,
                        Fundline = item.Fundline,
                        PeriodisedData = item.LineType == "EAS" ? GetPeriodisedDataNoAttributes(1) :  GetPeriodisedData(1)
                    };

                    learningDeliveries.Add(learningDelivery);
                }
            }

            return learningDeliveries;
        }


        private List<PeriodisedData> GetPeriodisedData(int lotSize)
        {
            HashSet<string> attributes = GetAllAttributes();

            List<PeriodisedData> periodisedDatas = new List<PeriodisedData>();

            for (int j = 1; j <= lotSize; j++)
            {
                foreach (var item in attributes)
                {
                    PeriodisedData periodisedData = new PeriodisedData()
                    {
                        AttributeName = item,
                        Periods = GetPeriodsData(1)
                    };

                    periodisedDatas.Add(periodisedData);
                }
            }

            return periodisedDatas;
        }

        private List<PeriodisedData> GetPeriodisedDataNoAttributes(int lotSize)
        {
            List<PeriodisedData> periodisedDatas = new List<PeriodisedData>();

            for (int j = 1; j <= lotSize; j++)
            {
                PeriodisedData periodisedData = new PeriodisedData()
                {
                    Periods = GetPeriodsData(1)
                };

                periodisedDatas.Add(periodisedData);
            }

            return periodisedDatas;
        }

        private List<Period> GetPeriodsData(int lotSize)
        {
            List<Period> periods = new List<Period>();
            for (int j = 1; j <= lotSize; j++)
            {
                for (int i = 1; i <= 12; i++)
                {
                    Period period = new Period() { PeriodId = i, Value = i * 10 };
                    periods.Add(period);
                }
            }

            return periods;
        }

        private FundingType GetFundingType(string fundingType)
        {
          return GetFundingTypes().First(x => x.Key == fundingType);

        }

        private List<FundingType> GetFundingTypes()
        {
            FundingTypesProvider fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());

            return fundingTypesProvider.Provide().ToList();

        }

        private HashSet<string> GetAllAttributes()
        {
            return new HashSet<string> { "AchievePayment",
                                        "BalancePayment",
                                        "OnProgPayment",
                                        "EmpOutcomePay",
                                        "LearnSuppFundCash",
                                        "LnrOnProgPay",
                                        "AreaUpliftBalPayment",
                                        "AreaUpliftOnProgPayment",
                                        "ALBSupportPayment",
                                        "CoreGovContPayment",
                                        "MathEngBalPayment",
                                        "MathEngOnProgPayment",
                                        "SmallBusPayment",
                                        "YoungAppPayment",
                                        "AchPayment",
                                        "LearnDelCarLearnPilotOnProgPayment",
                                        "LearnDelCarLearnPilotBalPayment" };
        }
        
        private List<FundLine> GetFundLines()
        {
            List<FundLine> fundLines = new List<FundLine>();

            fundLines.Add(new FundLine() { Fundline = "16-18 Apprenticeship", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "16-18 Trailblazer Apprenticeship", LineType = "ILR_TBL" });
            fundLines.Add(new FundLine() { Fundline = "16-18 Traineeships (Adult funded)", LineType = "ILR_FM25" });
            fundLines.Add(new FundLine() { Fundline = "19+ Traineeships (Adult funded)", LineType = "ILR_FM25" });
            fundLines.Add(new FundLine() { Fundline = "19-23 Apprenticeship", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "19-23 Trailblazer Apprenticeship", LineType = "ILR_TBL" });
            fundLines.Add(new FundLine() { Fundline = "19-24 Traineeship", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "19-24 Traineeship (non-procured)", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "19-24 Traineeship (procured from Nov 2017)", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "24+ Apprenticeship", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "24+ Trailblazer Apprenticeship", LineType = "ILR_TBL" });
            fundLines.Add(new FundLine() { Fundline = "Advanced Learner Loans Bursary", LineType = "ILR_ALB" });
            fundLines.Add(new FundLine() { Fundline = "AEB - Other Learning", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "AEB - Other Learning (non-procured)", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "AEB - Other Learning (procured from Nov 2017)", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 16-18 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 16-18 Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 16-18 Traineeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 19-23 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 19-23 Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 19-24 Traineeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 19-24 Traineeships (From Nov 2017)", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 24+ Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 24+ Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: Advanced Learner Loans Bursary", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: AEB-Other Learning", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: AEB-Other Learning (From Nov 2017)", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 16-18 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 16-18 Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 16-18 Traineeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 19-23 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 19-23 Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 19-24 Traineeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 19-24 Traineeships (From Nov 2017)", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 24+ Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 24+ Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: AEB-Other Learning", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: AEB-Other Learning (From Nov 2017)", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Discretionary Bursary: 16-19 Traineeships Bursary", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 16-18 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 16-18 Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 16-18 Traineeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 19-23 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 19-23 Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 19-24 Traineeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 19-24 Traineeships (From Nov 2017)", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 24+ Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 24+ Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: AEB-Other Learning", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: AEB-Other Learning (From Nov 2017)", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Support: Advanced Learner Loans Bursary", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Free Meals: 16-19 Traineeships Bursary", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Learner Support: 16-18 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Learner Support: 19-23 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Learner Support: 19-24 Traineeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Learner Support: 19-24 Traineeships (From Nov 2017)", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Learner Support: 24+ Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Vulnerable Bursary: 16-19 Traineeships Bursary", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Princes Trust: AEB-Other Learning", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Princes Trust: AEB-Other Learning (From Nov 2017)", LineType = "EAS" });

            return fundLines;
        }

    }

   
}
