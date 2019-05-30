using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Model;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Main1819.Service.Providers;
using ESFA.DC.Summarisation.Service;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.Summarisation.Main1819.Service.Tests
{
    public class SummarisationServiceTests
    {
        private const int learningDeliveryRecords = 2;

        private const decimal periodValue = 10;

        private const int ukprn = 10000001;

        [Fact]
        public void SummariseByPeriods()
        {
            var task = new SummarisationService();

            var result = task.SummarisePeriods(GetPeriodsData(5));

            result.Count().Should().Be(12);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(5 * item.Period * periodValue);
            }
        }

        [Theory]
        [InlineData("16-18 Apprenticeship")]
        [InlineData("16-18 Trailblazer Apprenticeship")]
        [InlineData("16-18 Traineeships (Adult funded)")]
        [InlineData("19+ Traineeships (Adult funded)")]
        [InlineData("19-23 Apprenticeship")]
        [InlineData("19-23 Trailblazer Apprenticeship")]
        [InlineData("19-24 Traineeship (non-procured)")]
        [InlineData("19-24 Traineeship (procured from Nov 2017)")]
        [InlineData("19-24 Traineeship")]
        [InlineData("24+ Apprenticeship")]
        [InlineData("24+ Trailblazer Apprenticeship")]
        [InlineData("Advanced Learner Loans Bursary")]
        [InlineData("AEB - Other Learning (non-procured)")]
        [InlineData("AEB - Other Learning (procured from Nov 2017)")]
        [InlineData("AEB - Other Learning")]
        [InlineData("Audit Adjustments: 16-18 Apprenticeships")]
        [InlineData("Audit Adjustments: 16-18 Trailblazer Apprenticeships")]
        [InlineData("Audit Adjustments: 16-18 Traineeships")]
        [InlineData("Audit Adjustments: 19-23 Apprenticeships")]
        [InlineData("Audit Adjustments: 19-23 Trailblazer Apprenticeships")]
        [InlineData("Audit Adjustments: 19-24 Traineeships (From Nov 2017)")]
        [InlineData("Audit Adjustments: 19-24 Traineeships")]
        [InlineData("Audit Adjustments: 24+ Apprenticeships")]
        [InlineData("Audit Adjustments: 24+ Trailblazer Apprenticeships")]
        [InlineData("Audit Adjustments: AEB-Other Learning (From Nov 2017)")]
        [InlineData("Audit Adjustments: AEB-Other Learning")]
        [InlineData("Authorised Claims: 16-18 Apprenticeships")]
        [InlineData("Authorised Claims: 16-18 Trailblazer Apprenticeships")]
        [InlineData("Authorised Claims: 16-18 Traineeships")]
        [InlineData("Authorised Claims: 19-23 Apprenticeships")]
        [InlineData("Authorised Claims: 19-23 Trailblazer Apprenticeships")]
        [InlineData("Authorised Claims: 19-24 Traineeships (From Nov 2017)")]
        [InlineData("Authorised Claims: 19-24 Traineeships")]
        [InlineData("Authorised Claims: 24+ Apprenticeships")]
        [InlineData("Authorised Claims: 24+ Trailblazer Apprenticeships")]
        [InlineData("Authorised Claims: Advanced Learner Loans Bursary")]
        [InlineData("Authorised Claims: AEB-Other Learning (From Nov 2017)")]
        [InlineData("Authorised Claims: AEB-Other Learning")]
        [InlineData("Discretionary Bursary: 16-19 Traineeships Bursary")]
        [InlineData("Excess Learning Support: 16-18 Apprenticeships")]
        [InlineData("Excess Learning Support: 16-18 Trailblazer Apprenticeships")]
        [InlineData("Excess Learning Support: 16-18 Traineeships")]
        [InlineData("Excess Learning Support: 19-23 Apprenticeships")]
        [InlineData("Excess Learning Support: 19-23 Trailblazer Apprenticeships")]
        [InlineData("Excess Learning Support: 19-24 Traineeships (From Nov 2017)")]
        [InlineData("Excess Learning Support: 19-24 Traineeships")]
        [InlineData("Excess Learning Support: 24+ Apprenticeships")]
        [InlineData("Excess Learning Support: 24+ Trailblazer Apprenticeships")]
        [InlineData("Excess Learning Support: AEB-Other Learning (From Nov 2017)")]
        [InlineData("Excess Learning Support: AEB-Other Learning")]
        [InlineData("Excess Support: Advanced Learner Loans Bursary")]
        [InlineData("Free Meals: 16-19 Traineeships Bursary")]
        [InlineData("Learner Support: 16-18 Apprenticeships")]
        [InlineData("Learner Support: 19-23 Apprenticeships")]
        [InlineData("Learner Support: 19-24 Traineeships (From Nov 2017)")]
        [InlineData("Learner Support: 19-24 Traineeships")]
        [InlineData("Learner Support: 24+ Apprenticeships")]
        [InlineData("Princes Trust: AEB-Other Learning (From Nov 2017)")]
        [InlineData("Princes Trust: AEB-Other Learning")]
        [InlineData("Vulnerable Bursary: 16-19 Traineeships Bursary")]
        public void GetPeriodsForFundLine(string strFundLine)
        {
            var fundLine = GetFundingTypes()
                                  .SelectMany(ft => ft.FundingStreams)
                                  .SelectMany(fs => fs.FundLines)
                                  .Where(fl => fl.Fundline == strFundLine).First();

            var task = new SummarisationService();

            var result = task.GetPeriodsForFundLine(fundLine.UseAttributes ? GetPeriodisedData(5) : GetPeriodisedDataNoAttributes(5), fundLine);

            int attributeCount = 1;

            if (fundLine.UseAttributes)
            {
                attributeCount = fundLine.Attributes.Count();
            }

            result.Count().Should().Be(5 * attributeCount * 12);
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

            var easfundlinesCount = fundingStream.FundLines.Count(fl => fl.LineType == "EAS");

            var ilrfundlinesCount = fundingStream.FundLines.Count(fl => fl.LineType != "EAS");

            int attributescount = 0;

            if (fundingStream.FundLines.Where(flW => flW.LineType != "EAS").Select(fl => fl.Attributes).Any())
            {
                attributescount = fundingStream.FundLines.Where(flW => flW.LineType != "EAS").Select(fl => fl.Attributes).First().Count();
            }

            var task = new SummarisationService();

            var results = task.Summarise(fundingStream, GetTestProvider(), GetContractAllocation(), GetCollectionPeriods()).OrderBy(x => x.Period).ToList();

            results.Count().Should().Be(12);

            int i = 1;

            foreach (var item in results)
            {
                item.ActualValue.Should().Be((learningDeliveryRecords * ilrfundlinesCount * attributescount * i * periodValue) + (learningDeliveryRecords * easfundlinesCount * i * periodValue));

                i++;
            }
        }

        [Theory]
        [InlineData("16-18 Traineeships (Adult funded)", "16-18TRN1819")]
        [InlineData("16-18 Traineeships (Adult Funded)", "16-18tRN1819")]
        [InlineData("16-18 traineeships (adult funded)", "16-18trn1819")]
        [InlineData("16-18 TRAINEESHIPS (ADULT FUNDED)", "16-18TRN1819")]
        public void SummariseByFundingStream_CaseInsensitiveCheck(string fundLine, string fundingStreamPeriodCode)
        {
            var fungingTypes = GetFundingTypes();

            FundingStream fundingStream = fungingTypes.SelectMany(ft => ft.FundingStreams).Where(fs => fs.PeriodCode == "16-18TRN1819" && fs.DeliverableLineCode == 2).First();

            var provider = new Provider()
            {
                UKPRN = ukprn,
                LearningDeliveries = new List<LearningDelivery>()
                {
                    new LearningDelivery()
                    {
                        LearnRefNumber = "100090123",
                        AimSeqNumber = 12345,
                        Fundline = fundLine,
                        PeriodisedData = new List<PeriodisedData>()
                        {
                            new PeriodisedData()
                            {
                                AttributeName = "LnrOnProgPay",
                                DeliverableCode = "2",
                                Periods = new List<Period>()
                                {
                                    new Period()
                                    {
                                        PeriodId = 6,
                                        Value = 144
                                    },
                                    new Period()
                                    {
                                        PeriodId = 7,
                                        Value = 45
                                    }
                                }
                            }
                        }
                    },
                    new LearningDelivery()
                    {
                        LearnRefNumber = "100090123",
                        AimSeqNumber = 12345,
                        Fundline = fundLine,
                        PeriodisedData = new List<PeriodisedData>()
                        {
                            new PeriodisedData()
                            {
                                DeliverableCode = "2",
                                Periods = new List<Period>()
                                {
                                    new Period()
                                    {
                                        PeriodId = 1,
                                        Value = 20
                                    },
                                    new Period()
                                    {
                                        PeriodId = 2,
                                        Value = 0
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var fcsContractAllocations = new List<FcsContractAllocation>()
            {
                new FcsContractAllocation()
                {
                    ContractAllocationNumber = "16-18TRN1819",
                    FundingStreamPeriodCode = fundingStreamPeriodCode,
                    DeliveryUkprn = ukprn,
                    DeliveryOrganisation = "ORG0000031"
                }
            };

            var task = new SummarisationService();

            var results = task.Summarise(fundingStream, provider, fcsContractAllocations, GetCollectionPeriods()).OrderBy(x => x.Period).ToList();

            results.Count().Should().Be(2);
            results.Should().NotBeNullOrEmpty();
        }

        private FundingTypesProvider NewFundingTypeProvider()
        {
            return new FundingTypesProvider(new JsonSerializationService());
        }

        private Provider GetTestProvider()
        {
            return new Provider()
            {
                UKPRN = ukprn,
                LearningDeliveries = GetLearningDeliveries()
            };
        }

        private List<LearningDelivery> GetLearningDeliveries()
        {
            List<LearningDelivery> learningDeliveries = new List<LearningDelivery>();

            foreach (var item in GetFundLines())
            {
                for (int i = 1; i <= learningDeliveryRecords; i++)
                {
                    LearningDelivery learningDelivery = new LearningDelivery()
                    {
                        LearnRefNumber = "learnref" + i,
                        AimSeqNumber = i,
                        Fundline = item.Fundline,
                        PeriodisedData = item.LineType == "EAS" ? GetPeriodisedDataNoAttributes(1) : GetPeriodisedData(1)
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
                    Period period = new Period() { PeriodId = i, Value = i * periodValue };
                    periods.Add(period);
                }
            }

            return periods;
        }

        private List<FundingType> GetFundingTypes()
        {
            FundingTypesProvider fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());

            return fundingTypesProvider.Provide().ToList();
        }

        private List<CollectionPeriod> GetCollectionPeriods()
        {
            var collectionPeriodsProvider = new CollectionPeriodsProvider(new JsonSerializationService());

            return collectionPeriodsProvider.Provide().ToList();
        }

        private IList<FcsContractAllocation> GetContractAllocation()
        {
            IList<FcsContractAllocation> fcsContractAllocations = new List<FcsContractAllocation>();
            var fundingStreams = GetFundingTypes().SelectMany(ft => ft.FundingStreams);

            foreach (var item in fundingStreams)
            {
                FcsContractAllocation allocation = new FcsContractAllocation()
                {
                    ContractAllocationNumber = $"Alloc{item.PeriodCode}",
                    FundingStreamPeriodCode = item.PeriodCode,
                    DeliveryUkprn = ukprn,
                    DeliveryOrganisation = $"Org{ukprn}"
                };
                fcsContractAllocations.Add(allocation);
            }

            return fcsContractAllocations;
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
            List<FundLine> fundLines = new List<FundLine>
            {
                new FundLine { Fundline = "16-18 Apprenticeship", LineType = "ILR_FM35" },
                new FundLine { Fundline = "16-18 Trailblazer Apprenticeship", LineType = "ILR_TBL" },
                new FundLine { Fundline = "16-18 Traineeships (Adult funded)", LineType = "ILR_FM25" },
                new FundLine { Fundline = "19+ Traineeships (Adult funded)", LineType = "ILR_FM25" },
                new FundLine { Fundline = "19-23 Apprenticeship", LineType = "ILR_FM35" },
                new FundLine { Fundline = "19-23 Trailblazer Apprenticeship", LineType = "ILR_TBL" },
                new FundLine { Fundline = "19-24 Traineeship", LineType = "ILR_FM35" },
                new FundLine { Fundline = "19-24 Traineeship (non-procured)", LineType = "ILR_FM35" },
                new FundLine { Fundline = "19-24 Traineeship (procured from Nov 2017)", LineType = "ILR_FM35" },
                new FundLine { Fundline = "24+ Apprenticeship", LineType = "ILR_FM35" },
                new FundLine { Fundline = "24+ Trailblazer Apprenticeship", LineType = "ILR_TBL" },
                new FundLine { Fundline = "Advanced Learner Loans Bursary", LineType = "ILR_ALB" },
                new FundLine { Fundline = "AEB - Other Learning", LineType = "ILR_FM35" },
                new FundLine { Fundline = "AEB - Other Learning (non-procured)", LineType = "ILR_FM35" },
                new FundLine { Fundline = "AEB - Other Learning (procured from Nov 2017)", LineType = "ILR_FM35" },
                new FundLine { Fundline = "Audit Adjustments: 16-18 Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: 16-18 Trailblazer Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: 16-18 Traineeships", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: 19-23 Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: 19-23 Trailblazer Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: 19-24 Traineeships", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: 19-24 Traineeships (From Nov 2017)", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: 24+ Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: 24+ Trailblazer Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: Advanced Learner Loans Bursary", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: AEB-Other Learning", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: AEB-Other Learning (From Nov 2017)", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Trailblazer Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Traineeships", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 19-23 Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 19-23 Trailblazer Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 19-24 Traineeships", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 19-24 Traineeships (From Nov 2017)", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 24+ Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 24+ Trailblazer Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: AEB-Other Learning", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: AEB-Other Learning (From Nov 2017)", LineType = "EAS" },
                new FundLine { Fundline = "Discretionary Bursary: 16-19 Traineeships Bursary", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 16-18 Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 16-18 Trailblazer Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 16-18 Traineeships", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 19-23 Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 19-23 Trailblazer Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 19-24 Traineeships", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 19-24 Traineeships (From Nov 2017)", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 24+ Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 24+ Trailblazer Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: AEB-Other Learning", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: AEB-Other Learning (From Nov 2017)", LineType = "EAS" },
                new FundLine { Fundline = "Excess Support: Advanced Learner Loans Bursary", LineType = "EAS" },
                new FundLine { Fundline = "Free Meals: 16-19 Traineeships Bursary", LineType = "EAS" },
                new FundLine { Fundline = "Learner Support: 16-18 Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Learner Support: 19-23 Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Learner Support: 19-24 Traineeships", LineType = "EAS" },
                new FundLine { Fundline = "Learner Support: 19-24 Traineeships (From Nov 2017)", LineType = "EAS" },
                new FundLine { Fundline = "Learner Support: 24+ Apprenticeships", LineType = "EAS" },
                new FundLine { Fundline = "Vulnerable Bursary: 16-19 Traineeships Bursary", LineType = "EAS" },
                new FundLine { Fundline = "Princes Trust: AEB-Other Learning", LineType = "EAS" },
                new FundLine { Fundline = "Princes Trust: AEB-Other Learning (From Nov 2017)", LineType = "EAS" }
            };

            return fundLines;
        }
    }
}
