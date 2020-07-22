using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.NCS.Model;
using System.Collections.Generic;
using ESFA.DC.Summarisation.NCS.NCS1920.Service.Providers;
using Xunit;
using FluentAssertions;
using System.Linq;
using ESFA.DC.Summarisation.NCS.Model.Config;
using ESFA.DC.Summarisation.Service.Model.Fcs;

namespace ESFA.DC.Summarisation.NCS.Service.Tests
{
    public class SummarisationNCSProcessTests
    {
        [Fact]
        public void SummarisePeriods()
        {
            var collectionPeriodsProvider =  new CollectionPeriodsProvider(new JsonSerializationService()) ;

            var collectionPeriods = collectionPeriodsProvider.Provide();

            var process = new SummarisationNCSProcess();

            var result = process.SummarisePeriods(TestFundingValues(), collectionPeriods);

            result.Count.Should().Be(12);

            result.First(s => s.Period == 201904).ActualValue.Should().Be(100);
            result.First(s => s.Period == 201905).ActualValue.Should().Be(200);
            result.First(s => s.Period == 201906).ActualValue.Should().Be(300);
            result.First(s => s.Period == 201907).ActualValue.Should().Be(400);
            result.First(s => s.Period == 201908).ActualValue.Should().Be(500);
            result.First(s => s.Period == 201909).ActualValue.Should().Be(0);
            result.First(s => s.Period == 201910).ActualValue.Should().Be(0);
            result.First(s => s.Period == 201911).ActualValue.Should().Be(0);
            result.First(s => s.Period == 201912).ActualValue.Should().Be(0);
            result.First(s => s.Period == 202001).ActualValue.Should().Be(0);
            result.First(s => s.Period == 202002).ActualValue.Should().Be(0);
            result.First(s => s.Period == 202003).ActualValue.Should().Be(0);
        }


        [Fact]
        public void SummariseProviderByFundingStream()
        {
            var fundingStream = FundingTypesConfigured().First().FundingStreams.First(s => s.DeliverableLineCode == 5);

            var provider = TestProvider();

            var allocations = TestContractAllocations();

            var collectionPeriods = CollectionPeriodsConfigured();

            var process = new SummarisationNCSProcess();

            var result = process.Summarise(fundingStream, provider, allocations, collectionPeriods);

            result.Count.Should().Be(12);

            result.First(s => s.Period == 201904).ActualValue.Should().Be(0);
            result.First(s => s.Period == 201905).ActualValue.Should().Be(0);
            result.First(s => s.Period == 201906).ActualValue.Should().Be(300);
            result.First(s => s.Period == 201907).ActualValue.Should().Be(400);
            result.First(s => s.Period == 201908).ActualValue.Should().Be(500);
            result.First(s => s.Period == 201909).ActualValue.Should().Be(0);
            result.First(s => s.Period == 201910).ActualValue.Should().Be(0);
            result.First(s => s.Period == 201911).ActualValue.Should().Be(0);
            result.First(s => s.Period == 201912).ActualValue.Should().Be(0);
            result.First(s => s.Period == 202001).ActualValue.Should().Be(0);
            result.First(s => s.Period == 202002).ActualValue.Should().Be(0);
            result.First(s => s.Period == 202003).ActualValue.Should().Be(0);
        }

