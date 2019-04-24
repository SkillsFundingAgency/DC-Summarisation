using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Main1819.Data.Repository
{
    public class Fm35Provider : ILearningDeliveryProvider
    {
        private readonly IIlr1819RulebaseContext _ilr;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.Main1819_ALB);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.ILR1819);

        public Fm35Provider(IIlr1819RulebaseContext ilr)
        {
            _ilr = ilr;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            return await _ilr.FM35_LearningDeliveries
                .Where(ld => ld.UKPRN == ukprn)
                .Select(ld => new LearningDelivery
                {
                    LearnRefNumber = ld.LearnRefNumber,
                    AimSeqNumber = ld.AimSeqNumber,
                    Fundline = ld.FundLine,
                    PeriodisedData = ld.FM35_LearningDelivery_PeriodisedValues
                        .Select(pv => new PeriodisedData
                        {
                            AttributeName = pv.AttributeName,
                            Periods = ConvertToPeriodsList(pv)
                        }).ToList()
                }).ToListAsync(cancellationToken);
        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            return await _ilr.FM35_Learners.Select(l => l.UKPRN).Distinct().ToListAsync(cancellationToken);
        }

        private IList<Period> ConvertToPeriodsList(FM35_LearningDelivery_PeriodisedValue values)
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
