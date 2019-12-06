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
using ESFA.DC.ESF.FundingData.Database.EF.Query;

namespace ESFA.DC.Summarisation.ESF.Data.Providers
{
    public class ESFILRProvider : ILearningDeliveryProvider
    {
        private readonly Func<IESFFundingDataContext> _fundingDataContext;

        public string SummarisationType => SummarisationTypeConstants.ESF_ILRData;

        public string CollectionType => CollectionTypeConstants.ESF;

        public ESFILRProvider(Func<IESFFundingDataContext> fundingDataContext)
        {
            _fundingDataContext = fundingDataContext;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            int previousCollectionYear = summarisationMessage.CollectionYear - 101;
            int previousCollectionMonth = 0;

            switch (summarisationMessage.CollectionMonth)
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
                    previousCollectionMonth = 14;
                    break;
            }

            using (var context = _fundingDataContext())
            {
                var distinctFundingData = await context.ESFFundingDatas
                                                     .Where(d => d.UKPRN == ukprn)
                                                     .Select(d => new
                                                     {
                                                         d.ConRefNumber,
                                                         d.CollectionType,
                                                         d.CollectionReturnCode
                                                     }).Distinct()
                                                     .ToListAsync(cancellationToken);

                var maxReturnKeys = distinctFundingData
                                     .Select(d => new {
                                         d.ConRefNumber,
                                         d.CollectionType,
                                         d.CollectionReturnCode,
                                         CollectionYear = Convert.ToInt16(d.CollectionType.Substring(3)),
                                         CollectionPeriod = Convert.ToInt16(d.CollectionReturnCode == string.Empty ? "0" : d.CollectionReturnCode.Substring(1))
                                     })
                                     .Where(x =>
                                                 (x.CollectionYear == summarisationMessage.CollectionYear && x.CollectionPeriod <= summarisationMessage.CollectionMonth)
                                                 || (x.CollectionYear == previousCollectionYear && x.CollectionPeriod <= previousCollectionMonth)
                                                 || (x.CollectionYear < previousCollectionYear)
                                         )
                                    .GroupBy(g => new { g.ConRefNumber, g.CollectionType })
                                    .Select(s => new { s.Key.ConRefNumber, s.Key.CollectionType, CollectionPeriod = s.Max(y => y.CollectionPeriod) })
                                    .ToList();

                var predicate = PredicateBuilder.False<ESFFundingData>();

                foreach(var key in maxReturnKeys)
                {
                    var collectionReturnCodeForKey = (key.CollectionPeriod == 0 ? string.Empty : $"R{key.CollectionPeriod:D2}");

                    predicate = predicate.Or(f => f.ConRefNumber == key.ConRefNumber && f.CollectionType == key.CollectionType && f.CollectionReturnCode == collectionReturnCodeForKey);
                }

                var esfpreSummarised = await context
                    .ESFFundingDatas
                    .Where(w => w.UKPRN == ukprn)
                    .Where(predicate)
                       .GroupBy(g => new { g.UKPRN, g.ConRefNumber, g.DeliverableCode, g.AttributeName, g.CollectionType, g.CollectionReturnCode })
                       .Select(s => new {
                           s.Key.UKPRN,
                           s.Key.ConRefNumber,
                           s.Key.DeliverableCode,
                           s.Key.AttributeName,
                           s.Key.CollectionType,
                           s.Key.CollectionReturnCode,
                           Period_1 = s.Sum(p => p.Period_1),
                           Period_2 = s.Sum(p => p.Period_2),
                           Period_3 = s.Sum(p => p.Period_3),
                           Period_4 = s.Sum(p => p.Period_4),
                           Period_5 = s.Sum(p => p.Period_5),
                           Period_6 = s.Sum(p => p.Period_6),
                           Period_7 = s.Sum(p => p.Period_7),
                           Period_8 = s.Sum(p => p.Period_8),
                           Period_9 = s.Sum(p => p.Period_9),
                           Period_10 = s.Sum(p => p.Period_10),
                           Period_11 = s.Sum(p => p.Period_11),
                           Period_12 = s.Sum(p => p.Period_12),
                       }).ToListAsync(cancellationToken);

                var result = esfpreSummarised
                        .Select(ld =>
                        {
                            int CollectionYear = Convert.ToInt32(ld.CollectionType.Substring(3));

                            var CalendarYearStart = 2000 + (CollectionYear / 100);
                            var CalendarYearEnd = 2000 + (CollectionYear % 100);

                            return new LearningDelivery
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
                                            BuildPeriodFromLearningDelivery(1, CollectionYear, CalendarYearStart, 8, ld.AttributeName, ld.Period_1),

                                            BuildPeriodFromLearningDelivery(2, CollectionYear, CalendarYearStart, 9, ld.AttributeName, ld.Period_2),

                                            BuildPeriodFromLearningDelivery(3, CollectionYear, CalendarYearStart, 10, ld.AttributeName, ld.Period_3),

                                            BuildPeriodFromLearningDelivery(4, CollectionYear, CalendarYearStart, 11, ld.AttributeName, ld.Period_4),

                                            BuildPeriodFromLearningDelivery(5, CollectionYear, CalendarYearStart, 12, ld.AttributeName, ld.Period_5),

                                            BuildPeriodFromLearningDelivery(6, CollectionYear, CalendarYearEnd, 1, ld.AttributeName, ld.Period_6),

                                            BuildPeriodFromLearningDelivery(7, CollectionYear, CalendarYearEnd, 2, ld.AttributeName, ld.Period_7),

                                            BuildPeriodFromLearningDelivery(8, CollectionYear, CalendarYearEnd, 3, ld.AttributeName, ld.Period_8),

                                            BuildPeriodFromLearningDelivery(9, CollectionYear, CalendarYearEnd, 4, ld.AttributeName, ld.Period_9),

                                            BuildPeriodFromLearningDelivery(10, CollectionYear, CalendarYearEnd, 5, ld.AttributeName, ld.Period_10),

                                            BuildPeriodFromLearningDelivery(11, CollectionYear, CalendarYearEnd, 6, ld.AttributeName, ld.Period_11),

                                            BuildPeriodFromLearningDelivery(12, CollectionYear, CalendarYearEnd, 7, ld.AttributeName, ld.Period_12),
                                        }
                                    }
                                }
                            };
                        }).ToList();

                return result;
            }
        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var contextFactory = _fundingDataContext())
            {
                return await contextFactory.ESFFundingDatas.Select(l => l.UKPRN).Distinct().ToListAsync(cancellationToken);
            }
        }

        private Period BuildPeriodFromLearningDelivery(int period, int collectionYear, int calendarYear, int calendarMonth, string attributeName, decimal? periodValue)
        {
            var isDeliverableVolume = string.Equals(attributeName, ESFConstants.DeliverableVolume, StringComparison.OrdinalIgnoreCase);

            return new Period
            {
                PeriodId = period,
                CollectionYear = collectionYear,
                CalendarYear = calendarYear,
                CalendarMonth = calendarMonth,
                Value = !isDeliverableVolume ? periodValue : 0,
                Volume = isDeliverableVolume ? Convert.ToInt32(periodValue) : 0
            };
        }
    }
}
