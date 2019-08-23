using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using ESFA.DC.DASPayments.EF.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Constants;

namespace ESFA.DC.Summarisation.Apps1920.Data
{
    public class NonLevyProvider : ILearningDeliveryProvider
    {
        private readonly Func<IDASPaymentsContext> _dasContext;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.Apps1920_NonLevy);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.Apps1920);

        public NonLevyProvider(Func<IDASPaymentsContext> dasContext)
        {
            _dasContext = dasContext;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var contextFactory = _dasContext())
            {
                return await contextFactory.Payments.Where(w => w.ContractType == ConstantKeys.ContractType_NonLevy).Select(l => Convert.ToInt32(l.Ukprn)).Distinct().ToListAsync(cancellationToken);
            }
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            List<int> CollectionYears = new List<int>();

            CollectionYears.Add(summarisationMessage.CollectionYear);

            using (var contextFactory = _dasContext())
            {
                return await contextFactory.Payments
                             .Where(p => p.Ukprn == ukprn 
                                        && p.ContractType == ConstantKeys.ContractType_NonLevy
                                        && CollectionYears.Contains(p.AcademicYear)
                                     )
                             .GroupBy(x => x.ReportingAimFundingLineType)
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
                                            CollectionMonth = pd.DeliveryPeriod,
                                            CollectionYear = pd.AcademicYear,
                                            Value = pd.Amount
                                        }
                                     }
                                 }).ToList()
                             }).ToListAsync(cancellationToken);
            }
        }
    }
}
