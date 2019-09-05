﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Main1920.Data.Providers
{
    public class AlbProvider : ILearningDeliveryProvider
    {
        private readonly Func<IIlr1920RulebaseContext> _ilr;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.Main1920_ALB);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.ILR1920);

        public AlbProvider(Func<IIlr1920RulebaseContext> ilr)
        {
            _ilr = ilr;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            using (var ilrContext = _ilr())
            {
                return await ilrContext.ALB_LearningDeliveries
                    .Where(ld => ld.UKPRN == ukprn)
                    .Select(ld => new LearningDelivery
                    {
                        LearnRefNumber = ld.LearnRefNumber,
                        AimSeqNumber = ld.AimSeqNumber,
                        Fundline = ld.FundLine,
                        PeriodisedData = ld.ALB_LearningDelivery_PeriodisedValues
                            .Where(x => (x.Period_1 + x.Period_2 + x.Period_3 + x.Period_4 + x.Period_5 + x.Period_6 + x.Period_7 + x.Period_8 + x.Period_9 + x.Period_10 + x.Period_11 + x.Period_12) > 0)
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
                return await ilrContext.ALB_Learners.Select(l => l.UKPRN).Distinct().ToListAsync(cancellationToken);
            }
        }

        public Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
            => ProvideAsync(ukprn, cancellationToken);
    }
}