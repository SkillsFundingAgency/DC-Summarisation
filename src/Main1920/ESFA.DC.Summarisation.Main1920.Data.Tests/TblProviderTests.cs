using ESFA.DC.ILR1920.DataStore.EF;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Main1920.Data.Providers;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.Summarisation.Main1920.Data.Tests
{
    public class TblProviderTests
    {
        [Fact]
        public async Task ProvideAsync_Check()
        {
            int ukprn = 10000055;
            var expectedLearningDelivery = new TBL_LearningDelivery()
            {
                UKPRN = ukprn,
                LearnRefNumber = "9000579502",
                AimSeqNumber = 1,
                FundLine = "16-18 Trailblazer Apprenticeship",
                TBL_LearningDelivery_PeriodisedValues = new TBL_LearningDelivery_PeriodisedValue[]
                        {
                            new TBL_LearningDelivery_PeriodisedValue()
                            {
                                LearnRefNumber = "9000579502",
                                AimSeqNumber = 1,
                                UKPRN = ukprn,
                                AttributeName = "MathEngBalPct",
                                Period_1 = 1.0M,
                                Period_2 = 2.0M,
                                Period_3 = 3.0M,
                                Period_4 = 4.0M,
                                Period_5 = 5.0M,
                                Period_6 = 6.0M,
                                Period_7 = 7.0M,
                                Period_8 = 8.0M,
                                Period_9 = 9.0M,
                                Period_10 = 10.0M,
                                Period_11 = 11.0M,
                                Period_12 = -120.0M,
                            }
                        }
            };

            var learningDeliveries = new List<TBL_LearningDelivery>
                {
                    expectedLearningDelivery,
                    new TBL_LearningDelivery()
                    {
                        UKPRN = 10000056,
                        LearnRefNumber = "9000579804",
                        AimSeqNumber = 2,
                        FundLine = "19-23 Trailblazer Apprenticeship",
                        TBL_LearningDelivery_PeriodisedValues = new TBL_LearningDelivery_PeriodisedValue[]
                        {
                            new TBL_LearningDelivery_PeriodisedValue()
                            {
                                LearnRefNumber = "9000579804",
                                AimSeqNumber = 2,
                                UKPRN = 10000056,
                                AttributeName = "LearnSuppFund",
                                Period_1 = 1.0M,
                                Period_2 = 1.0M,
                                Period_3 = 1.0M,
                                Period_4 = 1.0M,
                                Period_5 = 1.0M,
                                Period_6 = 1.0M,
                                Period_7 = 1.0M,
                                Period_8 = 1.0M,
                                Period_9 = 1.0M,
                                Period_10 = 1.0M,
                                Period_11 = 1.0M,
                                Period_12 = 1.0M,
                            }
                        }
                    }
                }.AsQueryable().BuildMock();

            var mockILRContext = new Mock<IIlr1920RulebaseContext>();
            mockILRContext
                .Setup(x => x.TBL_LearningDeliveries)
                .Returns(learningDeliveries.Object);

            var result = await NewProvider(() => mockILRContext.Object).ProvideAsync(ukprn, CancellationToken.None);
            result.Count().Should().Be(1);
            result[0].PeriodisedData.Count().Should().Be(1);
            result.Should().NotBeNullOrEmpty();
        }

        private TblProvider NewProvider(Func<IIlr1920RulebaseContext> ilrContext = null)
        {
            return new TblProvider(ilrContext);
        }
    }
}
