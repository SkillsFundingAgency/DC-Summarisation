using ESFA.DC.Summarisation.Data.Input.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Interfaces;
using Xunit;
using FluentAssertions;
using ESFA.DC.Summarisation.Main1819.Service.Tasks;
using ESFA.DC.Summarisation.Main1819.Service.Providers;
using ESFA.DC.Summarisation.Configuration;

namespace ESFA.DC.Summarisation.Main1819.Service.Tests
{
    public class SummarisationTaskTests
    {
        [Fact]
        public void SummariseByPeriods()
        {
            var task = new SummarisationTask();

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

            var task = new SummarisationTask();

            var result = task.SummariseByAttribute(GetPeriodisedData(5),attributesInterested);

            result.Count().Should().Be(12);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(5 * 2 * item.Period * 10);
            }

        }

        [Fact]
        public void SummariseByFundingStream()
        {
            FundingStream fundingStream = GetFundingType("ProgFundingFM35andEAS").FundingStreams.Where(fs => fs.PeriodCode == "APPS1819" && fs.DeliverableLineCode == 2).First();

            var task = new SummarisationTask();

            var results = task.SummariseByFundingStream(fundingStream, GetTestProvider());
        }

        [Fact]
        public void SummariseByFundingType()
        {
            var task = new SummarisationTask();

            var results = task.Summarise(GetFundingType("ProgFundingFM35andEAS"), GetTestProvider());
        }

        [Fact]
        public void TestSummarisatonTask()
        {
            Provider provider = GetData().First(x => x.UKPRN == 10023139);
            FundingType fundingType = GetFundingType("ProgFundingFM35andEAS");

            var task = new SummarisationTask();

            var results = task.Summarise(fundingType, provider);

            results.Count().Should().Be(12);
        }

        [Fact]
        public void PerformanceTest()
        {
            Provider provider = GetData().First(x => x.UKPRN == 10023139);

            for (int i = 0; i < 18; i++)
            {
                provider.LearningDeliveries.AddRange(provider.LearningDeliveries);
            }
            
            FundingType fundingType = GetFundingType("ProgFundingFM35andEAS");

            var task = new SummarisationTask();

            var results = task.Summarise(fundingType, provider);

        }

        [Fact]
        public void GetDataTest()
        {
            var result = GetData();

        }

        private  List<Provider> GetData()
        {
           string TestDataFileName  = "ESFA.DC.Summarisation.Main1819.Service.Tests.JsonFiles.InputTestData.json";
           IJsonSerializationService _jsonSerializationService = new JsonSerializationService();

            List<Provider> result;

            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(TestDataFileName))
            {
                return result = _jsonSerializationService.Deserialize<List<Provider>>(stream);
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
                        PeriodisedData = item.LineType == "EAS" ? GetPeriodisedDataNoAttributes() :  GetPeriodisedData(1)
                    };

