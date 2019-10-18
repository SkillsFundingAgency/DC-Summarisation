using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LearningDelivery = ESFA.DC.Summarisation.Data.Input.Model.LearningDelivery;
using ESFA.DC.ESF.FundingData.Database.EF.Interfaces;
using ESFA.DC.Summarisation.Constants;

namespace ESFA.DC.Summarisation.ESF.Data.Providers
{
    public class ESFILRProvider : ILearningDeliveryProvider
    {
        private readonly Func<IESFFundingDataContext> _fundingDataContext;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.ESF_ILRData);

        public string CollectionType => ConstantKeys.CollectionType_ESF;

        public ESFILRProvider(Func<IESFFundingDataContext> fundingDataContext)
        {
            _fundingDataContext = fundingDataContext;
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

            using (var contextFactory = _fundingDataContext())
            {
                var esfdataToFilter = contextFactory.LatestProviderSubmissions
               .Where(x => x.UKPRN == ukprn)
               .Select(s1 => new
               {
                   s1.UKPRN,
                   CollectionYear = Convert.ToInt32(s1.CollectionType.Substring(3)),
                   CollectionMonth = (s1.CollectionReturnCode == string.Empty ? 0 : Convert.ToInt32(s1.CollectionReturnCode.Substring(1)))
               })
               .Select(s2 => new
                    {
                        s2.UKPRN,
                        s2.CollectionYear,
                        CollectionPeriod = (s2.CollectionYear == previousCollectionYear && s2.CollectionMonth > previousCollectionMonth ? previousCollectionMonth : s2.CollectionMonth)
                    }).ToList();

                return await contextFactory.ESFFundingDatasSummarised
                    .Join(esfdataToFilter,
                                esf => new {esf.UKPRN, esf.CollectionYear,esf.CollectionPeriod },
                                filter => new { filter.UKPRN, filter.CollectionYear, filter.CollectionPeriod },
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
                                        CalendarYear = 2000 + (ld.CollectionYear / 100),
                                        CalendarMonth = 8,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_1 : 0,
                                        Volume = ld.AttributeName.Equals("DeliverableVolume") ? Convert.ToInt32(ld.Period_1) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 2,
                                        CollectionYear = ld.CollectionYear,
                                        CalendarYear = 2000 + (ld.CollectionYear / 100),
                                        CalendarMonth = 9,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_2 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt32(ld.Period_2) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 3,
                                        CollectionYear = ld.CollectionYear,
                                        CalendarYear = 2000 + (ld.CollectionYear / 100),
                                        CalendarMonth = 10,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_3 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt32(ld.Period_3) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 4,
                                        CollectionYear = ld.CollectionYear,
                                        CalendarYear = 2000 + (ld.CollectionYear / 100),
                                        CalendarMonth = 11,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_4 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt32(ld.Period_4) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 5,
                                        CollectionYear = ld.CollectionYear,
                                        CalendarYear = 2000 + (ld.CollectionYear / 100),
                                        CalendarMonth = 12,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_5 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt32(ld.Period_5) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 6,
                                        CollectionYear = ld.CollectionYear,
                                        CalendarYear = 2000 + (ld.CollectionYear % 100),
                                        CalendarMonth = 1,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_6 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt32(ld.Period_6) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 7,
                                        CollectionYear = ld.CollectionYear,
                                        CalendarYear = 2000 + (ld.CollectionYear % 100),
                                        CalendarMonth = 2,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_7 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt32(ld.Period_7) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 8,
                                        CollectionYear = ld.CollectionYear,
                                        CalendarYear = 2000 + (ld.CollectionYear % 100),
                                        CalendarMonth = 3,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_8 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt32(ld.Period_8) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 9,
                                        CollectionYear = ld.CollectionYear,
                                        CalendarYear = 2000 + (ld.CollectionYear % 100),
                                        CalendarMonth = 4,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_9 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt32(ld.Period_9) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 10,
                                        CollectionYear = ld.CollectionYear,
                                        CalendarYear = 2000 + (ld.CollectionYear % 100),
                                        CalendarMonth = 5,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_10 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt32(ld.Period_10) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 11,
                                        CollectionYear = ld.CollectionYear,
                                        CalendarYear = 2000 + (ld.CollectionYear % 100),
                                        CalendarMonth = 6,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_11 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt32(ld.Period_11) : 0
                                    },
                                    new Period
                                    {
                                        PeriodId = 12,
                                        CollectionYear = ld.CollectionYear,
                                        CalendarYear = 2000 + (ld.CollectionYear % 100),
                                        CalendarMonth = 7,
                                        Value = ld.AttributeName != "DeliverableVolume" ? ld.Period_12 : 0,
                                        Volume = ld.AttributeName == "DeliverableVolume" ? Convert.ToInt32(ld.Period_12) : 0
                                    }
                                }
                             }
                            }
                    }).ToListAsync(cancellationToken);
            }
            
        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var contextFactory = _fundingDataContext())
            {
                return await contextFactory.LatestProviderSubmissions.Select(l => l.UKPRN).Distinct().ToListAsync(cancellationToken);
            }
        }

        public Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
