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
    public class LevyProvider : ILearningDeliveryProvider
    {
        private readonly IDASPaymentsContext _dasContext;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.Apps1819_Levy);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.Apps1819);

        public LevyProvider(IDASPaymentsContext dasContext)
        {
            _dasContext = dasContext;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            return await _dasContext.Payments
                             .Where(p => p.Ukprn == ukprn)
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
                                            PeriodId = pd.CollectionPeriod,
                                            Value = pd.Amount
                                        }
                                     }
                                 }).ToList()
                             }).ToListAsync(cancellationToken);

        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            return await _dasContext.Payments.Select(l => Convert.ToInt32(l.Ukprn)).Distinct().ToListAsync(cancellationToken);
        }

    }
}
