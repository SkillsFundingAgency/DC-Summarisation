using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using ESFA.DC.DASPayments.EF.Interfaces;

namespace ESFA.DC.Summarisation.Apps1819.Data
{
    public class NonLevyProvider : ILearningDeliveryProvider
    {
        private readonly IDASPaymentsContext _dasContext;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.Apps1819_NonLevy);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.Apps1819);

        public NonLevyProvider(IDASPaymentsContext dasContext)
        {
            _dasContext = dasContext;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            return await _dasContext.Payments.Where(w => w.ContractType == 2).Select(l => Convert.ToInt32(l.Ukprn)).Distinct().ToListAsync(cancellationToken);
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            List<int> CollectionYears = new List<int>();

            CollectionYears.Add(summarisationMessage.CollectionYear);

            return await _dasContext.Payments
                             .Where(p => p.Ukprn == ukprn && p.ContractType == 2 && CollectionYears.Contains(p.AcademicYear))
                             .GroupBy(x => x.LearningAimFundingLineType)
                             .Select(ld => new LearningDelivery
                             {
                                 Fundline = ld.Key,
                                 PeriodisedData = ld.Select(pd => new PeriodisedData
                                 {
                                     ApprenticeshipContractType = pd.ContractType,
                                     FundingSource = pd.FundingSource,
                                     TransactionType = pd.TransactionType,
                                     Periods = new List<Period>
                                     {
                                        new Period
                                        {
                                            PeriodId = pd.DeliveryPeriod,
                                            Value = pd.Amount
                                        }
                                     }
                                 }).ToList()
                             }).ToListAsync(cancellationToken);
        }
    }
}
