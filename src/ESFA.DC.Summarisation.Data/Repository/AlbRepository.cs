using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Data.Repository
{
    public class AlbRepository : IProviderRepository
    {
        private readonly IIlr1819RulebaseContext _ilr;

        public string FundModel => nameof(Configuration.FundModel.ALB);

        public AlbRepository(IIlr1819RulebaseContext ilr)
        {
            _ilr = ilr;
        }

        public async Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(CancellationToken cancellationToken)
        {
            return await _ilr.AlbLearners
                .GroupBy(l => l.Ukprn)
                .Select(l => new Provider
                {
                    UKPRN = l.Key,
                    LearningDeliveries = l.SelectMany(ld => ld.AlbLearningDeliveries
                        .Select(
                            ldd => new LearningDelivery
                            {
                                LearnRefNumber = ldd.LearnRefNumber,
                                AimSeqNumber = ldd.AimSeqNumber,
                                Fundline = ldd.FundLine,
                                PeriodisedData = ldd.AlbLearningDeliveryPeriodisedValues
                                    .GroupBy(pv => pv.AttributeName)
                                    .Select(group => new PeriodisedData
                                    {
                                        AttributeName = group.Key,
                                        Periods = group.SelectMany(UnflattenToPeriod).ToList()
                                    } as IPeriodisedData).ToList()
                            } as ILearningDelivery
                        )).ToList()
                }).ToListAsync(cancellationToken);
        }

        public async Task<int> RetrieveProviderPageCountAsync(int pageSize, CancellationToken cancellationToken)
        {
            return await _ilr.AlbLearners
                .GroupBy(l => l.Ukprn)
                .CountAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _ilr.AlbLearners
                .GroupBy(l => l.Ukprn)
                .OrderBy(o => o.Key)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new Provider
                {
                    UKPRN = l.Key,
                    LearningDeliveries = l.SelectMany(ld => ld.AlbLearningDeliveries
                        .Select(
                            ldd => new LearningDelivery
                            {
                                LearnRefNumber = ldd.LearnRefNumber,
                                AimSeqNumber = ldd.AimSeqNumber,
                                Fundline = ldd.FundLine,
                                PeriodisedData = ldd.AlbLearningDeliveryPeriodisedValues
                                    .GroupBy(pv => pv.AttributeName)
                                    .Select(group => new PeriodisedData
                                    {
                                        AttributeName = group.Key,
                                        Periods = group.SelectMany(UnflattenToPeriod).ToList()
                                    } as IPeriodisedData).ToList()
                            } as ILearningDelivery
                        )).ToList()
                }).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(int Ukprn, CancellationToken cancellationToken)
        {
            return await _ilr.AlbLearners
                .Where(p => p.Ukprn == Ukprn)
                .Select(l => new Provider
                {
                    UKPRN = l.Ukprn,
                    LearningDeliveries = l.AlbLearningDeliveries
                        .Select(
                            ldd => new LearningDelivery
                            {
                                LearnRefNumber = ldd.LearnRefNumber,
                                AimSeqNumber = ldd.AimSeqNumber,
                                Fundline = ldd.FundLine,
                                PeriodisedData = ldd.AlbLearningDeliveryPeriodisedValues
                                    .GroupBy(pv => pv.AttributeName)
                                    .Select(group => new PeriodisedData
                                    {
                                        AttributeName = group.Key,
                                        Periods = group.SelectMany(UnflattenToPeriod).ToList()
                                    } as IPeriodisedData).ToList()
                            } as ILearningDelivery
                        ).ToList()
                }).ToListAsync(cancellationToken);
        }

        private IEnumerable<IPeriod> UnflattenToPeriod(AlbLearningDeliveryPeriodisedValue values)
        {
            return new List<Period>
            {
                new Period
                {
                    PeriodId = 1,
                    Value = values.Period1
                },
                new Period
                {
                    PeriodId = 2,
                    Value = values.Period2
                },
                new Period
                {
                    PeriodId = 3,
                    Value = values.Period3
                },
                new Period
                {
                    PeriodId = 4,
                    Value = values.Period4
                },
                new Period
                {
                    PeriodId = 5,
                    Value = values.Period5
                },
                new Period
                {
                    PeriodId = 6,
                    Value = values.Period6
                },
                new Period
                {
                    PeriodId = 7,
                    Value = values.Period7
                },
                new Period
                {
                    PeriodId = 8,
                    Value = values.Period8
                },
                new Period
                {
                    PeriodId = 9,
                    Value = values.Period9
                },
                new Period
                {
                    PeriodId = 10,
                    Value = values.Period10
                },
                new Period
                {
                    PeriodId = 11,
                    Value = values.Period11
                },
                new Period
                {
                    PeriodId = 12,
                    Value = values.Period12
                }
            };
        }
    }
}
