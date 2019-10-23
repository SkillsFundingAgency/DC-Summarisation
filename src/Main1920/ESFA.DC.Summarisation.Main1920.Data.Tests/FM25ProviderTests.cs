using ESFA.DC.ILR1920.DataStore.EF;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Constants;
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
    public class FM25ProviderTests
    {
        [Fact]
        public void SummarisationType_Check()
        {
            NewProvider().SummarisationType.Should().Be("Main1920_FM25");
        }

        [Fact]
        public void SummarisationType_CheckWithConstantValue()
        {
            NewProvider().SummarisationType.Should().Be(SummarisationTypeConstants.Main1920_FM25);
        }

        [Fact]
        public void CollectionType_Check()
        {
            NewProvider().CollectionType.Should().Be("ILR1920");
        }

        [Fact]
        public void CollectionType_CheckWithConstantValue()
        {
            NewProvider().CollectionType.Should().Be(CollectionTypeConstants.ILR1920);
        }

        [Fact]
        public async Task ProvideUkprnsAsync_Check()
        {
            var FM25Learners = new List<FM25_Learner>()
            {
                new FM25_Learner() { UKPRN = 1000825 },
                new FM25_Learner() { UKPRN = 1009876 },
                new FM25_Learner() { UKPRN = 145800 },
                new FM25_Learner() { UKPRN = 1000825 }
            }.AsQueryable().BuildMock();

            var expectedLearners = new List<int> { 1000825, 1009876, 145800 };

            var ilr1920ContextMock = new Mock<IIlr1920RulebaseContext>();
            ilr1920ContextMock
                .Setup(x => x.FM25_Learners)
                .Returns(FM25Learners.Object);

            var result = await NewProvider(() => ilr1920ContextMock.Object)
                .ProvideUkprnsAsync(CancellationToken.None);
            result.Count().Should().Be(3);
            result.Should().BeEquivalentTo(expectedLearners);
        }

        [Fact]
        public async Task ProvideUkprnsAsync_EmptyCheck()
        {
            var FM25Learners = new List<FM25_Learner>()
            {
            }.AsQueryable().BuildMockDbSet();

            var ilr1920ContextMock = new Mock<IIlr1920RulebaseContext>();
            ilr1920ContextMock
                .Setup(x => x.FM25_Learners)
                .Returns(FM25Learners.Object);

            var result = await NewProvider(() => ilr1920ContextMock.Object)
                .ProvideUkprnsAsync(CancellationToken.None);
            result.Count().Should().Be(0);
            result.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task ProvideAsync_Check()
        {
            int ukprn = 12456007;

            var FM25PeriodisedValues = new FM25_FM35_Learner_PeriodisedValue()
            {
                UKPRN = ukprn,
                LearnRefNumber = "9900066102",
                AttributeName = "LnrOnProgPay",
                Period_1 = 14.0M,
                Period_2 = 128.0M,
                Period_3 = 109.0M,
                Period_4 = 151.0M,
                Period_5 = 38.0M,
                Period_6 = 76.0M,
                Period_7 = 82.0M,
                Period_8 = 102.0M,
                Period_9 = 87.0M,
                Period_10 = 43.0M,
                Period_11 = 7.0M,
                Period_12 = 8.0M
            };

            var FM25Learners = new List<FM25_Learner>()
            {
                new FM25_Learner()
                {
                    UKPRN = ukprn,
                    LearnRefNumber = "9900066102",
                    FundLine = "16-18 Traineeships (Adult funded)",
                    FM25_FM35_Learner_PeriodisedValues = new FM25_FM35_Learner_PeriodisedValue[]
                    {
                        FM25PeriodisedValues,
                        new FM25_FM35_Learner_PeriodisedValue()
                        {
                            UKPRN = 1000083,
                            LearnRefNumber = "9900066103",
                            AttributeName = "LnrOnProgPay",
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
            }.AsQueryable().BuildMockDbSet();

            var ilr1920ContextMock = new Mock<IIlr1920RulebaseContext>();
            ilr1920ContextMock
                .Setup(x => x.FM25_Learners)
                .Returns(FM25Learners.Object);

            var result = await NewProvider(() => ilr1920ContextMock.Object)
                .ProvideAsync(ukprn, null, CancellationToken.None);
            result.Count().Should().Be(1);
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ProvideAsync_CheckRecordNotFound()
        {
            int ukprn = 1000856;

            var FM25Learners = new List<FM25_Learner>()
            {
                new FM25_Learner()
                {
                    UKPRN = 1000082,
                    LearnRefNumber = "9900066103",
                    FundLine = "19+ Traineeships (Adult funded)",
                    FM25_FM35_Learner_PeriodisedValues = new FM25_FM35_Learner_PeriodisedValue[]
                    {
                        new FM25_FM35_Learner_PeriodisedValue()
                        {
                            UKPRN = 1000082,
                            LearnRefNumber = "9900066103",
                            AttributeName = "LnrOnProgPay",
                            Period_1 = 25.0M,
                            Period_2 = 102.0M,
                            Period_3 = 87.0M,
                            Period_4 = 90.0M,
                            Period_5 = 108.0M,
                            Period_6 = 23.0M,
                            Period_7 = 45.0M,
                            Period_8 = 55.0M,
                            Period_9 = 77.0M,
                            Period_10 = 240.0M,
                            Period_11 = 61.0M,
                            Period_12 = 78.0M
                        }
                    }
                }
            }.AsQueryable().BuildMockDbSet();

            var ilr1920ContextMock = new Mock<IIlr1920RulebaseContext>();
            ilr1920ContextMock
                .Setup(x => x.FM25_Learners)
                .Returns(FM25Learners.Object);

            var result = await NewProvider(() => ilr1920ContextMock.Object)
                .ProvideAsync(ukprn,null, CancellationToken.None);
            result.Count().Should().Be(0);
            result.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task ProvideAsync_CheckEmpty()
        {
            int ukprn = 1000876;

            var FM25Learners = new List<FM25_Learner>()
            { }.AsQueryable().BuildMockDbSet();

            var ilrContextMock = new Mock<IIlr1920RulebaseContext>();
            ilrContextMock
                .Setup(x => x.FM25_Learners)
                .Returns(FM25Learners.Object);

            var result = await NewProvider(() => ilrContextMock.Object)
                .ProvideAsync(ukprn, null, CancellationToken.None);

            result.Count().Should().Be(0);
            result.Should().BeNullOrEmpty();
        }

        private Fm25Provider NewProvider(Func<IIlr1920RulebaseContext> ilrRulebaseContext = null)
        {
            return new Fm25Provider(ilrRulebaseContext);
        }
    }
}
