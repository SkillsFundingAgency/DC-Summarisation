using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LearningDelivery = ESFA.DC.Summarisation.Data.Input.Model.LearningDelivery;

namespace ESFA.DC.Summarisation.ESF.Data.Providers
{
    public class ESFILRProvider : ILearningDeliveryProvider
    {
        private readonly Func<Model.Interface.ISummarisationContext> _summarisationContext;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.ESF_ILRData);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.ESF);

        public ESFILRProvider(Func<Model.Interface.ISummarisationContext> summarisationContext)
        {
            _summarisationContext = summarisationContext;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            int previousCollectionYear = summarisationMessage.CollectionYear - 101;
            int previousCollectionMonth = 0;

            switch(summarisationMessage.CollectionMonth)
            {
                case 1:
                    previousCollectionMonth = 12;
                    break;
                case 2:
                    previousCollectionMonth = 13;
                    break;
                case 3:
                    previousCollectionMonth = 14;
                    break;
                default:
                    previousCollectionMonth = 0;
                    break;
            }

            using (var contextFactory = _summarisationContext())
            {
                // This is to find out the latest ILR submission of the provider from current ILR collection year and previous ILR collection year
                var esfdataToFilter = contextFactory.ESF_FundingDatas
               .Where(x => x.UKPRN == ukprn 
                        && (
                                (x.CollectionYear == summarisationMessage.CollectionYear && x.CollectionPeriod <= summarisationMessage.CollectionMonth) 
                                || (x.CollectionYear == previousCollectionYear && x.CollectionPeriod <= previousCollectionMonth)
                                || (x.CollectionYear < previousCollectionYear )
                            )
                        )
               .GroupBy(g => new { g.UKPRN, g.ConRefNumber, g.CollectionYear })
               .Select(s => new { s.Key.UKPRN, s.Key.ConRefNumber, s.Key.CollectionYear, CollectionPeriod = s.Max(y => y.CollectionPeriod) })
               .OrderByDescending(o => o.CollectionPeriod).ToList();


                return await contextFactory.ESF_FundingDatas
                    .Join(esfdataToFilter,
                                esf => new {esf.UKPRN,esf.ConRefNumber,esf.CollectionYear,esf.CollectionPeriod },
                                filter => new { filter.UKPRN, filter.ConRefNumber, filter.CollectionYear, filter.CollectionPeriod },
                                (esf, filter) => esf)
                    .Where(x => x.UKPRN == ukprn
                        && (x.Period_1 + x.Period_2 + x.Period_3 + x.Period_4 + x.Period_5 + x.Period_6 + x.Period_7 + x.Period_8 + x.Period_9 + x.Period_10 + x.Period_11 + x.Period_12) > 0)
                    .Select(ld => new LearningDelivery
                    {
                        ConRefNumber = ld.ConRefNumber,
                        PeriodisedData = new List<PeriodisedData>
                            {
                             new PeriodisedData
                              {
                                AttributeName = ld.AttributeName,
                                DeliverableCode = ld.DeliverableCode,
                                Periods = new List<Period>
                                {
                                    new Period
                                    {
                                        PeriodId = 1,
                                        CollectionYear = ld.CollectionYear,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_1 : 0,
                                        Volume = ld.AttributeName.Equals("DeliverableVolume") ? Convert.ToInt16(ld.Period_1) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 2,
                                        CollectionYear = ld.CollectionYear,
                                       Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_2 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt16(ld.Period_2) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 3,
                                        CollectionYear = ld.CollectionYear,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_3 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt16(ld.Period_3) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 4,
                                        CollectionYear = ld.CollectionYear,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_4 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt16(ld.Period_4) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 5,
                                        CollectionYear = ld.CollectionYear,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_5 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt16(ld.Period_5) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 6,
                                        CollectionYear = ld.CollectionYear,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_6 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt16(ld.Period_6) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 7,
                                        CollectionYear = ld.CollectionYear,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_7 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt16(ld.Period_7) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 8,
                                        CollectionYear = ld.CollectionYear,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_8 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt16(ld.Period_8) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 9,
                                        CollectionYear = ld.CollectionYear,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_9 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt16(ld.Period_9) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 10,
                                        CollectionYear = ld.CollectionYear,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_10 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt16(ld.Period_10) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 11,
                                        CollectionYear = ld.CollectionYear,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_11 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt16(ld.Period_11) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 12,
                                        CollectionYear = ld.CollectionYear,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_12 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt16(ld.Period_12) : 0
                                    }
                                }
                             }
                            }
                    }).ToListAsync(cancellationToken);
            }
            
        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var contextFactory = _summarisationContext())
            {
                return await contextFactory.ESF_FundingDatas.Select(l => l.UKPRN).Distinct().ToListAsync(cancellationToken);
            }
        }

        public Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
