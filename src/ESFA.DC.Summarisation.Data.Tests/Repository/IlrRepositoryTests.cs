using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Data.Repository;
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
            var fm35Learners = new List<Fm35Learner>
            {
                new Fm35Learner
                {
                    Ukprn = 10000000,
                    LearnRefNumber = "10000000Learner1",
                    Fm35LearningDeliveries = new List<Fm35LearningDelivery>
                    {
                        new Fm35LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            LearnRefNumber = "10000000Learner1",
                            Fm35LearningDeliveryPeriodisedValues = new List<Fm35LearningDeliveryPeriodisedValue>
                            {
                                new Fm35LearningDeliveryPeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period1 = 10,
                                    Period2 = 20,
                                    Period3 = 30,
                                    Period4 = 40
                                }
                            }
                        }
                    }
                },
                new Fm35Learner
                {
                    Ukprn = 10000000,
                    LearnRefNumber = "10000000Learner2",
                    Fm35LearningDeliveries = new List<Fm35LearningDelivery>
                    {
                        new Fm35LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            Fm35LearningDeliveryPeriodisedValues = new List<Fm35LearningDeliveryPeriodisedValue>
                            {
                                new Fm35LearningDeliveryPeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period1 = 10,
                                    Period2 = 20,
                                    Period3 = 30,
                                    Period4 = 40
                                }
                            }
                        }
                    }
                }
            }.AsQueryable().BuildMock();

            var ilrMock = new Mock<IIlr1819RulebaseContext>();
            ilrMock
                .Setup(s => s.Fm35Learners)
                .Returns(fm35Learners.Object);

            var service = new IlrRepository(ilrMock.Object);

            var providers = await service.RetrieveFM35ProvidersAsync(CancellationToken.None);

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
            var fm35Learners = new List<Fm35Learner>
            {
                new Fm35Learner
                {
                    Ukprn = 10000000,
                    LearnRefNumber = "10000000Learner1",
                    Fm35LearningDeliveries = new List<Fm35LearningDelivery>
                    {
                        new Fm35LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            LearnRefNumber = "10000000Learner1",
                            Fm35LearningDeliveryPeriodisedValues = new List<Fm35LearningDeliveryPeriodisedValue>
                            {
                                new Fm35LearningDeliveryPeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period1 = 10,
                                    Period2 = 20,
                                    Period3 = 30,
                                    Period4 = 40
                                }
                            }
                        }
                    }
                },
                new Fm35Learner
                {
                    Ukprn = 10000000,
                    LearnRefNumber = "10000000Learner2",
                    Fm35LearningDeliveries = new List<Fm35LearningDelivery>
                    {
                        new Fm35LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            Fm35LearningDeliveryPeriodisedValues = new List<Fm35LearningDeliveryPeriodisedValue>
                            {
                                new Fm35LearningDeliveryPeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period1 = 10,
                                    Period2 = 20,
                                    Period3 = 30,
                                    Period4 = 40
                                }
                            }
                        }
                    }
                }
            }.AsQueryable().BuildMock();

            var ilrMock = new Mock<IIlr1819RulebaseContext>();
            ilrMock
                .Setup(s => s.Fm35Learners)
                .Returns(fm35Learners.Object);

            var service = new IlrRepository(ilrMock.Object);

            var providers = await service.RetrieveFM35ProvidersAsync(CancellationToken.None);

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
            var fm35Learners = new List<Fm35Learner>
            {
                new Fm35Learner
                {
                    Ukprn = 10000000,
                    LearnRefNumber = "10000000Learner1",
                    Fm35LearningDeliveries = new List<Fm35LearningDelivery>
                    {
                        new Fm35LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            LearnRefNumber = "10000000Learner1",
                            Fm35LearningDeliveryPeriodisedValues = new List<Fm35LearningDeliveryPeriodisedValue>
                            {
                                new Fm35LearningDeliveryPeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period1 = 10,
                                    Period2 = 20,
                                    Period3 = 30,
                                    Period4 = 40
                                }
                            }
                        }
                    }
                },
                new Fm35Learner
                {
                    Ukprn = 10000001,
                    LearnRefNumber = "10000001Learner2",
                    Fm35LearningDeliveries = new List<Fm35LearningDelivery>
                    {
                        new Fm35LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            Fm35LearningDeliveryPeriodisedValues = new List<Fm35LearningDeliveryPeriodisedValue>
                            {
                                new Fm35LearningDeliveryPeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period1 = 10,
                                    Period2 = 20,
                                    Period3 = 30,
                                    Period4 = 40
                                }
                            }
                        }
                    }
                },
                new Fm35Learner
                {
                    Ukprn = 10000002,
                    LearnRefNumber = "10000002Learner2",
                    Fm35LearningDeliveries = new List<Fm35LearningDelivery>
                    {
                        new Fm35LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            Fm35LearningDeliveryPeriodisedValues = new List<Fm35LearningDeliveryPeriodisedValue>
                            {
                                new Fm35LearningDeliveryPeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period1 = 10,
                                    Period2 = 20,
                                    Period3 = 30,
                                    Period4 = 40
                                }
                            }
                        }
                    }
                }
            }.AsQueryable().BuildMock();

            var ilrMock = new Mock<IIlr1819RulebaseContext>();
            ilrMock
                .Setup(s => s.Fm35Learners)
                .Returns(fm35Learners.Object);

            var service = new IlrRepository(ilrMock.Object);

            var providers = await service.RetrieveFM35ProvidersAsync(1, 2, CancellationToken.None);

            providers.Count.Should().Be(2);

            providers = await service.RetrieveFM35ProvidersAsync(2, 2, CancellationToken.None);

            providers.Count.Should().Be(1);
        }

        [Fact]
        public async Task RetrieveSingleFm35ProviderByUkprnTest()
        {
            var fm35Learners = new List<Fm35Learner>
            {
                new Fm35Learner
                {
                    Ukprn = 10000000,
                    LearnRefNumber = "10000000Learner1",
                    Fm35LearningDeliveries = new List<Fm35LearningDelivery>
                    {
                        new Fm35LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            LearnRefNumber = "10000000Learner1",
                            Fm35LearningDeliveryPeriodisedValues = new List<Fm35LearningDeliveryPeriodisedValue>
                            {
                                new Fm35LearningDeliveryPeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period1 = 10,
                                    Period2 = 20,
                                    Period3 = 30,
                                    Period4 = 40
                                }
                            }
                        }
                    }
                },
                new Fm35Learner
                {
                    Ukprn = 10000001,
                    LearnRefNumber = "10000001Learner1",
                    Fm35LearningDeliveries = new List<Fm35LearningDelivery>
                    {
                        new Fm35LearningDelivery
                        {
                            FundLine = "FundLine1",
                            AimSeqNumber = 1,
                            Fm35LearningDeliveryPeriodisedValues = new List<Fm35LearningDeliveryPeriodisedValue>
                            {
                                new Fm35LearningDeliveryPeriodisedValue
                                {
                                    AttributeName = "Attribute1",
                                    Period1 = 10,
                                    Period2 = 20,
                                    Period3 = 30,
                                    Period4 = 40
                                }
                            }
                        }
                    }
                }
            }.AsQueryable().BuildMock();

            var ilrMock = new Mock<IIlr1819RulebaseContext>();
            ilrMock
                .Setup(s => s.Fm35Learners)
                .Returns(fm35Learners.Object);

            var service = new IlrRepository(ilrMock.Object);

            var providers = await service.RetrieveFM35ProvidersAsync(10000000, CancellationToken.None);

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
        [InlineData(1000, 1000)]
        public async Task CreateBulkMockFm35LearnerData(int numOfProviders, int learnersPerProvider)
        {
            var learners = new List<Fm35Learner>();

            for (int i = 0; i < numOfProviders; i++)
            {
                var ukprn = 10000000 + i;

                for (int l = 0; l < learnersPerProvider; l++)
                {
                    var learnRefNumber = $"{ukprn}Learner{l}";

                    learners.Add(new Fm35Learner
                    {

                        Ukprn = ukprn,
                        LearnRefNumber = learnRefNumber,
                        Fm35LearningDeliveries = new List<Fm35LearningDelivery>
                        {
                            new Fm35LearningDelivery
                            {
                                FundLine = $"FundLine{i}",
                                AimSeqNumber = 1,
                                LearnRefNumber = learnRefNumber,
                                Fm35LearningDeliveryPeriodisedValues = new List<Fm35LearningDeliveryPeriodisedValue>
                                {
                                    new Fm35LearningDeliveryPeriodisedValue
                                    {
                                        AttributeName = "Attribute1",
                                        Period1 = 10,
                                        Period2 = 20,
                                        Period3 = 30,
                                        Period4 = 40,
                                        Period5 = 50,
                                        Period6 = 60,
                                        Period7 = 70,
                                        Period8 = 80,
                                        Period9 = 90,
                                        Period10 = 100,
                                        Period11 = 110,
                                        Period12 = 120
                                    },
                                    new Fm35LearningDeliveryPeriodisedValue
                                    {
                                        AttributeName = "Attribute2",
                                        Period1 = 10,
                                        Period2 = 20,
                                        Period3 = 30,
                                        Period4 = 40,
                                        Period5 = 50,
                                        Period6 = 60,
                                        Period7 = 70,
                                        Period8 = 80,
                                        Period9 = 90,
                                        Period10 = 100,
                                        Period11 = 110,
                                        Period12 = 120
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
                .Setup(s => s.Fm35Learners)
                .Returns(learnersMock.Object);

            var service = new IlrRepository(ilrMock.Object);

            var stopWatch = new Stopwatch();

            stopWatch.Start();
            var providers = await service.RetrieveFM35ProvidersAsync(CancellationToken.None);
            stopWatch.Stop();

            _outputHelper.WriteLine($"Projecting {numOfProviders} of Providers with {learnersPerProvider} learners each took {stopWatch.Elapsed}");
        }
    }
}
