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
                    previousCollectionMonth = 0;
                    break;
            }

            using (var contextFactory = _fundingDataContext())
            {
                // This is to find out the latest ILR submission of the provider from current ILR collection year and previous ILR collection year
                var esfdataToFilter = contextFactory.ESFFundingDatasSummarised
               .Where(x => x.UKPRN == ukprn
                        && (
                                (x.CollectionYear == summarisationMessage.CollectionYear && x.CollectionPeriod <= summarisationMessage.CollectionMonth)
                                || (x.CollectionYear == previousCollectionYear && x.CollectionPeriod <= previousCollectionMonth)
                                || (x.CollectionYear < previousCollectionYear)
                            )
                        )
               .GroupBy(g => new { g.UKPRN, g.ConRefNumber, g.CollectionYear })
               .Select(s => new { s.Key.UKPRN, s.Key.ConRefNumber, s.Key.CollectionYear, CollectionPeriod = s.Max(y => y.CollectionPeriod) })
               .OrderByDescending(o => o.CollectionPeriod).ToList();

                var esfdata = await contextFactory.ESFFundingDatasSummarised
                            .Where(x => x.UKPRN == ukprn
                                && (x.Period_1 != 0 || x.Period_2 != 0 || x.Period_3 != 0 || x.Period_4 != 0
                                                    || x.Period_5 != 0 || x.Period_6 != 0 || x.Period_7 != 0 || x.Period_8 != 0
                                                    || x.Period_9 != 0 || x.Period_10 != 0 || x.Period_11 != 0 || x.Period_12 != 0)
                                                    )
                            .ToListAsync(cancellationToken);

                return esfdata
                      .Join(esfdataToFilter,
                                esf => new { esf.UKPRN, esf.ConRefNumber, esf.CollectionYear, esf.CollectionPeriod },
                                filter => new { filter.UKPRN, filter.ConRefNumber, filter.CollectionYear, filter.CollectionPeriod },
                                (esf, filter) => esf)
                        .Select(ld =>
                         {
                            var CalendarYear2018 = 2000 + (ld.CollectionYear / 100);
                            var CalendarYear2019 = 2000 + (ld.CollectionYear % 100);

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
                                            BuildPeriodFromLearningDelivery(1, ld.CollectionYear, CalendarYear2018, 8, ld.AttributeName, ld.Period_1),

                                            BuildPeriodFromLearningDelivery(2, ld.CollectionYear, CalendarYear2018, 9, ld.AttributeName, ld.Period_2),

                                            BuildPeriodFromLearningDelivery(3, ld.CollectionYear, CalendarYear2018, 10, ld.AttributeName, ld.Period_3),

                                            BuildPeriodFromLearningDelivery(4, ld.CollectionYear, CalendarYear2018, 11, ld.AttributeName, ld.Period_4),

                                            BuildPeriodFromLearningDelivery(5, ld.CollectionYear, CalendarYear2018, 12, ld.AttributeName, ld.Period_5),

                                            BuildPeriodFromLearningDelivery(6, ld.CollectionYear, CalendarYear2019, 1, ld.AttributeName, ld.Period_6),

                                            BuildPeriodFromLearningDelivery(7, ld.CollectionYear, CalendarYear2019, 2, ld.AttributeName, ld.Period_7),

                                            BuildPeriodFromLearningDelivery(8, ld.CollectionYear, CalendarYear2019, 3, ld.AttributeName, ld.Period_8),

                                            BuildPeriodFromLearningDelivery(9, ld.CollectionYear, CalendarYear2019, 4, ld.AttributeName, ld.Period_9),

                                            BuildPeriodFromLearningDelivery(10, ld.CollectionYear, CalendarYear2019, 5, ld.AttributeName, ld.Period_10),

                                            BuildPeriodFromLearningDelivery(11, ld.CollectionYear, CalendarYear2019, 6, ld.AttributeName, ld.Period_11),

                                            BuildPeriodFromLearningDelivery(12, ld.CollectionYear, CalendarYear2019, 7, ld.AttributeName, ld.Period_12),
                                        }
                                    }
                                }
                            };
                         }).ToList();
            }
        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var contextFactory = _fundingDataContext())
            {
                return await contextFactory.ESFFundingDatasSummarised.Select(l => l.UKPRN).Distinct().ToListAsync(cancellationToken);
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
