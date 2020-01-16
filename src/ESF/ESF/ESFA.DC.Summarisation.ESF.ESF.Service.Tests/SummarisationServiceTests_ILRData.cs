using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.External.FCS.Model;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Summarisation.ESF.ESF.Service.Tests
{
    public class SummarisationServiceTests_ILRData
    {
        private const int learningDeliveryRecords = 2;

        private const int contracts = 2;

        private const decimal periodValue = 10;

        private const int periodVolume = 10;

        [Fact]
        public void SummarisePeriodsTest_withVolume()
        {
            var task = new SummarisationDeliverableProcess();

            FundLine fundLine = new FundLine() { CalculateVolume = true };

            var result = task.SummarisePeriods(GetPeriodsData(5), fundLine, GetCollectionPeriods(), GetContractAllocation(GetProviders().First()));

            result.Count().Should().Be(12);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(5 * periodValue);

                item.ActualValue.Should().Be(5 * periodVolume);
            }
        }

        [Fact]
        public void SummarisePeriodsTest_NoVolume()
        {
            var task = new SummarisationDeliverableProcess();

            FundLine fundLine = new FundLine() { CalculateVolume = false };

            var result = task.SummarisePeriods(GetPeriodsData(5), fundLine, GetCollectionPeriods(), GetContractAllocation(GetProviders().First()));

            result.Count().Should().Be(12);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(5 * periodValue);

                item.ActualVolume.Should().Be(0);
            }
        }

        [Theory]
        [InlineData(12.51032, 6.11421, 18.62)]
        [InlineData(19.94123, 1.05854, 21.00)]
        [InlineData(10.23101, 0.01143, 10.24)]
        [InlineData(5, 3, 8.00)]
        public void Summarise_CheckRounding(decimal value1, decimal value2, decimal result)
        {
            FundingStream fundingStream = GetFundingTypes()
                .SelectMany(ft => ft.FundingStreams)
                .Single(fs => fs.PeriodCode == "ESF1420" && fs.DeliverableLineCode == 1);

            int ukprn = GetProviders().First();

            var allocation = GetContractAllocation(ukprn);
            
            var periods = new List<IPeriod>()
            {
                new Period()
                {
                    PeriodId = 1,
                    CalendarMonth = 8,
                    CalendarYear = 2018,
                    Value = value1
                },
                new Period()
                {
                    PeriodId = 1,
                    CalendarMonth = 8,
                    CalendarYear = 2018,
                    Value = value2
                }
            };

            List<PeriodisedData> periodisedDatas = new List<PeriodisedData>()
            {
                new PeriodisedData()
                {
                    AttributeName = "StartEarnings",
                    DeliverableCode = "ST01",
                    Periods = periods
                }
            };

            List<LearningDelivery> learningDeliveries = new List<LearningDelivery>()
            {
                new LearningDelivery()
                {
                        LearnRefNumber = "100000425",
                        ConRefNumber = "All10000001-1",
                        AimSeqNumber = 10001,
                        Fundline = "16-18 Apprenticeship",
                        DeliverableCode = "ST01",
                        PeriodisedData = periodisedDatas
                },
            };

            LearningProvider testProvider = new LearningProvider()
            {
                UKPRN = ukprn,
                LearningDeliveries = learningDeliveries
            };

            var task = new SummarisationDeliverableProcess();

            var results = task.Summarise(fundingStream, testProvider, allocation, GetCollectionPeriods());

            results.Count().Should().Be(12);
            results.FirstOrDefault(w => w.ActualValue > 0).ActualValue.Should().Be(result);
        }

        [Fact]
        public void Summarise_FundingStream()
        {
            FundingStream fundingStream = GetFundingTypes()
                .SelectMany(ft => ft.FundingStreams)
                .Single(fs => fs.PeriodCode == "ESF1420" && fs.DeliverableLineCode == 1);

            int ukprn = GetProviders().First();

            var allocation = GetContractAllocation(ukprn);

            var task = new SummarisationDeliverableProcess();

            var result = task.Summarise(fundingStream, GetTestProvider(ukprn), allocation, GetCollectionPeriods());

            result.Count().Should().Be(12);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(100);

                var fl = fundingStream.FundLines.FirstOrDefault();

                if (fl.CalculateVolume == true)
                    item.ActualVolume.Should().Be(100);
                else

                    item.ActualVolume.Should().Be(0);
            }
        }

        [Fact]
        public void Summarise_FundingStreams()
        {
            List<FundingStream> fundingStreams = GetFundingTypes()
                                                .Where(w => w.SummarisationType == "ESF_ILR_And_Supp")
                                                .SelectMany(ft => ft.FundingStreams).ToList();

            int ukprn = GetProviders().First();

            FcsContractAllocation allocation = GetContractAllocation(ukprn);

            var task = new SummarisationDeliverableProcess();

            var result = task.Summarise(fundingStreams, GetTestProvider(ukprn), allocation, GetCollectionPeriods());

            result.Count(w => w.DeliverableCode == 1).Should().Be(12);

            result.Count(w => w.DeliverableCode == 4).Should().Be(12);

            result.Count(w => w.DeliverableCode == 18).Should().Be(12);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(100);

                var fl = fundingStreams.Where(s => s.DeliverableLineCode == item.DeliverableCode).Select(s => s.FundLines).FirstOrDefault();

                if (fl.FirstOrDefault().CalculateVolume == true)
                    item.ActualVolume.Should().Be(100);
                else
                    item.ActualVolume.Should().Be(0);
            }
        }

        [Fact]
        public void Summarise_Allocations()
        {
            List<FundingStream> fundingStreams = GetFundingTypes()
                                                .Where(w => w.SummarisationType == "ESF_ILR_And_Supp")
                                                .SelectMany(ft => ft.FundingStreams).ToList();

            int ukprn = GetProviders().First();

            ICollection<IFcsContractAllocation> fcsContractAllocations = new List<IFcsContractAllocation>();

            for (int i = 1; i <= contracts; i++)
            {
                FcsContractAllocation allocation = new FcsContractAllocation()
                {
                    ContractAllocationNumber = $"All{ukprn}-{i}",
                    FundingStreamPeriodCode = "ESF1420",
                    DeliveryUkprn = ukprn,
                    DeliveryOrganisation = $"Org{ukprn}",
                    ContractStartDate = 201808,
                    ContractEndDate = 201907
                };

                fcsContractAllocations.Add(allocation);
            }

            var summarisationMessageMock = new Mock<ISummarisationMessage>();
            var task = new SummarisationDeliverableProcess();

            var result = task.Summarise(fundingStreams, GetTestProvider(ukprn), fcsContractAllocations, GetCollectionPeriods(),summarisationMessageMock.Object);

            foreach (var allocation in fcsContractAllocations)
            {
                result.Count(w => w.ContractAllocationNumber == allocation.ContractAllocationNumber && w.DeliverableCode == 1).Should().Be(12);

                result.Count(w => w.ContractAllocationNumber == allocation.ContractAllocationNumber && w.DeliverableCode == 4).Should().Be(12);

                result.Count(w => w.ContractAllocationNumber == allocation.ContractAllocationNumber && w.DeliverableCode == 18).Should().Be(12);

                foreach (var item in result)
                {
                    item.ActualValue.Should().Be(100);

                    var fl = fundingStreams.Where(s => s.DeliverableLineCode == item.DeliverableCode).Select(s => s.FundLines).FirstOrDefault();

                    if (fl.FirstOrDefault().CalculateVolume == true)
                        item.ActualVolume.Should().Be(100);
                    else
                        item.ActualVolume.Should().Be(0);
                }
            }
        }

        [Fact]
        public void Summarise_Providers()
        {
            List<FundingStream> fundingStreams = GetFundingTypes()
                                                .Where(w => w.SummarisationType == "ESF_ILR_And_Supp")
                                                .SelectMany(ft => ft.FundingStreams).ToList();

            foreach (var ukprn in GetProviders())
            {
                ICollection<IFcsContractAllocation> fcsContractAllocations = new List<IFcsContractAllocation>();

                for (int i = 1; i <= contracts; i++)
                {
                    FcsContractAllocation allocation = new FcsContractAllocation()
                    {
                        ContractAllocationNumber = $"All{ukprn}-{i}",
                        FundingStreamPeriodCode = "ESF1420",
                        DeliveryUkprn = ukprn,
                        DeliveryOrganisation = $"Org{ukprn}",
                        ContractStartDate = 201808,
                        ContractEndDate = 201907
                    };

                    fcsContractAllocations.Add(allocation);
                }

                var summarisationMessageMock = new Mock<ISummarisationMessage>();

                var task = new SummarisationDeliverableProcess();

                var result = task.Summarise(fundingStreams, GetTestProvider(ukprn), fcsContractAllocations, GetCollectionPeriods(), summarisationMessageMock.Object);

                foreach (var allocation in fcsContractAllocations)
                {
                    result.Count(w => w.ContractAllocationNumber == allocation.ContractAllocationNumber && w.DeliverableCode == 1).Should().Be(12);

                    result.Count(w => w.ContractAllocationNumber == allocation.ContractAllocationNumber && w.DeliverableCode == 4).Should().Be(12);

                    result.Count(w => w.ContractAllocationNumber == allocation.ContractAllocationNumber && w.DeliverableCode == 18).Should().Be(12);

                    foreach (var item in result)
                    {
                        item.ActualValue.Should().Be(100);

                        var fl = fundingStreams.Where(s => s.DeliverableLineCode == item.DeliverableCode).Select(s => s.FundLines).FirstOrDefault();

                        if (fl.FirstOrDefault().CalculateVolume == true)
                            item.ActualVolume.Should().Be(100);
                        else
                            item.ActualVolume.Should().Be(0);
                    }
                }
            }
        }


        private FundingTypesProvider NewFundingTypeProvider()
        {
            return new FundingTypesProvider(new JsonSerializationService());
        }

        private LearningProvider GetTestProvider(int ukprn)
        {
            return new LearningProvider()
            {
                UKPRN = ukprn,
                LearningDeliveries = GetLearningDeliveries(ukprn)
            };
        }

        private List<LearningDelivery> GetLearningDeliveries(int ukprn)
        {
            List<LearningDelivery> learningDeliveries = new List<LearningDelivery>();

            for (int i = 1; i <= contracts; i++)
            {
                LearningDelivery learningDelivery = new LearningDelivery()
                {

                    ConRefNumber = $"All{ukprn}-{i}",
                    PeriodisedData = GetPeriodisedData()
                };

                learningDeliveries.Add(learningDelivery);
            }

            return learningDeliveries;
        }

        private List<PeriodisedData> GetPeriodisedData()
        {
            HashSet<string> attributes = GetAllAttributes();

            HashSet<string> deliverableCodes = GetAllDeliverableCodes();

            List<PeriodisedData> periodisedDatas = new List<PeriodisedData>();

            foreach (var deliverableCode in deliverableCodes)
            {
                foreach (var attribute in attributes)
                {
                     PeriodisedData periodisedData = new PeriodisedData()
                        {
                            AttributeName = attribute,
                            DeliverableCode = deliverableCode,
                            Periods = GetPeriodsData(2)
                        };

                    periodisedDatas.Add(periodisedData);
                }
            }

            return periodisedDatas;
        }

        private List<IPeriod> GetPeriodsData(int lotSize)
        {
            List<IPeriod> periods = new List<IPeriod>();
            for (int i = 1; i <= lotSize; i++)
            {
                foreach (var collectionPeriod in GetCollectionPeriods())
                {
                    Period period = new Period()
                    {
                        PeriodId = collectionPeriod.CollectionMonth,
                        CollectionYear = collectionPeriod.CollectionYear,
                        CalendarYear = collectionPeriod.CalendarYear,
                        CalendarMonth = collectionPeriod.CalendarMonth,
                        Value = periodValue,
                        Volume = periodVolume
                    };
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

            return collectionPeriodsProvider.Provide().Where(w => w.CollectionYear == 1819).ToList();
        }

        private HashSet<int> GetProviders()
        {
            return new HashSet<int> { 10000001, 10000002 };
        }

        private HashSet<string> GetAllDeliverableCodes()
        {
            return new HashSet<string> { 
                                        "ST01",
                                        "FS01",
                                        "PG01"
                                        
            };
        }

        private HashSet<string> GetAllAttributes()
        {
            return new HashSet<string> { "StartEarnings",
                                    "AchievementEarnings",
                                    "AdditionalProgCostEarnings",
                                    "ProgressionEarnings",
                                    "DeliverableVolume"
            };
        }

        private FcsContractAllocation GetContractAllocation(int ukprn)
        {
            FcsContractAllocation allocation = new FcsContractAllocation()
            {
                ContractAllocationNumber = $"All{ukprn}-1",
                FundingStreamPeriodCode = "ESF1420",
                DeliveryUkprn = ukprn,
                DeliveryOrganisation = $"Org{ukprn}",
                ContractStartDate = 201808,
                ContractEndDate = 201907
            };

            return allocation;
        }


    }
}