                    learningDeliveries.Add(learningDelivery);
                }
            }

            return learningDeliveries;
        }


        private List<PeriodisedData> GetPeriodisedData(int lot)
        {
            HashSet<string> attributes = GetAllAttributes();

            List<PeriodisedData> periodisedDatas = new List<PeriodisedData>();

            for (int j = 1; j <= lot; j++)
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

        private List<PeriodisedData> GetPeriodisedDataNoAttributes()
        {
            List<PeriodisedData> periodisedDatas = new List<PeriodisedData>();

            for (int j = 1; j <= 5; j++)
            {
                PeriodisedData periodisedData = new PeriodisedData()
                {
                    Periods = GetPeriodsData(1)
                };

                periodisedDatas.Add(periodisedData);
            }

            return periodisedDatas;
        }

        private List<Period> GetPeriodsData(int lots)
        {
            List<Period> periods = new List<Period>();
            for (int j = 1; j <= lots; j++)
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
            FundingTypesProvider fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());

            return fundingTypesProvider.Provide().First(x => x.Key == fundingType);
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
        /*
        private HashSet<string> GetILRFundlines()
        {
            return new HashSet<string> {"16-18 Apprenticeship",
                                        "16-18 Trailblazer Apprenticeship",
                                        "16-18 Traineeships (Adult funded)",
                                        "19+ Traineeships (Adult funded)",
                                        "19-23 Apprenticeship",
                                        "19-23 Trailblazer Apprenticeship",
                                        "19-24 Traineeship",
                                        "19-24 Traineeship (non-procured)",
                                        "19-24 Traineeship (procured from Nov 2017)",
                                        "24+ Apprenticeship",
                                        "24+ Trailblazer Apprenticeship",
                                        "Advanced Learner Loans Bursary",
                                        "AEB - Other Learning",
                                        "AEB - Other Learning (non-procured)",
                                        "AEB - Other Learning (procured from Nov 2017)" };
        }
        
        private HashSet<string> GetEASFundlines()
        {
            return new HashSet<string> {"Audit Adjustments: 16-18 Apprenticeships",
                                        "Audit Adjustments: 16-18 Trailblazer Apprenticeships",
                                        "Audit Adjustments: 16-18 Traineeships",
                                        "Audit Adjustments: 19-23 Apprenticeships",
                                        "Audit Adjustments: 19-23 Trailblazer Apprenticeships",
                                        "Audit Adjustments: 19-24 Traineeships",
                                        "Audit Adjustments: 19-24 Traineeships (From Nov 2017)",
                                        "Audit Adjustments: 24+ Apprenticeships",
                                        "Audit Adjustments: 24+ Trailblazer Apprenticeships",
                                        "Authorised Claims: Advanced Learner Loans Bursary",
                                        "Audit Adjustments: AEB-Other Learning",
                                        "Audit Adjustments: AEB-Other Learning (From Nov 2017)",
                                        "Authorised Claims: 16-18 Apprenticeships",
                                        "Authorised Claims: 16-18 Trailblazer Apprenticeships",
                                        "Authorised Claims: 16-18 Traineeships",
                                        "Authorised Claims: 19-23 Apprenticeships",
                                        "Authorised Claims: 19-23 Trailblazer Apprenticeships",
                                        "Authorised Claims: 19-24 Traineeships",
                                        "Authorised Claims: 19-24 Traineeships (From Nov 2017)",
                                        "Authorised Claims: 24+ Apprenticeships",
                                        "Authorised Claims: 24+ Trailblazer Apprenticeships",
                                        "Authorised Claims: AEB-Other Learning",
                                        "Authorised Claims: AEB-Other Learning (From Nov 2017)",
                                        "Discretionary Bursary: 16-19 Traineeships Bursary",
                                        "Excess Learning Support: 16-18 Apprenticeships",
                                        "Excess Learning Support: 16-18 Trailblazer Apprenticeships",
                                        "Excess Learning Support: 16-18 Traineeships",
                                        "Excess Learning Support: 19-23 Apprenticeships",
                                        "Excess Learning Support: 19-23 Trailblazer Apprenticeships",
                                        "Excess Learning Support: 19-24 Traineeships",
                                        "Excess Learning Support: 19-24 Traineeships (From Nov 2017)",
                                        "Excess Learning Support: 24+ Apprenticeships",
                                        "Excess Learning Support: 24+ Trailblazer Apprenticeships",
                                        "Excess Learning Support: AEB-Other Learning",
                                        "Excess Learning Support: AEB-Other Learning (From Nov 2017)",
                                        "Excess Support: Advanced Learner Loans Bursary",
                                        "Free Meals: 16-19 Traineeships Bursary",
                                        "Learner Support: 16-18 Apprenticeships",
                                        "Learner Support: 19-23 Apprenticeships",
                                        "Learner Support: 19-24 Traineeships",
                                        "Learner Support: 19-24 Traineeships (From Nov 2017)",
                                        "Learner Support: 24+ Apprenticeships",
                                        "Vulnerable Bursary: 16-19 Traineeships Bursary",
                                        "Princes Trust: AEB-Other Learning",
                                        "Princes Trust: AEB-Other Learning (From Nov 2017)" };
        }
        */


    }

   
}
