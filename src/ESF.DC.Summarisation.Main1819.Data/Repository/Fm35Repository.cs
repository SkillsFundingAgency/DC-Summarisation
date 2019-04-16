using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Main1819.Data.Repository
{
    public class Fm35Repository : IProviderRepository
    {
        private readonly IIlr1819RulebaseContext _ilr;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.Main1819_FM35);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.ILR1819);

        public Fm35Repository(IIlr1819RulebaseContext ilr)
        {
               _ilr = ilr;
        }

        public async Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(CancellationToken cancellationToken)
        {
            return await _ilr.FM35_Learners
                .GroupBy(l => l.UKPRN)
                .Select(l => new Provider
                {
                    UKPRN = l.Key,
                    LearningDeliveries = l.SelectMany(ld => ld.FM35_LearningDeliveries
                        .Select(
                            ldd => new LearningDelivery
                            {
                                LearnRefNumber = ldd.LearnRefNumber,
                                AimSeqNumber = ldd.AimSeqNumber,
                                Fundline = ldd.FundLine,
                                PeriodisedData = ldd.FM35_LearningDelivery_PeriodisedValues
                                    .GroupBy(pv => pv.AttributeName)
                                    .Select(group => new PeriodisedData
                                    {
                                        AttributeName = group.Key,
                                        Periods = group.SelectMany(UnflattenToPeriod).ToList()
                                    } as IPeriodisedData).ToList()
                            } as ILearningDelivery)).ToList()
                }).ToListAsync(cancellationToken);
        }

        public async Task<int> RetrieveProviderPageCountAsync(int pageSize, CancellationToken cancellationToken)
        {
            var providerCount = await _ilr.FM35_Learners
                  .GroupBy(l => l.UKPRN)
                  .CountAsync(cancellationToken);

            return (providerCount % pageSize) > 0
                ? (providerCount / pageSize) + 1
                : (providerCount / pageSize);
            
        }

        public async Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(int pageSize, int pageNumber, CancellationToken cancellationToken)
        {
            return await _ilr.FM35_Learners
                .GroupBy(l => l.UKPRN)
                .OrderBy(o => o.Key)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new Provider
                {
                    UKPRN = l.Key,
                    LearningDeliveries = l.SelectMany(ld => ld.FM35_LearningDeliveries
                        .Select(
                            ldd => new LearningDelivery
                            {
                                LearnRefNumber = ldd.LearnRefNumber,
                                AimSeqNumber = ldd.AimSeqNumber,
                                Fundline = ldd.FundLine,
                                PeriodisedData = ldd.FM35_LearningDelivery_PeriodisedValues
                                    .GroupBy(pv => pv.AttributeName)
                                    .Select(group => new PeriodisedData
                                    {
                                        AttributeName = group.Key,
                                        Periods = group.SelectMany(pd => UnflattenToPeriod(pd)).ToList()
                                    } as IPeriodisedData).ToList()
                            } as ILearningDelivery)).ToList()
                }).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(int Ukprn, CancellationToken cancellationToken)
        {
            return await _ilr.FM35_Learners
                .Where(p => p.UKPRN == Ukprn)
                .Select(l => new Provider
                {
                    UKPRN = l.UKPRN,
                    LearningDeliveries = l.FM35_LearningDeliveries
                        .Select(
                            ldd => new LearningDelivery
                            {
                                LearnRefNumber = ldd.LearnRefNumber,
                                AimSeqNumber = ldd.AimSeqNumber,
                                Fundline = ldd.FundLine,
                                PeriodisedData = ldd.FM35_LearningDelivery_PeriodisedValues
                                    .GroupBy(pv => pv.AttributeName)
                                    .Select(group => new PeriodisedData
                                    {
                                        AttributeName = group.Key,
                                        Periods = group.SelectMany(UnflattenToPeriod).ToList()
                                    } as IPeriodisedData).ToList()
                            } as ILearningDelivery).ToList()
                }).ToListAsync(cancellationToken);
        }

        private IEnumerable<IPeriod> UnflattenToPeriod(FM35_LearningDelivery_PeriodisedValue values)
        {
            return new List<Period>
            {
                new Period
                {
                    PeriodId = 1,
                    Value = values.Period_1
                },
                new Period
                {
                    PeriodId = 2,
                    Value = values.Period_2
                },
                new Period
                {
                    PeriodId = 3,
                    Value = values.Period_3
                },
                new Period
                {
                    PeriodId = 4,
                    Value = values.Period_4
                },
                new Period
                {
                    PeriodId = 5,
                    Value = values.Period_5
                },
                new Period
                {
                    PeriodId = 6,
                    Value = values.Period_6
                },
                new Period
                {
                    PeriodId = 7,
                    Value = values.Period_7
                },
                new Period
                {
                    PeriodId = 8,
                    Value = values.Period_8
                },
                new Period
                {
                    PeriodId = 9,
                    Value = values.Period_9
                },
                new Period
                {
                    PeriodId = 10,
                    Value = values.Period_10
                },
                new Period
                {
                    PeriodId = 11,
                    Value = values.Period_11
                },
                new Period
                {
                    PeriodId = 12,
                    Value = values.Period_12
                }
            };
        }
    }
}
