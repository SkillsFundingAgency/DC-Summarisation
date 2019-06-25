using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using ESFA.DC.EAS1819.EF.Interface;

namespace ESFA.DC.Summarisation.Main1819.Data.Providers
{
    public class EasProvider : ILearningDeliveryProvider
    {
        private readonly IEasdbContext _easContext;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.Main1819_EAS);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.ILR1819);

        public EasProvider(IEasdbContext easContext)
        {
            _easContext = easContext;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            return await _easContext.EasSubmissionValues
                            .Where(sv => sv.EasSubmission.Ukprn == ukprn.ToString())
                            .Join(_easContext.PaymentTypes, sv => sv.PaymentId, p => p.PaymentId, (value, type) => new { value.CollectionPeriod, value.PaymentValue, type.PaymentName })
                            .GroupBy(x => x.PaymentName)
                            .Select(ld => new LearningDelivery
                            {
                                LearnRefNumber = "",
                                AimSeqNumber = 0,
                                Fundline = ld.Key,
                                PeriodisedData = ld.Select(pd => new PeriodisedData
                                {
                                    AttributeName = "",
                                    Periods = new List<Period>
                                    {
                                        new Period
                                        {
                                            PeriodId = pd.CollectionPeriod,
                                            Value = pd.PaymentValue
                                        }
                                    }
                                }).ToList()
                            }).ToListAsync(cancellationToken);
        }
       
        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            return await _easContext.EasSubmissions.Select(l => Convert.ToInt32(l.Ukprn)).Distinct().ToListAsync(cancellationToken);
        }

        public Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken) => ProvideAsync(ukprn, cancellationToken);

    }
}
