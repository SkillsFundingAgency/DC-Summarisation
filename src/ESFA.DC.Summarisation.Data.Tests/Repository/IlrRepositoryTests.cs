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

        [Fact]
        public async Task RetrieveSingleFm35ProvidersTest()
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
                    UKPRN = 10000000,
                    LearnRefNumber = "10000000Learner2",
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

            var service = new Fm35Repository(ilrMock.Object);

            var providers = await service.RetrieveProvidersAsync(CancellationToken.None);

            providers.Count.Should().Be(1);
            providers.FirstOrDefault(x => x.UKPRN == 10000000)
                ?.LearningDeliveries.Count.Should().Be(2);

            providers.FirstOrDefault(x => x.UKPRN == 10000000)
                ?.LearningDeliveries.FirstOrDefault(ld => ld.LearnRefNumber == "10000000Learner1")
                ?.PeriodisedData.Count.Should().Be(1);

            providers.FirstOrDefault(x => x.UKPRN == 10000000)
                ?.LearningDeliveries.FirstOrDefault(ld => ld.LearnRefNumber == "10000000Learner1")
                ?.PeriodisedData.FirstOrDefault()
                ?.Periods.Count.Should().Be(12);
        }

        [Fact]
        public async Task RetrieveMultipleFm35ProvidersTest()
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
                    UKPRN = 10000000,
                    LearnRefNumber = "10000000Learner2",
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

            var service = new Fm35Repository(ilrMock.Object);

            var providers = await service.RetrieveProvidersAsync(CancellationToken.None);

            providers.Count.Should().Be(1);
            providers.FirstOrDefault(x => x.UKPRN == 10000000)
                ?.LearningDeliveries.Count.Should().Be(2);

            providers.FirstOrDefault(x => x.UKPRN == 10000000)
                ?.LearningDeliveries.FirstOrDefault(ld => ld.LearnRefNumber == "10000000Learner1")
                ?.PeriodisedData.Count.Should().Be(1);

            providers.FirstOrDefault(x => x.UKPRN == 10000000)
                ?.LearningDeliveries.FirstOrDefault(ld => ld.LearnRefNumber == "10000000Learner1")
                ?.PeriodisedData.FirstOrDefault()
                ?.Periods.Count.Should().Be(12);
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

            var ilrMock = new Mock<IIlr1819RulebaseContext>();
            ilrMock
                .Setup(s => s.FM35_Learners)
                .Returns(fm35Learners.Object);

            var service = new Fm35Repository(ilrMock.Object);

            var providers = await service.RetrieveProvidersAsync(1, 2, CancellationToken.None);

            providers.Count.Should().Be(1);

            providers = await service.RetrieveProvidersAsync(2, 2, CancellationToken.None);

            providers.Count.Should().Be(1);
        }

        [Fact]
        public async Task RetrieveSingleFm35ProviderByUkprnTest()
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
                    LearnRefNumber = "10000001Learner1",
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

            var service = new Fm35Repository(ilrMock.Object);

            var providers = await service.RetrieveProvidersAsync(10000000, CancellationToken.None);

            providers.Count.Should().Be(1);
            providers.FirstOrDefault(x => x.UKPRN == 10000000)
                ?.LearningDeliveries.Count.Should().Be(1);

            providers.FirstOrDefault(x => x.UKPRN == 10000000)
                ?.LearningDeliveries.FirstOrDefault(ld => ld.LearnRefNumber == "10000000Learner1")
                ?.PeriodisedData.Count.Should().Be(1);

            providers.FirstOrDefault(x => x.UKPRN == 10000000)
                ?.LearningDeliveries.FirstOrDefault(ld => ld.LearnRefNumber == "10000000Learner1")
                ?.PeriodisedData.FirstOrDefault()
                ?.Periods.Count.Should().Be(12);
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(100, 10)]
        [InlineData(1000, 100)]
        //[InlineData(1000, 1000)] out of memory exception
        public async Task CreateBulkMockFm35LearnerData(int numOfProviders, int learnersPerProvider)
        {
            var learners = new List<FM35_Learner>();

            for (int i = 0; i < numOfProviders; i++)
            {
                var ukprn = 10000000 + i;

                for (int l = 0; l < learnersPerProvider; l++)
                {
                    var learnRefNumber = $"{ukprn}Learner{l}";

                    learners.Add(new FM35_Learner
                    {
                        UKPRN = ukprn,
                        LearnRefNumber = learnRefNumber,
                        FM35_LearningDeliveries = new List<FM35_LearningDelivery>
                        {
                            new FM35_LearningDelivery
                            {
                                FundLine = $"FundLine{i}",
                                AimSeqNumber = 1,
                                LearnRefNumber = learnRefNumber,
                                FM35_LearningDelivery_PeriodisedValues = new List<FM35_LearningDelivery_PeriodisedValue>
                                {
                                    new FM35_LearningDelivery_PeriodisedValue
                                    {
                                        AttributeName = "Attribute1",
                                        Period_1 = 10,
                                        Period_2 = 20,
                                        Period_3 = 30,
                                        Period_4 = 40,
                                        Period_5 = 50,
                                        Period_6 = 60,
                                        Period_7 = 70,
                                        Period_8 = 80,
                                        Period_9 = 90,
                                        Period_10 = 100,
                                        Period_11 = 110,
                                        Period_12 = 120
                                    },
                                    new FM35_LearningDelivery_PeriodisedValue
                                    {
                                        AttributeName = "Attribute2",
                                        Period_1 = 10,
                                        Period_2 = 20,
                                        Period_3 = 30,
                                        Period_4 = 40,
                                        Period_5 = 50,
                                        Period_6 = 60,
                                        Period_7 = 70,
                                        Period_8 = 80,
                                        Period_9 = 90,
                                        Period_10 = 100,
                                        Period_11 = 110,
                                        Period_12 = 120
                                    },
                                }
                            }
                        }
                    });
                }
            }

            var learnersMock = learners.AsQueryable().BuildMock();

            var ilrMock = new Mock<IIlr1819RulebaseContext>();
            ilrMock
                .Setup(s => s.FM35_Learners)
                .Returns(learnersMock.Object);

            var service = new Fm35Repository(ilrMock.Object);

            var stopWatch = new Stopwatch();

            stopWatch.Start();
            var providers = await service.RetrieveProvidersAsync(CancellationToken.None);
            stopWatch.Stop();

            _outputHelper.WriteLine($"Projecting {numOfProviders} of Providers with {learnersPerProvider} learners each took {stopWatch.Elapsed}");
        }
    }
}
