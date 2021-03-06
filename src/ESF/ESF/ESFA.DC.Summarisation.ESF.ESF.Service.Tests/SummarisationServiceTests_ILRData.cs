﻿using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.ESF.Model;
using ESFA.DC.Summarisation.ESF.Model.Config;
using ESFA.DC.Summarisation.ESF.Service;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Summarisation.ESF.ESF.Service.Tests
{
    public class SummarisationServiceTests_ILRData
    {
        private const int Contracts = 2;

        private const decimal PeriodValue = 10;

        private const int PeriodVolume = 10;

        [Fact]
        public void SummarisePeriodsTest_withVolume()
        {
            var task = new SummarisationDeliverableProcess();

            DeliverableLine fundLine = new DeliverableLine() { CalculateVolume = true };

            var result = task.SummarisePeriods(GetPeriodsData(5), fundLine, GetCollectionPeriods(), GetContractAllocation(GetProviders().First()));

            result.Count.Should().Be(12);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(5 * PeriodValue);

                item.ActualValue.Should().Be(5 * PeriodVolume);
            }
        }

        [Fact]
        public void SummarisePeriodsTest_NoVolume()
        {
            var task = new SummarisationDeliverableProcess();

            DeliverableLine fundLine = new DeliverableLine() { CalculateVolume = false };

            var result = task.SummarisePeriods(GetPeriodsData(5), fundLine, GetCollectionPeriods(), GetContractAllocation(GetProviders().First()));

            result.Count.Should().Be(12);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(5 * PeriodValue);

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

            var periods = new List<Period>()
            {
                new Period()
                {
                    PeriodId = 1,
                    CalendarMonth = 8,
                    CalendarYear = 2018,
                    Value = value1,
                },
                new Period()
                {
                    PeriodId = 1,
                    CalendarMonth = 8,
                    CalendarYear = 2018,
                    Value = value2,
                },
            };

            List<PeriodisedData> periodisedDatas = new List<PeriodisedData>()
            {
                new PeriodisedData()
                {
                    AttributeName = "StartEarnings",
                    DeliverableCode = "ST01",
                    Periods = periods,
                },
            };

            List<LearningDelivery> learningDeliveries = new List<LearningDelivery>()
            {
                new LearningDelivery()
                {
                        ConRefNumber = "All10000001-1",
                        PeriodisedData = periodisedDatas,
                },
            };

            LearningProvider testProvider = new LearningProvider()
            {
                UKPRN = ukprn,
                LearningDeliveries = learningDeliveries,
            };

            var task = new SummarisationDeliverableProcess();

            var results = task.Summarise(fundingStream, testProvider, allocation, GetCollectionPeriods());

            results.Count.Should().Be(12);
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

            result.Count.Should().Be(12);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(100);

                var fl = fundingStream.DeliverableLines.FirstOrDefault();

                item.ActualVolume.Should().Be(fl?.CalculateVolume == true ? 100 : 0);
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

            foreach (var item in result.Where(w => w.DeliverableCode == 1 || w.DeliverableCode == 4 || w.DeliverableCode == 18))
            {
                item.ActualValue.Should().Be(100);

                var fl = fundingStreams.Where(s => s.DeliverableLineCode == item.DeliverableCode).Select(s => s.DeliverableLines).FirstOrDefault();

                item.ActualVolume.Should().Be(fl.FirstOrDefault()?.CalculateVolume == true ? 100 : 0);
            }
        }

        [Fact]
        public void Summarise_Allocations()
        {
            List<FundingStream> fundingStreams = GetFundingTypes()
                                                .Where(w => w.SummarisationType == "ESF_ILR_And_Supp")
                                                .SelectMany(ft => ft.FundingStreams).ToList();

            int ukprn = GetProviders().First();

            ICollection<FcsContractAllocation> fcsContractAllocations = new List<FcsContractAllocation>();

            for (int i = 1; i <= Contracts; i++)
            {
                FcsContractAllocation allocation = new FcsContractAllocation()
                {
                    ContractAllocationNumber = $"All{ukprn}-{i}",
                    FundingStreamPeriodCode = "ESF1420",
                    DeliveryUkprn = ukprn,
                    DeliveryOrganisation = $"Org{ukprn}",
                    ActualsSchemaPeriodStart = 201808,
                    ActualsSchemaPeriodEnd = 201907,
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

                foreach (var item in result.Where(w => w.DeliverableCode == 1 || w.DeliverableCode == 4 || w.DeliverableCode == 18))
                {
                    item.ActualValue.Should().Be(100);

                    var fl = fundingStreams.Where(s => s.DeliverableLineCode == item.DeliverableCode).Select(s => s.DeliverableLines).FirstOrDefault();

                    item.ActualVolume.Should().Be(fl.FirstOrDefault()?.CalculateVolume == true ? 100 : 0);
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
                ICollection<FcsContractAllocation> fcsContractAllocations = new List<FcsContractAllocation>();

                for (int i = 1; i <= Contracts; i++)
                {
                    FcsContractAllocation allocation = new FcsContractAllocation()
                    {
                        ContractAllocationNumber = $"All{ukprn}-{i}",
                        FundingStreamPeriodCode = "ESF1420",
                        DeliveryUkprn = ukprn,
                        DeliveryOrganisation = $"Org{ukprn}",
                        ActualsSchemaPeriodStart = 201808,
                        ActualsSchemaPeriodEnd = 201907,
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

                    foreach (var item in result.Where(w => w.DeliverableCode == 1 || w.DeliverableCode == 4 || w.DeliverableCode == 18))
                    {
                        item.ActualValue.Should().Be(100);

                        var fl = fundingStreams.Where(s => s.DeliverableLineCode == item.DeliverableCode).Select(s => s.DeliverableLines).FirstOrDefault();

                        item.ActualVolume.Should().Be(fl?.FirstOrDefault()?.CalculateVolume == true ? 100 : 0);
                    }
                }
            }
        }

        private static FundingTypesProvider NewFundingTypeProvider()
        {
            return new FundingTypesProvider(new JsonSerializationService());
        }

        private static LearningProvider GetTestProvider(int ukprn)
        {
            return new LearningProvider()
            {
                UKPRN = ukprn,
                LearningDeliveries = GetLearningDeliveries(ukprn),
            };
        }

        private static List<LearningDelivery> GetLearningDeliveries(int ukprn)
        {
            List<LearningDelivery> learningDeliveries = new List<LearningDelivery>();

            for (int i = 1; i <= Contracts; i++)
            {
                LearningDelivery learningDelivery = new LearningDelivery()
                {
                    ConRefNumber = $"All{ukprn}-{i}",
                    PeriodisedData = GetPeriodisedData(),
                };

                learningDeliveries.Add(learningDelivery);
            }

            return learningDeliveries;
        }

        private static List<PeriodisedData> GetPeriodisedData()
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
                            Periods = GetPeriodsData(2),
                        };

                     periodisedDatas.Add(periodisedData);
                }
            }

            return periodisedDatas;
        }

        private static List<Period> GetPeriodsData(int lotSize)
        {
            var periods = new List<Period>();

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
                        Value = PeriodValue,
                        Volume = PeriodVolume,
                    };
                    periods.Add(period);
                }
            }

            return periods;
        }

        private static List<FundingType> GetFundingTypes()
        {
            FundingTypesProvider fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());

            return fundingTypesProvider.Provide().ToList();
        }

        private static List<CollectionPeriod> GetCollectionPeriods()
        {
            var collectionPeriodsProvider = new CollectionPeriodsProvider(new JsonSerializationService());

            return collectionPeriodsProvider.Provide().Where(w => w.CollectionYear == 1819).ToList();
        }

        private static HashSet<int> GetProviders()
        {
            return new HashSet<int> { 10000001, 10000002 };
        }

        private static HashSet<string> GetAllDeliverableCodes()
        {
            return new HashSet<string>
            {
                "ST01",
                "FS01",
                "PG01",
            };
        }

        private static HashSet<string> GetAllAttributes()
        {
            return new HashSet<string>
            {
                "StartEarnings",
                "AchievementEarnings",
                "AdditionalProgCostEarnings",
                "ProgressionEarnings",
                "DeliverableVolume",
            };
        }

        private static FcsContractAllocation GetContractAllocation(int ukprn)
        {
            FcsContractAllocation allocation = new FcsContractAllocation()
            {
                ContractAllocationNumber = $"All{ukprn}-1",
                FundingStreamPeriodCode = "ESF1420",
                DeliveryUkprn = ukprn,
                DeliveryOrganisation = $"Org{ukprn}",
                ActualsSchemaPeriodStart = 201808,
                ActualsSchemaPeriodEnd = 201907,
            };

            return allocation;
        }
    }
}
