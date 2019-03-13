﻿using ESFA.DC.Summarisation.Data.Input.Model;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Serialization.Json;
using Xunit;
using FluentAssertions;
using ESFA.DC.Summarisation.Main1819.Service.Providers;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.External.FCS.Model;

namespace ESFA.DC.Summarisation.Main1819.Service.Tests
{
    public class SummarisationServiceTests
    {
        private int learningDeliveryRecords = 2;

        public decimal periodValue = 10;

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

            var result = task.GetPeriodsForFundLine(fundLine.UseAttributes? GetPeriodisedData(5) : GetPeriodisedDataNoAttributes(5), fundLine);

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

            var easfundlinesCount = fundingStream.FundLines.Where(fl => fl.LineType =="EAS").Count();

            var ilrfundlinesCount = fundingStream.FundLines.Where(fl => fl.LineType != "EAS").Count();

            int attributescount = 0;

            if (fundingStream.FundLines.Where(flW => flW.LineType != "EAS").Select(fl => fl.Attributes).Any())
            {
                attributescount = fundingStream.FundLines.Where(flW => flW.LineType != "EAS").Select(fl => fl.Attributes).First().Count();
            }

            var task = new SummarisationService();

            var results = task.Summarise(fundingStream, GetTestProvider(), GetContractAllocation(), GetCollectionPeriods()).OrderBy(x=>x.Period).ToList();

            results.Count().Should().Be(12);

            int i = 1;

            foreach (var item in results)
            {
                item.ActualValue.Should().Be((learningDeliveryRecords * ilrfundlinesCount * attributescount * i * periodValue) + (learningDeliveryRecords * easfundlinesCount * i * periodValue));

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

            var results = task.Summarise(fundingType, GetTestProvider(), GetContractAllocation(), GetCollectionPeriods());

            Dictionary<string, int> fspdlc = new Dictionary<string, int>();

            var fundingStreams = fundingType.FundingStreams.Select(fs => new {fs.PeriodCode, fs.DeliverableLineCode }).ToList();

            foreach (var item in fundingStreams)
            {
                results.Count(r => r.FundingStreamPeriodCode == item.PeriodCode && r.DeliverableCode == item.DeliverableLineCode).Should().Be(12);

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
            List<CollectionPeriod> collectionPeriods = GetCollectionPeriods();

            var learningDeliveries = provider.LearningDeliveries.ToList();

            for (int i = 0; i < 17; i++)
            {
                learningDeliveries.AddRange(learningDeliveries);
            }

            provider.LearningDeliveries = learningDeliveries;

            FundingType fundingType = GetFundingType(key);

            var task = new SummarisationService();

            var results = task.Summarise(fundingType, provider, GetContractAllocation(),GetCollectionPeriods()).ToList();

            var fundingStreams = fundingType.FundingStreams.Select(fs => new { fs.PeriodCode, fs.DeliverableLineCode }).ToList();

            foreach (var item in fundingStreams)
            {
                results.Count(r => r.FundingStreamPeriodCode == item.PeriodCode && r.DeliverableCode == item.DeliverableLineCode).Should().Be(12);
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
                UKPRN = ukprn,
                LearningDeliveries = GetLearningDeliveries()
            };
        }
        private List<ILearningDelivery> GetLearningDeliveries()
        {
            List<ILearningDelivery> learningDeliveries = new List<ILearningDelivery>();

            foreach (var item in GetFundLines())
            {
                for (int i = 1; i <= learningDeliveryRecords; i++)
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


        private List<IPeriodisedData> GetPeriodisedData(int lotSize)
        {
            HashSet<string> attributes = GetAllAttributes();

            List<IPeriodisedData> periodisedDatas = new List<IPeriodisedData>();

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

        private List<IPeriodisedData> GetPeriodisedDataNoAttributes(int lotSize)
        {
            List<IPeriodisedData> periodisedDatas = new List<IPeriodisedData>();

            for (int j = 1; j <= lotSize; j++)
            {
                IPeriodisedData periodisedData = new PeriodisedData()
                {
                    Periods = GetPeriodsData(1)
                };

                periodisedDatas.Add(periodisedData);
            }

            return periodisedDatas;
        }

        private List<IPeriod> GetPeriodsData(int lotSize)
        {
            List<IPeriod> periods = new List<IPeriod>();
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

        private FundingType GetFundingType(string fundingType)
        {
          return GetFundingTypes().First(x => x.Key == fundingType);

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
            var fundingStreams =  GetFundingTypes().SelectMany(ft => ft.FundingStreams);

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