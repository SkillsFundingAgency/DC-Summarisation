using ESFA.DC.ESF.FundingData.Database.EF.Interfaces;
using ESFA.DC.ESF.FundingData.Database.EF.Query;
using ESFA.DC.Summarisation.ESF.ESF.Service.Providers;
using ESFA.DC.Summarisation.Interfaces;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.Summarisation.ESF.ESF.Service.Tests
{
    public class ESFILRProvider_Tests
    {
        [Theory]
        [InlineData("ILR1516", 1819, 12, 0)]
        [InlineData("ILR1617", 1819, 12, 0)]
        [InlineData("ILR1718", 1819, 12, 14)]
        [InlineData("ILR1819", 1819, 12, 12)]
        [InlineData("ILR1819", 1819, 13, 13)]
        [InlineData("ILR1920", 1819, 13, 3)]
        [InlineData("ILR1920", 1819, 14, 3)]
        public void PredicateCollectionMonth_Test(string collectionType, int previousCollectionYear, int previousCollectionMonth, int expectedResult)
        {
            var esfFundingDataContextMock = new Mock<IESFFundingDataContext>();

            var latestProviderSubmissions = TestLatestProviderSubmissons().First(x => x.Key == collectionType);

            var actualResult = NewProvider(() => esfFundingDataContextMock.Object)
                        .PredicateCollectionMonth(latestProviderSubmissions, previousCollectionYear, previousCollectionMonth);

            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData("ILR1819", 1819, 14, 14)]
        public void PredicateCollectionMonth_R14(string collectionType, int previousCollectionYear, int previousCollectionMonth, int expectedResult)
        {
            var esfFundingDataContextMock = new Mock<IESFFundingDataContext>();

            KeyValuePair<string, List<int>> latestProviderSubmissisons = new KeyValuePair<string, List<int>>("ILR1819", new List<int> { 14, 13, 12, 11, 10 });

            var actualResult = NewProvider(() => esfFundingDataContextMock.Object)
                        .PredicateCollectionMonth(latestProviderSubmissisons, previousCollectionYear, previousCollectionMonth);

            Assert.Equal(expectedResult, actualResult);
        }

        private Dictionary<string, List<int>> TestLatestProviderSubmissons()
        {
            Dictionary<string, List<int>> latestProviderSubmissisons = new Dictionary<string, List<int>>();

            latestProviderSubmissisons.Add("ILR1516", new List<int> { 0 });

            latestProviderSubmissisons.Add("ILR1617", new List<int> { 0 });

            latestProviderSubmissisons.Add("ILR1718", new List<int> { 14, 13, 0 });

            latestProviderSubmissisons.Add("ILR1819", new List<int> { 13, 12, 11, 10 });

            latestProviderSubmissisons.Add("ILR1920", new List<int> { 3, 2, 1 });

            return latestProviderSubmissisons;
        }

        [Fact]
        public async Task ProvideUkprnsAsync_Check()
        {
            var latestProviderSubmissions = new List<LatestProviderSubmission>()
            {
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1516", CollectionReturnCode = "" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1617", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1718", CollectionReturnCode = "R14" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R14" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1920", CollectionReturnCode = "R02" },

                new LatestProviderSubmission() { UKPRN = 1009876, CollectionType = "ILR1516", CollectionReturnCode = "" },
                new LatestProviderSubmission() { UKPRN = 1009876, CollectionType = "ILR1617", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1009876, CollectionType = "ILR1718", CollectionReturnCode = "R14" },
                new LatestProviderSubmission() { UKPRN = 1009876, CollectionType = "ILR1819", CollectionReturnCode = "R14" },
                new LatestProviderSubmission() { UKPRN = 1009876, CollectionType = "ILR1920", CollectionReturnCode = "R02" },

            }.AsQueryable().BuildMock();

            var expectedProviders = new List<int> { 1000825, 1009876 };

            var esfFundingDataContextMock = new Mock<IESFFundingDataContext>();

            esfFundingDataContextMock
                .Setup(x => x.LatestProviderSubmissions)
                .Returns(latestProviderSubmissions.Object);

            var result = await NewProvider(() => esfFundingDataContextMock.Object)
                .ProvideUkprnsAsync(CancellationToken.None);

            result.Count().Should().Be(2);

            result.Should().BeEquivalentTo(expectedProviders);
        }

        [Fact]
        public async Task ProvideAsync_1920_R01_PeriodEnd()
        {
            var latestProviderSubmissions = new List<LatestProviderSubmission>()
            {
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1516", CollectionReturnCode = "" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1617", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1718", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1718", CollectionReturnCode = "R14" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R12" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1920", CollectionReturnCode = "R02" },
            }.AsQueryable().BuildMock();

            var esfFundingData = new List<ESFFundingData>()
            {
                new ESFFundingData() { UKPRN = 1000825, CollectionType = "ILR1516", CollectionReturnCode = "", ConRefNumber = "Conref-1516", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825, CollectionType = "ILR1516", CollectionReturnCode = "", ConRefNumber = "Conref-1516", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.11M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1617", CollectionReturnCode = "R13", ConRefNumber = "Conref-1617", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.12M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1617", CollectionReturnCode = "R13", ConRefNumber = "Conref-1617", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.13M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1718", CollectionReturnCode = "R13", ConRefNumber = "Conref-1718", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.14M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1718", CollectionReturnCode = "R13", ConRefNumber = "Conref-1718", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.15M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1718", CollectionReturnCode = "R14", ConRefNumber = "Conref-1718", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.16M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1718", CollectionReturnCode = "R14", ConRefNumber = "Conref-1718", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.17M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1920", CollectionReturnCode = "R01", ConRefNumber = "Conref-1920", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1920", CollectionReturnCode = "R02", ConRefNumber = "Conref-1920", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
            }.AsQueryable().BuildMock();

            var expectedProviders = new List<int> { 1000825, 1009876 };

            var esfFundingDataContextMock = new Mock<IESFFundingDataContext>();

            esfFundingDataContextMock
                 .Setup(x => x.LatestProviderSubmissions)
                 .Returns(latestProviderSubmissions.Object);

            esfFundingDataContextMock
                 .Setup(x => x.ESFFundingDatas)
                 .Returns(esfFundingData.Object);

            var summarisationMessageMock = new Mock<ISummarisationMessage>();

            summarisationMessageMock.SetupGet(s => s.Ukprn).Returns(1000825);
            summarisationMessageMock.SetupGet(s => s.CollectionYear).Returns(1920);
            summarisationMessageMock.SetupGet(s => s.CollectionMonth).Returns(1);

            var result = await NewProvider(() => esfFundingDataContextMock.Object)
                 .ProvideAsync(1000825, summarisationMessageMock.Object, CancellationToken.None);

            result.Count().Should().Be(5);

            result.Where(w => w.ConRefNumber == "Conref-1516").SelectMany(pd => pd.PeriodisedData).Select(p => p.Periods.First(f => f.PeriodId == 1)).First().Value.Should().Be(20.21M);

            result.Where(w => w.ConRefNumber == "Conref-1617").SelectMany(pd => pd.PeriodisedData).Select(p => p.Periods.First(f => f.PeriodId == 1)).First().Value.Should().Be(20.25M);

            result.Where(w => w.ConRefNumber == "Conref-1718").SelectMany(pd => pd.PeriodisedData).Select(p => p.Periods.First(f => f.PeriodId == 1)).First().Value.Should().Be(20.33M);

            result.Where(w => w.ConRefNumber == "Conref-1819").SelectMany(pd => pd.PeriodisedData).Select(p => p.Periods.First(f => f.PeriodId == 1)).First().Value.Should().Be(101M);

            result.Where(w => w.ConRefNumber == "Conref-1920").SelectMany(pd => pd.PeriodisedData).Select(p => p.Periods.First(f => f.PeriodId == 1)).First().Value.Should().Be(10.10M);
        }

        [Fact]
        public async Task ProvideAsync_1920_R02_PeriodEnd()
        {
            var latestProviderSubmissions = new List<LatestProviderSubmission>()
            {
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1516", CollectionReturnCode = "" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1617", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1718", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1718", CollectionReturnCode = "R14" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R12" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1920", CollectionReturnCode = "R02" },
            }.AsQueryable().BuildMock();

            var esfFundingData = new List<ESFFundingData>()
            {
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1920", CollectionReturnCode = "R01", ConRefNumber = "Conref-1920", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1920", CollectionReturnCode = "R02", ConRefNumber = "Conref-1920", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
            }.AsQueryable().BuildMock();

            var expectedProviders = new List<int> { 1000825, 1009876 };

            var esfFundingDataContextMock = new Mock<IESFFundingDataContext>();

            esfFundingDataContextMock
                .Setup(x => x.LatestProviderSubmissions)
                .Returns(latestProviderSubmissions.Object);

            esfFundingDataContextMock
                .Setup(x => x.ESFFundingDatas)
                .Returns(esfFundingData.Object);

            var summarisationMessageMock = new Mock<ISummarisationMessage>();

            summarisationMessageMock.SetupGet(s => s.Ukprn).Returns(1000825);
            summarisationMessageMock.SetupGet(s => s.CollectionYear).Returns(1920);
            summarisationMessageMock.SetupGet(s => s.CollectionMonth).Returns(2);

            var result = await NewProvider(() => esfFundingDataContextMock.Object)
                .ProvideAsync(1000825, summarisationMessageMock.Object, CancellationToken.None);

            result.Where(w => w.ConRefNumber == "Conref-1819").SelectMany(pd => pd.PeriodisedData).Select(p => p.Periods.First(f => f.PeriodId == 1)).First().Value.Should().Be(80.80M);

            result.Where(w => w.ConRefNumber == "Conref-1920").SelectMany(pd => pd.PeriodisedData).Select(p => p.Periods.First(f => f.PeriodId == 1)).First().Value.Should().Be(10.10M);
        }

        [Fact]
        public async Task ProvideAsync_1920_R02_R13Removed()
        {
            var latestProviderSubmissions = new List<LatestProviderSubmission>()
            {
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1516", CollectionReturnCode = "" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1617", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1718", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1718", CollectionReturnCode = "R14" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R12" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1920", CollectionReturnCode = "R02" },
            }.AsQueryable().BuildMock();

            var esfFundingData = new List<ESFFundingData>()
            {
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1920", CollectionReturnCode = "R01", ConRefNumber = "Conref-1920", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1920", CollectionReturnCode = "R02", ConRefNumber = "Conref-1920", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
            }.AsQueryable().BuildMock();

            var expectedProviders = new List<int> { 1000825, 1009876 };

            var esfFundingDataContextMock = new Mock<IESFFundingDataContext>();

            esfFundingDataContextMock
                .Setup(x => x.LatestProviderSubmissions)
                .Returns(latestProviderSubmissions.Object);

            esfFundingDataContextMock
                .Setup(x => x.ESFFundingDatas)
                .Returns(esfFundingData.Object);

            var summarisationMessageMock = new Mock<ISummarisationMessage>();

            summarisationMessageMock.SetupGet(s => s.Ukprn).Returns(1000825);
            summarisationMessageMock.SetupGet(s => s.CollectionYear).Returns(1920);
            summarisationMessageMock.SetupGet(s => s.CollectionMonth).Returns(2);

            var result = await NewProvider(() => esfFundingDataContextMock.Object)
                .ProvideAsync(1000825, summarisationMessageMock.Object, CancellationToken.None);

            result.Where(w => w.ConRefNumber == "Conref-1819").Count().Should().Be(0);

            result.Where(w => w.ConRefNumber == "Conref-1920").SelectMany(pd => pd.PeriodisedData).Select(p => p.Periods.First(f => f.PeriodId == 1)).First().Value.Should().Be(10.10M);
        }

        [Fact]
        public async Task ProvideAsync_1920_R02_R14Added()
        {
            var latestProviderSubmissions = new List<LatestProviderSubmission>()
            {
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1516", CollectionReturnCode = "" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1617", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1718", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1718", CollectionReturnCode = "R14" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R12" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R14" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1920", CollectionReturnCode = "R02" },
            }.AsQueryable().BuildMock();

            var esfFundingData = new List<ESFFundingData>()
            {
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R14", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R14", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1920", CollectionReturnCode = "R01", ConRefNumber = "Conref-1920", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1920", CollectionReturnCode = "R02", ConRefNumber = "Conref-1920", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
            }.AsQueryable().BuildMock();

            var expectedProviders = new List<int> { 1000825, 1009876 };

            var esfFundingDataContextMock = new Mock<IESFFundingDataContext>();

            esfFundingDataContextMock
                .Setup(x => x.LatestProviderSubmissions)
                .Returns(latestProviderSubmissions.Object);

            esfFundingDataContextMock
                .Setup(x => x.ESFFundingDatas)
                .Returns(esfFundingData.Object);

            var summarisationMessageMock = new Mock<ISummarisationMessage>();

            summarisationMessageMock.SetupGet(s => s.Ukprn).Returns(1000825);
            summarisationMessageMock.SetupGet(s => s.CollectionYear).Returns(1920);
            summarisationMessageMock.SetupGet(s => s.CollectionMonth).Returns(2);

            var result = await NewProvider(() => esfFundingDataContextMock.Object)
                .ProvideAsync(1000825, summarisationMessageMock.Object, CancellationToken.None);

            result.Where(w => w.ConRefNumber == "Conref-1819").SelectMany(pd => pd.PeriodisedData).Select(p => p.Periods.First(f => f.PeriodId == 1)).First().Value.Should().Be(80.80M);

            result.Where(w => w.ConRefNumber == "Conref-1920").SelectMany(pd => pd.PeriodisedData).Select(p => p.Periods.First(f => f.PeriodId == 1)).First().Value.Should().Be(10.10M);
        }

        [Fact]
        public async Task ProvideAsync_1920_R03PeriodEnd()
        {
            var latestProviderSubmissions = new List<LatestProviderSubmission>()
            {
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1516", CollectionReturnCode = "" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1617", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1718", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1718", CollectionReturnCode = "R14" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R12" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R13" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R14" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1920", CollectionReturnCode = "R02" },
                new LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1920", CollectionReturnCode = "R03" },
            }.AsQueryable().BuildMock();

            var esfFundingData = new List<ESFFundingData>()
            {
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R12", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R13", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R14", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1819", CollectionReturnCode = "R14", ConRefNumber = "Conref-1819", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1920", CollectionReturnCode = "R01", ConRefNumber = "Conref-1920", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1920", CollectionReturnCode = "R02", ConRefNumber = "Conref-1920", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1920", CollectionReturnCode = "R03", ConRefNumber = "Conref-1920", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1920", CollectionReturnCode = "R03", ConRefNumber = "Conref-1920", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1920", CollectionReturnCode = "R03", ConRefNumber = "Conref-1920", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingData() { UKPRN = 1000825,  CollectionType = "ILR1920", CollectionReturnCode = "R03", ConRefNumber = "Conref-1920", DeliverableCode = "D01", AttributeName = "Att1", Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
            }.AsQueryable().BuildMock();

            var expectedProviders = new List<int> { 1000825, 1009876 };

            var esfFundingDataContextMock = new Mock<IESFFundingDataContext>();

            esfFundingDataContextMock
                .Setup(x => x.LatestProviderSubmissions)
                .Returns(latestProviderSubmissions.Object);

            esfFundingDataContextMock
                .Setup(x => x.ESFFundingDatas)
                .Returns(esfFundingData.Object);

            var summarisationMessageMock = new Mock<ISummarisationMessage>();

            summarisationMessageMock.SetupGet(s => s.Ukprn).Returns(1000825);
            summarisationMessageMock.SetupGet(s => s.CollectionYear).Returns(1920);
            summarisationMessageMock.SetupGet(s => s.CollectionMonth).Returns(3);

            var result = await NewProvider(() => esfFundingDataContextMock.Object)
                .ProvideAsync(1000825, summarisationMessageMock.Object, CancellationToken.None);

            result.Where(w => w.ConRefNumber == "Conref-1819").SelectMany(pd => pd.PeriodisedData).Select(p => p.Periods.First(f => f.PeriodId == 1)).First().Value.Should().Be(20.20M);

            result.Where(w => w.ConRefNumber == "Conref-1920").SelectMany(pd => pd.PeriodisedData).Select(p => p.Periods.First(f => f.PeriodId == 1)).First().Value.Should().Be(40.40M);
        }

        private ESFILRProvider NewProvider(Func<IESFFundingDataContext> esfFundingDataContext = null)
        {
            return new ESFILRProvider(esfFundingDataContext);
        }
    }
}
