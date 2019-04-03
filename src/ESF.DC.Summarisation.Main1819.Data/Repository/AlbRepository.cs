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
    public class AlbRepository : IProviderRepository
    {
        private readonly IIlr1819RulebaseContext _ilr;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.Main1819_ALB);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.ILR1819);

        public AlbRepository(IIlr1819RulebaseContext ilr)
        {
            _ilr = ilr;
        }

        public async Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(CancellationToken cancellationToken)
        {
            return await _ilr.ALB_Learners
                .GroupBy(l => l.UKPRN)
                .Select(l => new Provider
                {
                    UKPRN = l.Key,
                    LearningDeliveries = l.SelectMany(ld => ld.ALB_LearningDeliveries
                        .Select(
                            ldd => new LearningDelivery
                            {
                                LearnRefNumber = ldd.LearnRefNumber,
                                AimSeqNumber = ldd.AimSeqNumber,
                                Fundline = ldd.FundLine,
                                PeriodisedData = ldd.ALB_LearningDelivery_PeriodisedValues
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
            return await _ilr.ALB_Learners
                .GroupBy(l => l.UKPRN)
                .CountAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _ilr.ALB_Learners
                 .GroupBy(l => l.UKPRN)
                 .OrderBy(o => o.Key)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .Select(l => new Provider
                 {
                     UKPRN = l.Key,
                     LearningDeliveries = l.SelectMany(ld => ld.ALB_LearningDeliveries
                         .Select(
                             ldd => new LearningDelivery
                             {
                                 LearnRefNumber = ldd.LearnRefNumber,
                                 AimSeqNumber = ldd.AimSeqNumber,
                                 Fundline = ldd.FundLine,
                                 PeriodisedData = ldd.ALB_LearningDelivery_PeriodisedValues
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
            return await _ilr.ALB_Learners
                .Where(p => p.UKPRN == Ukprn)
                .Select(l => new Provider
                {
                    UKPRN = l.UKPRN,
                    LearningDeliveries = l.ALB_LearningDeliveries
                        .Select(
                            ldd => new LearningDelivery
                            {
                                LearnRefNumber = ldd.LearnRefNumber,
                                AimSeqNumber = ldd.AimSeqNumber,
                                Fundline = ldd.FundLine,
                                PeriodisedData = ldd.ALB_LearningDelivery_PeriodisedValues
                                    .GroupBy(pv => pv.AttributeName)
                                    .Select(group => new PeriodisedData
                                    {
                                        AttributeName = group.Key,
                                        Periods = group.SelectMany(UnflattenToPeriod).ToList()
                                    } as IPeriodisedData).ToList()
                            } as ILearningDelivery).ToList()
                }).ToListAsync(cancellationToken);
        }

        private IEnumerable<IPeriod> UnflattenToPeriod(ALB_LearningDelivery_PeriodisedValue values)
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
