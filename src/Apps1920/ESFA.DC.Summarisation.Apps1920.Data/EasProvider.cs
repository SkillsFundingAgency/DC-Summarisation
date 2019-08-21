using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using ESFA.DC.DASPayments.EF.Interfaces;

namespace ESFA.DC.Summarisation.Apps1920.Data
{
    public class EasProvider : ILearningDeliveryProvider
    {
        private readonly Func<IDASPaymentsContext> _dasContext;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.Apps1920_EAS);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.Apps1920);

        public EasProvider(Func<IDASPaymentsContext> easContext)
        {
            _dasContext = easContext;
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
            using (var easContext = _dasContext())
            {
                return await easContext.ProviderAdjustmentPayments
                        .Where(sv => sv.Ukprn == ukprn && sv.CollectionPeriodYear >= 2018)
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
                                        CollectionMonth = pd.SubmissionCollectionPeriod,
                                        CollectionYear = pd.SubmissionAcademicYear,
                                        Value = pd.Amount
                                    }
                                }
                            }).ToList()
                        }).ToListAsync(cancellationToken);
            }
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}