using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main.Interfaces;
using ESFA.DC.Summarisation.Main.Model;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Main2021.Service.Providers
{
    public class TblProvider : ISummarisationInputDataProvider
    {
        private readonly Func<IIlr2021Context> _ilrContext;

        public TblProvider(Func<IIlr2021Context> ilrContext)
        {
            _ilrContext = ilrContext;
        }

        public string CollectionType => CollectionTypeConstants.ILR2021;

        public async Task<ICollection<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
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
                            .Where(x => (x.Period_1 != 0 || x.Period_2 != 0 || x.Period_3 != 0 || x.Period_4 != 0
                                         || x.Period_5 != 0 || x.Period_6 != 0 || x.Period_7 != 0 || x.Period_8 != 0
                                         || x.Period_9 != 0 || x.Period_10 != 0 || x.Period_11 != 0 || x.Period_12 != 0))
                            .Select(pv => new PeriodisedData
                            {
                                AttributeName = pv.AttributeName,
                                Periods = new List<Period>
                                {
                                    new Period
                                    {
                                        PeriodId = 1,
                                        Value = pv.Period_1,
                                    },
                                    new Period
                                    {
                                        PeriodId = 2,
                                        Value = pv.Period_2,
                                    },
                                    new Period
                                    {
                                        PeriodId = 3,
                                        Value = pv.Period_3,
                                    },
                                    new Period
                                    {
                                        PeriodId = 4,
                                        Value = pv.Period_4,
                                    },
                                    new Period
                                    {
                                        PeriodId = 5,
                                        Value = pv.Period_5,
                                    },
                                    new Period
                                    {
                                        PeriodId = 6,
                                        Value = pv.Period_6,
                                    },
                                    new Period
                                    {
                                        PeriodId = 7,
                                        Value = pv.Period_7,
                                    },
                                    new Period
                                    {
                                        PeriodId = 8,
                                        Value = pv.Period_8,
                                    },
                                    new Period
                                    {
                                        PeriodId = 9,
                                        Value = pv.Period_9,
                                    },
                                    new Period
                                    {
                                        PeriodId = 10,
                                        Value = pv.Period_10,
                                    },
                                    new Period
                                    {
                                        PeriodId = 11,
                                        Value = pv.Period_11,
                                    },
                                    new Period
                                    {
                                        PeriodId = 12,
                                        Value = pv.Period_12,
                                    },
                                },
                            }).ToList(),
                    }).ToListAsync(cancellationToken);

                return learningDeliveries;
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