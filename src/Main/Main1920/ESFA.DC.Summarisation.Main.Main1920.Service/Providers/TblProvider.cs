using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Main1920.Service.Providers
{
    public class TblProvider : AbstractLearningProviderProvider, ISummarisationInputDataProvider<ILearningProvider>
    {
        private readonly Func<IIlr1920RulebaseContext> _ilrContext;

        public TblProvider(Func<IIlr1920RulebaseContext> ilrContext)
        {
            _ilrContext = ilrContext;
        }

        public string CollectionType => CollectionTypeConstants.ILR1920;

        public async Task<ILearningProvider> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            using (var ilrContext = _ilrContext())
            {
                var learningDeliveries = await ilrContext.TBL_LearningDeliveries
                    .Where(ld => ld.UKPRN == ukprn)
                    .Select(ld => new LearningDelivery
                    {
                        LearnRefNumber = ld.LearnRefNumber,
                        AimSeqNumber = ld.AimSeqNumber,
                        Fundline = ld.FundLine,
                        PeriodisedData = ld.TBL_LearningDelivery_PeriodisedValues
                            .Select(pv => new PeriodisedData
                            {
                                AttributeName = pv.AttributeName,
                                Periods = new List<IPeriod>
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

                return BuildLearningProvider(ukprn, learningDeliveries);
            }
        }

        public async Task<ICollection<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var ilrContext = _ilrContext())
            {
                return await ilrContext.TBL_Learners
                    .Select(l => l.UKPRN).Distinct()
                    .ToListAsync(cancellationToken);
            }
        }
    }
}