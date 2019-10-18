﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using ESFA.DC.DASPayments.EF.Interfaces;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Configuration;

namespace ESFA.DC.Summarisation.Apps1920.Data
{
    public class EasProvider : ILearningDeliveryProvider
    {
        private readonly Func<IDASPaymentsContext> _dasContext;
        private readonly ISummarisationConfigProvider<CollectionPeriod> _collectionPeriodProvider;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.Apps1920_EAS);

        public string CollectionType => ConstantKeys.CollectionType_APPS;

        public EasProvider(Func<IDASPaymentsContext> easContext, ISummarisationConfigProvider<CollectionPeriod> collectionPeriodProvider)
        {
            _dasContext = easContext;
            _collectionPeriodProvider = collectionPeriodProvider;
        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var easContext = _dasContext())
            {
                return await easContext.ProviderAdjustmentPayments.Select(l => Convert.ToInt32(l.Ukprn)).Distinct().ToListAsync(cancellationToken);
            }
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            int previousCollectionYear = summarisationMessage.CollectionYear - 101;

            var collectionPeriods = _collectionPeriodProvider.Provide().ToList();

            List<int> CollectionYears = new List<int>()
            {
                summarisationMessage.CollectionYear,
                previousCollectionYear
            };            

            string previouscollectionPeriodName = string.Empty;
            string currentcollectionPeriodName = $"{summarisationMessage.CollectionYear}-{summarisationMessage.CollectionMonth:D2}";

            switch (summarisationMessage.CollectionMonth)
            {
                case 2:
                    previouscollectionPeriodName = $"{previousCollectionYear}-R13";
                    break;
                case 3:
                    previouscollectionPeriodName = $"{previousCollectionYear}-R14";
                    break;
                default:
                    previouscollectionPeriodName = string.Empty;
                    break;
            }

            List<string> CollectionPeriodNames = new List<string>()
            {
                currentcollectionPeriodName,
                previouscollectionPeriodName
            };

            using (var easContext = _dasContext())
            {
                var nonlevyEAS = await easContext.ProviderAdjustmentPayments
                            .Where(sv => sv.Ukprn == ukprn
                              //&& CollectionYears.Contains(sv.SubmissionAcademicYear)
                                && !ConstantKeys.Levy1799_EASlines.Contains(sv.PaymentTypeName)
                            ).
                            Select(q1 => new
                                {
                                       q1.PaymentTypeName,
                                       CollectionMonth = q1.SubmissionCollectionPeriod,
                                       CollectionYear = q1.SubmissionAcademicYear,
                                       Value = q1.Amount
                                }
                            ).ToListAsync(cancellationToken);

                var levyEAS = (await easContext.ProviderAdjustmentPayments
                            .Where(sv => sv.Ukprn == ukprn
                                && CollectionPeriodNames.Contains(sv.CollectionPeriodName)
                                && ConstantKeys.Levy1799_EASlines.Contains(sv.PaymentTypeName)
                            ).ToListAsync(cancellationToken))
                            .Select(p =>
                            {
                                var collectionPeriod = GetCollectionPeriodFor(collectionPeriods, p.CollectionPeriodYear, p.CollectionPeriodMonth);

                                return new
                                {
                                    p.PaymentTypeName,
                                    CollectionMonth = collectionPeriod?.CollectionMonth ?? 0,
                                    CollectionYear = collectionPeriod?.CollectionYear ?? 0,
                                    Value = p.Amount
                                };

                            }).ToList();


                var eas = nonlevyEAS.Concat(levyEAS).ToList();


                return eas
                        .GroupBy(x => x.PaymentTypeName)
                        .Select(ld => new LearningDelivery
                        {
                            AimSeqNumber = 0,
                            Fundline = ld.Key,
                            PeriodisedData = ld.Select(pd => new PeriodisedData
                            {
                                AttributeName = string.Empty,
                                Periods = new List<Period>
                                {
                                    new Period
                                    {
                                        CollectionMonth = pd.CollectionMonth,
                                        CollectionYear = pd.CollectionYear,
                                        Value = pd.Value
                                    }
                                }
                            }).ToList()
                        }).ToList();
            }
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private CollectionPeriod GetCollectionPeriodFor(IEnumerable<CollectionPeriod> collectionPeriods, int calendarYear, int calendarMonth)
        {
            return collectionPeriods.FirstOrDefault(cp => cp.CalendarYear == calendarYear && cp.CalendarMonth == calendarMonth);
        }
    }
}