        [Fact]
        public void SummariseProvider()
        {
            var fundingStreams = FundingTypesConfigured().First().FundingStreams;

            var provider = TestProvider();

            var allocations = TestContractAllocations();

            var collectionPeriods = CollectionPeriodsConfigured();

            var process = new SummarisationNCSProcess();

            var result = process.Summarise(fundingStreams, provider, allocations, collectionPeriods);

            result.Count.Should().Be(36);

            result.Where(w => w.DeliverableCode == 2).First(s => s.Period == 201904).ActualValue.Should().Be(100);
            result.Where(w => w.DeliverableCode == 2).First(s => s.Period == 201905).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 2).First(s => s.Period == 201906).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 2).First(s => s.Period == 201907).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 2).First(s => s.Period == 201908).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 2).First(s => s.Period == 201909).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 2).First(s => s.Period == 201910).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 2).First(s => s.Period == 201911).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 2).First(s => s.Period == 201912).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 2).First(s => s.Period == 202001).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 2).First(s => s.Period == 202002).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 2).First(s => s.Period == 202003).ActualValue.Should().Be(0);

            result.Where(w => w.DeliverableCode == 4).First(s => s.Period == 201904).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 4).First(s => s.Period == 201905).ActualValue.Should().Be(200);
            result.Where(w => w.DeliverableCode == 4).First(s => s.Period == 201906).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 4).First(s => s.Period == 201907).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 4).First(s => s.Period == 201908).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 4).First(s => s.Period == 201909).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 4).First(s => s.Period == 201910).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 4).First(s => s.Period == 201911).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 4).First(s => s.Period == 201912).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 4).First(s => s.Period == 202001).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 4).First(s => s.Period == 202002).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 4).First(s => s.Period == 202003).ActualValue.Should().Be(0);

            result.Where(w => w.DeliverableCode == 5).First(s => s.Period == 201904).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 5).First(s => s.Period == 201905).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 5).First(s => s.Period == 201906).ActualValue.Should().Be(300);
            result.Where(w => w.DeliverableCode == 5).First(s => s.Period == 201907).ActualValue.Should().Be(400);
            result.Where(w => w.DeliverableCode == 5).First(s => s.Period == 201908).ActualValue.Should().Be(500);
            result.Where(w => w.DeliverableCode == 5).First(s => s.Period == 201909).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 5).First(s => s.Period == 201910).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 5).First(s => s.Period == 201911).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 5).First(s => s.Period == 201912).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 5).First(s => s.Period == 202001).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 5).First(s => s.Period == 202002).ActualValue.Should().Be(0);
            result.Where(w => w.DeliverableCode == 5).First(s => s.Period == 202003).ActualValue.Should().Be(0);
        }


        private ICollection<FundingValue> TestFundingValues()
        {
            return new List<FundingValue>
            {
                new FundingValue{CollectionYear = 1920, CalendarMonth = 4, OutcomeType = 1, Value = 100, },

                new FundingValue{CollectionYear = 1920, CalendarMonth = 5, OutcomeType = 2, Value = 100, },
                new FundingValue{CollectionYear = 1920, CalendarMonth = 5, OutcomeType = 2, Value = 100, },

                new FundingValue{CollectionYear = 1920, CalendarMonth = 6, OutcomeType = 3, Value = 100, },
                new FundingValue{CollectionYear = 1920, CalendarMonth = 6, OutcomeType = 3, Value = 100, },
                new FundingValue{CollectionYear = 1920, CalendarMonth = 6, OutcomeType = 3, Value = 100, },

                new FundingValue{CollectionYear = 1920, CalendarMonth = 7, OutcomeType = 4, Value = 100, },
                new FundingValue{CollectionYear = 1920, CalendarMonth = 7, OutcomeType = 4, Value = 100, },
                new FundingValue{CollectionYear = 1920, CalendarMonth = 7, OutcomeType = 4, Value = 100, },
                new FundingValue{CollectionYear = 1920, CalendarMonth = 7, OutcomeType = 4, Value = 100, },

                new FundingValue{CollectionYear = 1920, CalendarMonth = 8, OutcomeType = 5, Value = 100, },
                new FundingValue{CollectionYear = 1920, CalendarMonth = 8, OutcomeType = 5, Value = 100, },
                new FundingValue{CollectionYear = 1920, CalendarMonth = 8, OutcomeType = 5, Value = 100, },
                new FundingValue{CollectionYear = 1920, CalendarMonth = 8, OutcomeType = 5, Value = 100, },
                new FundingValue{CollectionYear = 1920, CalendarMonth = 8, OutcomeType = 5, Value = 100, },
            };
        }

        private TouchpointProviderFundingData TestProvider()
        {
            var provider = new TouchpointProviderFundingData
            {
               Provider =  new TouchpointProvider{UKPRN = 10001647, TouchpointId = "0000000101" },
               FundingValues = TestFundingValues()
            };

            return provider;
        }

        private ICollection<CollectionPeriod> CollectionPeriodsConfigured()
        {
            var collectionPeriodsProvider = new CollectionPeriodsProvider(new JsonSerializationService());

            return collectionPeriodsProvider.Provide();
        }

        private ICollection<FundingType> FundingTypesConfigured()
        {
            var fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());

            return fundingTypesProvider.Provide();
        }

        private ICollection<FcsContractAllocation> TestContractAllocations()
        {
            return new List<FcsContractAllocation>
            {
                 new FcsContractAllocation
                 {
                     ContractAllocationNumber = "Alloc1",
                     DeliveryOrganisation = "Org101",
                     DeliveryUkprn = 10001647,
                     UoPcode = "0000000101",
                     FundingStreamPeriodCode = "NCS-C1920"
                 },

                 new FcsContractAllocation
                 {
                     ContractAllocationNumber = "Alloc2",
                     DeliveryOrganisation = "Org101",
                     DeliveryUkprn = 10001647,
                     UoPcode = "",
                     FundingStreamPeriodCode = "APPS1920"
                 },

            };

        }
    }
}
