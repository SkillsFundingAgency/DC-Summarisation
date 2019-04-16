using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Main1819.Data.Repository;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace ESFA.DC.Summarisation.Data.Tests.Repository
{
    public class IlrRepositoryTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public IlrRepositoryTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Theory]
        [InlineData(10, 1)]
        [InlineData(2, 2)]
        [InlineData(1, 3)]
        public async Task RetrieveFm35ProvidersCountTest(int pageSize, int expectedPages)
        {
            var fm35Learners = new List<FM35_Learner>
            {
                new FM35_Learner
                {
                    UKPRN = 10000000,
                    LearnRefNumber = "10000000Learner1",
                    FM35_LearningDeliveries = new List<FM35_LearningDelivery>
                    {
                        new FM35_LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            LearnRefNumber = "10000000Learner1",
                            FM35_LearningDelivery_PeriodisedValues = new List<FM35_LearningDelivery_PeriodisedValue>
                            {
                                new FM35_LearningDelivery_PeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period_1 = 10,
                                    Period_2 = 20,
                                    Period_3 = 30,
                                    Period_4 = 40
                                }
                            }
                        }
                    }
                },
                new FM35_Learner
                {
                    UKPRN = 10000001,
                    LearnRefNumber = "10000001Learner2",
                    FM35_LearningDeliveries = new List<FM35_LearningDelivery>
                    {
                        new FM35_LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            FM35_LearningDelivery_PeriodisedValues = new List<FM35_LearningDelivery_PeriodisedValue>
                            {
                                new FM35_LearningDelivery_PeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period_1 = 10,
                                    Period_2 = 20,
                                    Period_3 = 30,
                                    Period_4 = 40
                                }
                            }
                        }
                    }
                },
                new FM35_Learner
                {
                    UKPRN = 10000002,
                    LearnRefNumber = "10000002Learner2",
                    FM35_LearningDeliveries = new List<FM35_LearningDelivery>
                    {
                        new FM35_LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            FM35_LearningDelivery_PeriodisedValues = new List<FM35_LearningDelivery_PeriodisedValue>
                            {
                                new FM35_LearningDelivery_PeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period_1 = 10,
                                    Period_2 = 20,
                                    Period_3 = 30,
                                    Period_4 = 40
                                }
                            }
                        }
                    }
                }
            }.AsQueryable().BuildMock();

            var ilrMock = new Mock<IIlr1819RulebaseContext>();
            ilrMock
                .Setup(s => s.FM35_Learners)
                .Returns(fm35Learners.Object);

            //var service = new Fm35Repository(ilrMock.Object);

            //var result = await service.RetrieveProviderPageCountAsync(pageSize, CancellationToken.None);

            //result.Should().Be(expectedPages);
        }

        [Fact]
        public async Task RetrieveFm35ProvidersPagingTest()
        {
            var fm35Learners = new List<FM35_Learner>
            {
                new FM35_Learner
                {
                    UKPRN = 10000000,
                    LearnRefNumber = "10000000Learner1",
                    FM35_LearningDeliveries = new List<FM35_LearningDelivery>
                    {
                        new FM35_LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            LearnRefNumber = "10000000Learner1",
                            FM35_LearningDelivery_PeriodisedValues = new List<FM35_LearningDelivery_PeriodisedValue>
                            {
                                new FM35_LearningDelivery_PeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period_1 = 10,
                                    Period_2 = 20,
                                    Period_3 = 30,
                                    Period_4 = 40
                                }
                            }
                        }
                    }
                },
                new FM35_Learner
                {
                    UKPRN = 10000001,
                    LearnRefNumber = "10000001Learner2",
                    FM35_LearningDeliveries = new List<FM35_LearningDelivery>
                    {
                        new FM35_LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            FM35_LearningDelivery_PeriodisedValues = new List<FM35_LearningDelivery_PeriodisedValue>
                            {
                                new FM35_LearningDelivery_PeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period_1 = 10,
                                    Period_2 = 20,
                                    Period_3 = 30,
                                    Period_4 = 40
                                }
                            }
                        }
                    }
                },
                new FM35_Learner
                {
                    UKPRN = 10000002,
                    LearnRefNumber = "10000002Learner2",
                    FM35_LearningDeliveries = new List<FM35_LearningDelivery>
                    {
                        new FM35_LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            FM35_LearningDelivery_PeriodisedValues = new List<FM35_LearningDelivery_PeriodisedValue>
                            {
                                new FM35_LearningDelivery_PeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period_1 = 10,
                                    Period_2 = 20,
                                    Period_3 = 30,
                                    Period_4 = 40
                                }
                            }
                        }
                    }
                }
            }.AsQueryable().BuildMock();

            //var ilrMock = new Mock<IIlr1819RulebaseContext>();
            //ilrMock
            //    .Setup(s => s.FM35_Learners)
            //    .Returns(fm35Learners.Object);

            //var service = new Fm35Repository(ilrMock.Object);

            //var providers = await service.RetrieveProvidersAsync(1, 2, CancellationToken.None);

            //providers.Count.Should().Be(2);

            //providers = await service.RetrieveProvidersAsync(2, 2, CancellationToken.None);

            //providers.Count.Should().Be(1);
        }
    }
}
