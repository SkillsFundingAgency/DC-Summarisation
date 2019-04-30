using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.ILR1819.DataStore.EF;

namespace ESF.DC.Summarisation.Main1819.Data.Repository
{
    public class TblRepository : IProviderRepository
    {
        private readonly IIlr1819RulebaseContext _ilrContext;

        public TblRepository(IIlr1819RulebaseContext ilrContext)
        {
            _ilrContext = ilrContext;
        }

        public string SummarisationType => nameof(ESFA.DC.Summarisation.Configuration.Enum.SummarisationType.Main1819_TBL);

        public string CollectionType => nameof(ESFA.DC.Summarisation.Configuration.Enum.CollectionType.ILR1819);

        public async Task<int> RetrieveProviderPageCountAsync(int pageSize, CancellationToken cancellationToken)
        {
            var providerCount = await _ilrContext.TBL_Learners.GroupBy(t => t.UKPRN).CountAsync();
            return providerCount % pageSize == 0 
                ? (providerCount / pageSize) 
                : (providerCount / pageSize) + 1;
        }

        public async Task<IReadOnlyCollection<IProvider>> RetrieveProvidersAsync(int pageSize, int pageNumber, CancellationToken cancellationToken)
        {
            return await _ilrContext.TBL_Learners
                 .GroupBy(l => l.UKPRN)
                 .OrderBy(o => o.Key)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .Select(l => new Provider
                 {
                     UKPRN = l.Key,
                     LearningDeliveries = l.SelectMany(ld => ld.TBL_LearningDeliveries
                         .Select(
                             ldd => new LearningDelivery
                             {
                                 LearnRefNumber = ldd.LearnRefNumber,
                                 AimSeqNumber = ldd.AimSeqNumber,
                                 Fundline = ldd.FundLine,
                                 PeriodisedData = ldd.TBL_LearningDelivery_PeriodisedValues
                                     .GroupBy(pv => pv.AttributeName)
                                     .Select(group => new PeriodisedData
                                     {
                                         AttributeName = group.Key,
                                         Periods = group.SelectMany(pd => UnflattenToPeriod(pd)).ToList()
                                     }).ToList()
                             })).ToList()
                 }).ToListAsync(cancellationToken);
        }

        private IEnumerable<Period> UnflattenToPeriod(TBL_LearningDelivery_PeriodisedValue values)
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
