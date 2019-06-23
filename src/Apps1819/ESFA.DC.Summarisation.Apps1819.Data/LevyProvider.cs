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
            //TODO : CollectionYears and Collection Period should be derived from Message
            List<int> CollectionYears = new List<int> { 1819 };
            int CollectionPeriod = 1;

            return await _dasContext.Payments
                             .Where(p => p.Ukprn == ukprn && p.ContractType == 1 && CollectionYears.Contains(p.AcademicYear) && p.CollectionPeriod == CollectionPeriod )
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
            return await _dasContext.Payments.Where(w => w.ContractType == 1).Select(l => Convert.ToInt32(l.Ukprn)).Distinct().ToListAsync(cancellationToken);
        }

    }
}
