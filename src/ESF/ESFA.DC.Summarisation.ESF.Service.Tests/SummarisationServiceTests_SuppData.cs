using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Model;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ESFA.DC.Summarisation.ESF.Service.Tests
{
    public class SummarisationServiceTests_SuppData
    {
        private const int learningDeliveryRecords = 2;

        private const int contracts = 2;

        private const decimal periodValue = 10;

        //private const int ukprn = 10000001;

        [Fact]
        public void SummarisePeriodsTest_withVolume()
        {
            var task = new SummarisationDeliverableProcess();

            FundLine fundLine = new FundLine() { CalculateVolume = true };

            var result = task.SummarisePeriods(GetPeriodsData(5), fundLine, GetCollectionPeriods());

            result.Count().Should().Be(67);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(5 * periodValue);

                item.ActualVolume.Should().Be(5);
            }
        }

        [Fact]
        public void SummarisePeriodsTest_NoVolume()
        {
            var task = new SummarisationDeliverableProcess();

            FundLine fundLine = new FundLine() { CalculateVolume = false };

            var result = task.SummarisePeriods(GetPeriodsData(5), fundLine, GetCollectionPeriods());

            result.Count().Should().Be(67);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(5 * periodValue);

                item.ActualVolume.Should().Be(0);
            }
        }

        [Fact]
        public void Summarise_FundingStream()
        {
            FundingStream fundingStream = GetFundingTypes()
                .SelectMany(ft => ft.FundingStreams)
                .Where(fs => fs.PeriodCode == "ESF1420" && fs.DeliverableLineCode == 5).FirstOrDefault();

            int ukprn = GetProviders().First();

            FcsContractAllocation allocation = new FcsContractAllocation()
            {
                ContractAllocationNumber = $"All{ukprn}-1",
                FundingStreamPeriodCode = "ESF1420",
                DeliveryUkprn = ukprn,
                DeliveryOrganisation = $"Org{ukprn}"
            };

            var task = new SummarisationDeliverableProcess();

            var result = task.Summarise(fundingStream,GetTestProvider(ukprn),allocation, GetCollectionPeriods());

            result.Count().Should().Be(67);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(2 * periodValue);

                var fl = fundingStream.FundLines.FirstOrDefault();

                if (fl.CalculateVolume == true)
                    item.ActualVolume.Should().Be(2);
                else

                    item.ActualVolume.Should().Be(0);
            }
        }

        [Fact]
        public void Summarise_FundingStreams()
        {
            List<FundingStream> fundingStreams = GetFundingTypes()
                                                .Where(w => w.SummarisationType == "ESF_SuppData")
                                                .SelectMany(ft => ft.FundingStreams).ToList();

            int ukprn = GetProviders().First();

            FcsContractAllocation allocation = new FcsContractAllocation()
            {
                ContractAllocationNumber = $"All{ukprn}-1",
                FundingStreamPeriodCode = "ESF1420",
                DeliveryUkprn = ukprn,
                DeliveryOrganisation = $"Org{ukprn}"
            };

            var task = new SummarisationDeliverableProcess();

            var result = task.Summarise(fundingStreams, GetTestProvider(ukprn), allocation, GetCollectionPeriods());

            result.Count(w => w.DeliverableCode == 5).Should().Be(67);

            result.Count(w => w.DeliverableCode == 6).Should().Be(67);

            result.Count(w => w.DeliverableCode == 7).Should().Be(67);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(2 * periodValue);

                var fl = fundingStreams.Where(s => s.DeliverableLineCode == item.DeliverableCode).Select(s => s.FundLines).FirstOrDefault();

                if (fl.FirstOrDefault().CalculateVolume == true)
                    item.ActualVolume.Should().Be(2);
                else
                    item.ActualVolume.Should().Be(0);
            }
        }

        [Fact]
        public void Summarise_Allocations()
        {
            List<FundingStream> fundingStreams = GetFundingTypes()
                                                .Where(w => w.SummarisationType == "ESF_SuppData")
                                                .SelectMany(ft => ft.FundingStreams).ToList();

            int ukprn = GetProviders().First();

            List<FcsContractAllocation> fcsContractAllocations = new List<FcsContractAllocation>();

            for (int i = 1; i <= contracts; i++)
            {
                FcsContractAllocation allocation = new FcsContractAllocation()
                {
                    ContractAllocationNumber = $"All{ukprn}-{i}",
                    FundingStreamPeriodCode = "ESF1420",
                    DeliveryUkprn = ukprn,
                    DeliveryOrganisation = $"Org{ukprn}"
                };
                fcsContractAllocations.Add(allocation);
            }
            var summarisationMessageMock = new Mock<ISummarisationMessage>();
            var task = new SummarisationDeliverableProcess();

            var result = task.Summarise(fundingStreams, GetTestProvider(ukprn), fcsContractAllocations, GetCollectionPeriods(),summarisationMessageMock.Object);

            foreach (var allocation in fcsContractAllocations)
            {
                result.Count(w => w.ContractAllocationNumber==allocation.ContractAllocationNumber && w.DeliverableCode == 5).Should().Be(67);

                result.Count(w => w.ContractAllocationNumber == allocation.ContractAllocationNumber && w.DeliverableCode == 6).Should().Be(67);

                result.Count(w => w.ContractAllocationNumber == allocation.ContractAllocationNumber && w.DeliverableCode == 7).Should().Be(67);

                foreach (var item in result)
                {
                    item.ActualValue.Should().Be(2 * periodValue);

                    var fl = fundingStreams.Where(s => s.DeliverableLineCode == item.DeliverableCode).Select(s => s.FundLines).FirstOrDefault();
                    
                    if(fl.FirstOrDefault().CalculateVolume == true)
                        item.ActualVolume.Should().Be(2);
                    else
                        item.ActualVolume.Should().Be(0);
                }
            }
        }

        [Fact]
        public void Summarise_Providers()
        {
            List<FundingStream> fundingStreams = GetFundingTypes()
                                                .Where(w => w.SummarisationType == "ESF_SuppData")
                                                .SelectMany(ft => ft.FundingStreams).ToList();

            foreach (var ukprn in GetProviders())
            {
                List<FcsContractAllocation> fcsContractAllocations = new List<FcsContractAllocation>();

                for (int i = 1; i <= contracts; i++)
                {
                    FcsContractAllocation allocation = new FcsContractAllocation()
                    {
                        ContractAllocationNumber = $"All{ukprn}-{i}",
                        FundingStreamPeriodCode = "ESF1420",
                        DeliveryUkprn = ukprn,
                        DeliveryOrganisation = $"Org{ukprn}"
                    };
                    fcsContractAllocations.Add(allocation);
                }

                var summarisationMessageMock = new Mock<ISummarisationMessage>();

                var task = new SummarisationDeliverableProcess();

                var result = task.Summarise(fundingStreams, GetTestProvider(ukprn), fcsContractAllocations, GetCollectionPeriods(), summarisationMessageMock.Object);

                foreach (var allocation in fcsContractAllocations)
                {
                    result.Count(w => w.ContractAllocationNumber == allocation.ContractAllocationNumber && w.DeliverableCode == 5).Should().Be(67);

                    result.Count(w => w.ContractAllocationNumber == allocation.ContractAllocationNumber && w.DeliverableCode == 6).Should().Be(67);

                    result.Count(w => w.ContractAllocationNumber == allocation.ContractAllocationNumber && w.DeliverableCode == 7).Should().Be(67);

                    foreach (var item in result)
                    {
                        item.ActualValue.Should().Be(2 * periodValue);

                        var fl = fundingStreams.Where(s => s.DeliverableLineCode == item.DeliverableCode).Select(s => s.FundLines).FirstOrDefault();

                        if (fl.FirstOrDefault().CalculateVolume == true)
                            item.ActualVolume.Should().Be(2);
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

        private Provider GetTestProvider(int ukprn)
        {
            return new Provider()
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

                    ConRefNumber = $"All{ukprn}-{i} ",
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
                PeriodisedData periodisedData = new PeriodisedData()
                {
                    DeliverableCode = deliverableCode,
                    Periods = GetPeriodsData(2)
                };

                periodisedDatas.Add(periodisedData);
            }

            return periodisedDatas;
        }

        private List<Period> GetPeriodsData(int lotSize)
        {
            List<Period> periods = new List<Period>();
            for (int i = 1; i <= lotSize; i++)
            {
                foreach (var collectionPeriod in GetCollectionPeriods())
                {
                    Period period = new Period()
                    {
                        //PeriodId = collectionPeriod.Period,
                        CalendarMonth = collectionPeriod.CalendarMonth,
                        CalendarYear = collectionPeriod.CalendarYear,
                        Value = periodValue,
                        Volume = 1
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

            return collectionPeriodsProvider.Provide().ToList();
        }

        private HashSet<int> GetProviders()
        {
            return new HashSet<int> { 10000001, 10000002 };
        }

        private HashSet<string> GetAllDeliverableCodes()
        {
            return new HashSet<string> { "AC01",
                                        "CG01",
                                        "CG02",
                                        "FS01",
                                        "NR01",
                                        "PG01",
                                        "PG02",
                                        "PG03",
                                        "PG04",
                                        "PG05",
                                        "PG06",
                                        "RQ01",
                                        "SD01",
                                        "SD02",
                                        "SD03",
                                        "SD04",
                                        "SD05",
                                        "SD06",
                                        "SD07",
                                        "SD08",
                                        "SD09",
                                        "SD10",
                                        "ST01",
                                        "SU01",
                                        "SU02",
                                        "SU03",
                                        "SU04",
                                        "SU05",
                                        "SU11",
                                        "SU12",
                                        "SU13",
                                        "SU14",
                                        "SU15",
                                        "SU21",
                                        "SU22",
                                        "SU23",
                                        "SU24"
            };
        }

        private HashSet<string> GetAllAttributes()
        {
            return new HashSet<string> { "StartEarnings",
                                    "AchievementEarnings",
                                    "AdditionalProgCostEarnings",
                                    "ProgressionEarnings"
            };
        }

        
    }
}
