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
using ESFA.DC.Summarisation.Data.Input.Interface;

namespace ESFA.DC.Summarisation.Main1920.Service.Providers
{
    public class EasProvider : AbstractLearningProviderProvider, ISummarisationInputDataProvider<ILearningProvider>
    {
        private readonly Func<IEasdbContext> _easContext;

        public string SummarisationType => SummarisationTypeConstants.Main1920_EAS;

        public string CollectionType => CollectionTypeConstants.ILR1920;

        public EasProvider(Func<IEasdbContext> easContext)
        {
            _easContext = easContext;
        }

        public async Task<ILearningProvider> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            using (var easContext = _easContext())
            {
                var learningDeliveries = await easContext.EasSubmissionValues
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
                                                                } as IPeriod).ToList()

                            }).ToList()

                        }
                        ).ToListAsync(cancellationToken);

                return BuildLearningProvider(ukprn, learningDeliveries);
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