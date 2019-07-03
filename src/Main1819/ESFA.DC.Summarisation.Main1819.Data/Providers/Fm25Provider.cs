using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESF.DC.Summarisation.Main1819.Data.Providers
{
    public class Fm25Provider : ILearningDeliveryProvider
    {
        public string SummarisationType => nameof(ESFA.DC.Summarisation.Configuration.Enum.SummarisationType.Main1819_FM25);

        public string CollectionType => nameof(ESFA.DC.Summarisation.Configuration.Enum.CollectionType.ILR1819);

        private readonly Func<IIlr1819RulebaseContext> _ilr1819RulebaseContext;

        public Fm25Provider(Func<IIlr1819RulebaseContext> ilr1819RulebaseContext)
        {
            _ilr1819RulebaseContext = ilr1819RulebaseContext;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            using (var ilrContext = _ilr1819RulebaseContext())
            {
                return await ilrContext.FM25_Learners
                    .Where(ld => ld.UKPRN == ukprn)
                    .Select(ld => new LearningDelivery
                    {
                        LearnRefNumber = ld.LearnRefNumber,
                        Fundline = ld.FundLine,
                        PeriodisedData = ld.FM25_FM35_Learner_PeriodisedValues
                            .Where(x => (
                                x.Period_1 +
                                x.Period_2 +
                                x.Period_3 +
                                x.Period_4 +
                                x.Period_5 +
                                x.Period_6 +
                                x.Period_7 +
                                x.Period_8 +
                                x.Period_9 +
                                x.Period_10 +
                                x.Period_11 +
                                x.Period_12) > 0)
                            .Select(pv => new PeriodisedData
                            {
                                AttributeName = pv.AttributeName,
                                Periods = new List<Period>
                                {
                                new Period
                                {
                                    PeriodId = 1,
                                    Value = pv.Period_1
                                },
                                new Period
                                {
                                    PeriodId = 2,
                                    Value = pv.Period_2
                                },
                                new Period
                                {
                                    PeriodId = 3,
                                    Value = pv.Period_3
                                },
                                new Period
                                {
                                    PeriodId = 4,
                                    Value = pv.Period_4
                                },
                                new Period
                                {
                                    PeriodId = 5,
                                    Value = pv.Period_5
                                },
                                new Period
                                {
                                    PeriodId = 6,
                                    Value = pv.Period_6
                                },
                                new Period
                                {
                                    PeriodId = 7,
                                    Value = pv.Period_7
                                },
                                new Period
                                {
                                    PeriodId = 8,
                                    Value = pv.Period_8
                                },
                                new Period
                                {
                                    PeriodId = 9,
                                    Value = pv.Period_9
                                },
                                new Period
                                {
                                    PeriodId = 10,
                                    Value = pv.Period_10
                                },
                                new Period
                                {
                                    PeriodId = 11,
                                    Value = pv.Period_11
                                },
                                new Period
                                {
                                    PeriodId = 12,
                                    Value = pv.Period_12
                                }
                                }
                            }).ToList()
                    }).ToListAsync(cancellationToken);
            }
        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var ilrContext = _ilr1819RulebaseContext())
            {
                return await ilrContext.FM25_Learners
                    .Select(i => i.UKPRN).Distinct()
                    .ToListAsync();
            }
        }

        public Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken) => ProvideAsync(ukprn, cancellationToken);
    }
}
