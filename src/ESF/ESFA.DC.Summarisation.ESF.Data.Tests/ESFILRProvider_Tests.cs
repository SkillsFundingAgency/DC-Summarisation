using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.ESF.Service.Providers;
using ESFA.DC.ESF.FundingData.Database.EF.Interfaces;
using Query = ESFA.DC.ESF.FundingData.Database.EF.Query;
using ESFA.DC.ESF.FundingData.Database.EF;
using Xunit;
using MockQueryable.Moq;
using Moq;
using System.Threading;
using FluentAssertions;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.ESF.Data.Tests
{
    public class ESFILRProvider_Tests
    {
        /*Following tests are valid once we implement LatestProviderSubmission data for summarisation*/
        /*[Fact]
        public async Task ProvideUkprnsAsync_Check()
        {
            var latestProviderSubmissions = new List<Query.LatestProviderSubmission>()
            {
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1516", CollectionReturnCode = "" },
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1617", CollectionReturnCode = "R13"  },
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1718", CollectionReturnCode = "R14"  },
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R14"  },
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1920", CollectionReturnCode = "R02"  },

                new Query.LatestProviderSubmission() { UKPRN = 1009876, CollectionType = "ILR1516", CollectionReturnCode = "" },
                new Query.LatestProviderSubmission() { UKPRN = 1009876, CollectionType = "ILR1617", CollectionReturnCode = "R13"  },
                new Query.LatestProviderSubmission() { UKPRN = 1009876, CollectionType = "ILR1718", CollectionReturnCode = "R14"  },
                new Query.LatestProviderSubmission() { UKPRN = 1009876, CollectionType = "ILR1819", CollectionReturnCode = "R14"  },
                new Query.LatestProviderSubmission() { UKPRN = 1009876, CollectionType = "ILR1920", CollectionReturnCode = "R02"  },

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
        public async Task ProvideAsync_Check_R13Added()
        {
            var latestProviderSubmissions = new List<Query.LatestProviderSubmission>()
            {
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1516", CollectionReturnCode = "" },
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1617", CollectionReturnCode = "R13"  },
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1718", CollectionReturnCode = "R14"  },
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R14"  },
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1920", CollectionReturnCode = "R02"  },
            }.AsQueryable().BuildMock();

            var esfFundingData = new List<ESFFundingDataSummarised>()
            {
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1516, CollectionPeriod = 0, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-2", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1516, CollectionPeriod = 0, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1617, CollectionPeriod = 13, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1617, CollectionPeriod = 14, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1718, CollectionPeriod = 1, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1718, CollectionPeriod = 5, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1718, CollectionPeriod = 12, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1718, CollectionPeriod = 14, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 1, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 2, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 3, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 4, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 5, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 6, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 7, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 8, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 9, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 10, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 12, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 13, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 14, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1920, CollectionPeriod = 1, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1920, CollectionPeriod = 2, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
            }.AsQueryable().BuildMock();

            var expectedProviders = new List<int> { 1000825, 1009876 };

            var esfFundingDataContextMock = new Mock<IESFFundingDataContext>();

            esfFundingDataContextMock
                .Setup(x => x.LatestProviderSubmissions)
                .Returns(latestProviderSubmissions.Object);

            esfFundingDataContextMock
                .Setup(x => x.ESFFundingDatasSummarised)
                .Returns(esfFundingData.Object);

            var summarisationMessageMock = new Mock<ISummarisationMessage>();

            summarisationMessageMock.SetupGet(s => s.Ukprn).Returns(1000825);
            summarisationMessageMock.SetupGet(s => s.CollectionYear).Returns(1920);
            summarisationMessageMock.SetupGet(s => s.CollectionMonth).Returns(2);

            var result = await NewProvider(() => esfFundingDataContextMock.Object)
                .ProvideAsync(1000825, summarisationMessageMock.Object, CancellationToken.None);

            result.Count().Should().Be(6);
        }

        [Fact]
        public async Task ProvideAsync_Check_R13Removed()
        {
            var latestProviderSubmissions = new List<Query.LatestProviderSubmission>()
            {
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1516", CollectionReturnCode = "" },
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1617", CollectionReturnCode = "R13"  },
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1718", CollectionReturnCode = "R14"  },
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1819", CollectionReturnCode = "R13"  },
                new Query.LatestProviderSubmission() { UKPRN = 1000825, CollectionType = "ILR1920", CollectionReturnCode = "R02"  },
            }.AsQueryable().BuildMock();

            var esfFundingData = new List<ESFFundingDataSummarised>()
            {
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1516, CollectionPeriod = 0, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-2", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1516, CollectionPeriod = 0, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1617, CollectionPeriod = 13, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1617, CollectionPeriod = 14, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1718, CollectionPeriod = 1, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1718, CollectionPeriod = 5, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1718, CollectionPeriod = 12, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1718, CollectionPeriod = 14, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 1, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 2, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 3, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 4, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 5, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 6, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 7, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 8, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 9, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 10, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1819, CollectionPeriod = 12, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },

                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1920, CollectionPeriod = 1, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
                new ESFFundingDataSummarised() { UKPRN = 1000825, ConRefNumber = "Conref-1", DeliverableCode = "D01", AttributeName= "Att1", CollectionYear = 1920, CollectionPeriod = 2, Period_1 = 10.10M, Period_2 = 20.20M, Period_3 = 30.30M, Period_4 = 40.40M, Period_5 = 50.50M, Period_6 = 60.60M, Period_7 = 70.70M, Period_8 = 80.80M, Period_9 = 90.90M, Period_10 = 100.0M, Period_11 = 110.0M, Period_12 = 120.0M },
            }.AsQueryable().BuildMock();

            var expectedProviders = new List<int> { 1000825, 1009876 };

            var esfFundingDataContextMock = new Mock<IESFFundingDataContext>();

            esfFundingDataContextMock
                .Setup(x => x.LatestProviderSubmissions)
                .Returns(latestProviderSubmissions.Object);

            esfFundingDataContextMock
                .Setup(x => x.ESFFundingDatasSummarised)
                .Returns(esfFundingData.Object);

            var summarisationMessageMock = new Mock<ISummarisationMessage>();

            summarisationMessageMock.SetupGet(s => s.Ukprn).Returns(1000825);
            summarisationMessageMock.SetupGet(s => s.CollectionYear).Returns(1920);
            summarisationMessageMock.SetupGet(s => s.CollectionMonth).Returns(2);

            var result = await NewProvider(() => esfFundingDataContextMock.Object)
                .ProvideAsync(1000825, summarisationMessageMock.Object, CancellationToken.None);

            result.Count().Should().Be(5);
        }

        private ESFILRProvider NewProvider(Func<IESFFundingDataContext> esfFundingDataContext = null)
        {
            return new ESFILRProvider(esfFundingDataContext);
        }*/
    }
}
