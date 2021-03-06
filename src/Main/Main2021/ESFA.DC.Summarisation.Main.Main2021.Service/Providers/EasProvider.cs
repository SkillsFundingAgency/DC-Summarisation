﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EAS2021.EF.Interface;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main.Interfaces;
using ESFA.DC.Summarisation.Main.Model;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Main2021.Service.Providers
{
    public class EasProvider : ISummarisationInputDataProvider
    {
        private readonly Func<IEasdbContext> _easContext;

        public string CollectionType => CollectionTypeConstants.ILR2021;

        public EasProvider(Func<IEasdbContext> easContext)
        {
            _easContext = easContext;
        }

        public async Task<ICollection<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            using (var easContext = _easContext())
            {
                var learningDeliveries = await easContext.EasSubmissionValues
                        .Where(sv => sv.EasSubmission.Ukprn == ukprn.ToString())
                        .Join(easContext.PaymentTypes, sv => sv.PaymentId, p => p.PaymentId, (value, type) => new { value.CollectionPeriod, value.PaymentValue, type.PaymentName })
                        .GroupBy(x => x.PaymentName)
                        .Select(ld => new LearningDelivery
                        {
                            LearnRefNumber = string.Empty,
                            AimSeqNumber = 0,
                            Fundline = ld.Key,
                            PeriodisedData = ld.GroupBy(x => x.PaymentName).Select(pd => new PeriodisedData
                            {
                                AttributeName = string.Empty,
                                Periods = Enumerable.Range(1, 12)
                                                    .GroupJoin(
                                                                ld,
                                                                range => range,
                                                                ldr => ldr.CollectionPeriod,
                                                                (irange, ldRows) => new
                                                                {
                                                                    CollectionPeriod = irange,
                                                                    LearningDeliveryRows = ldRows,
                                                                })
                                                                .SelectMany(
                                                                    result => result.LearningDeliveryRows.DefaultIfEmpty(),
                                                                    (x, y) => new Period
                                                                    {
                                                                        PeriodId = x.CollectionPeriod,
                                                                        Value = y.PaymentValue,
                                                                    }).ToList(),
                            }).ToList(),
                        })
                        .ToListAsync(cancellationToken);

                return learningDeliveries;
            }
        }

        public async Task<ICollection<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var easContext = _easContext())
            {
                return await easContext.EasSubmissions.Select(l => Convert.ToInt32(l.Ukprn)).Distinct().ToListAsync(cancellationToken);
            }
        }
    }
}