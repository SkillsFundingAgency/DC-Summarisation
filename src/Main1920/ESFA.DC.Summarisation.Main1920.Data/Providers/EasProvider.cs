using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using ESFA.DC.EAS1920.EF.Interface;
using ESFA.DC.Summarisation.Constants;

namespace ESFA.DC.Summarisation.Main1920.Data.Providers
{
    public class EasProvider : ISummarisationInputDataProvider<IList<LearningDelivery>>
    {
        private readonly Func<IEasdbContext> _easContext;

        public string SummarisationType => SummarisationTypeConstants.Main1920_EAS;

        public string CollectionType => CollectionTypeConstants.ILR1920;

        public EasProvider(Func<IEasdbContext> easContext)
        {
            _easContext = easContext;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            using (var easContext = _easContext())
            {
                return await easContext.EasSubmissionValues
                        .Where(sv => sv.EasSubmission.Ukprn == ukprn.ToString())
                        .Join(easContext.PaymentTypes, sv => sv.PaymentId, p => p.PaymentId, (value, type) => new { value.CollectionPeriod, value.PaymentValue, type.PaymentName })
                        .GroupBy(x => x.PaymentName)
                        .Select(ld => new LearningDelivery
                        {
                            LearnRefNumber = "",
                            AimSeqNumber = 0,
                            Fundline = ld.Key,
                            PeriodisedData = ld.GroupBy(x => x.PaymentName).Select(pd => new PeriodisedData
                            {
                                AttributeName = "",
                                Periods = Enumerable.Range(1, 12)
                                                    .GroupJoin(ld,
                                                                range => range,
                                                                ldr => ldr.CollectionPeriod,
                                                                (irange, ldRows) => new
                                                                {
                                                                    CollectionPeriod = irange,
                                                                    LearningDeliveryRows = ldRows
                                                                })
                                                                .SelectMany(result => result.LearningDeliveryRows.DefaultIfEmpty(),
                                                                (x, y) => new Period
                                                                {
                                                                    PeriodId = x.CollectionPeriod,
                                                                    Value = y.PaymentValue
                                                                }
                                                                ).ToList()

                            }).ToList()

                        }
                        ).ToListAsync(cancellationToken);
            }
        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var easContext = _easContext())
            {
                return await easContext.EasSubmissions.Select(l => Convert.ToInt32(l.Ukprn)).Distinct().ToListAsync(cancellationToken);
            }
        }
    }
}