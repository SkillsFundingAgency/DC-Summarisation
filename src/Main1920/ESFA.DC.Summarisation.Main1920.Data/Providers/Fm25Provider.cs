﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Main1920.Data.Providers
{
    public class Fm25Provider : ILearningDeliveryProvider
    {
        public string SummarisationType => SummarisationTypeConstants.Main1920_FM25;

        public string CollectionType => CollectionTypeConstants.ILR1920;

        private readonly Func<IIlr1920RulebaseContext> _ilr;

        public Fm25Provider(Func<IIlr1920RulebaseContext> ilr)
        {
            _ilr = ilr;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            using (var ilrContext = _ilr())
            {
                return await ilrContext.FM25_Learners
                    .Where(ld => ld.UKPRN == ukprn)
                    .Select(ld => new LearningDelivery
                    {
                        LearnRefNumber = ld.LearnRefNumber,
                        Fundline = ld.FundLine,
                        PeriodisedData = ld.FM25_FM35_Learner_PeriodisedValues
                            .Where(x => (x.Period_1 != 0 || x.Period_2 != 0 || x.Period_3 != 0 || x.Period_4 != 0
                                            || x.Period_5 != 0 || x.Period_6 != 0 || x.Period_7 != 0 || x.Period_8 != 0
                                            || x.Period_9 != 0 || x.Period_10 != 0 || x.Period_11 != 0 || x.Period_12 != 0
                                        )
                                )
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
            using (var ilrContext = _ilr())
            {
                return await ilrContext.FM25_Learners
                    .Select(i => i.UKPRN).Distinct()
                    .ToListAsync();
            }
        }
    }
}
