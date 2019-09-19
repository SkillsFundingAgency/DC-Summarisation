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
    public class LevyProvider : ILearningDeliveryProvider
    {
        private readonly Func<IDASPaymentsContext> _dasContext;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.Apps1920_Levy);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.Apps1920);

        public LevyProvider(Func<IDASPaymentsContext> dasContext)
        {
            _dasContext = dasContext;
        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var contextFactory = _dasContext())
            {
                return await contextFactory.Payments.Where(w => w.ContractType == 1).Select(l => Convert.ToInt32(l.Ukprn)).Distinct().ToListAsync(cancellationToken);
            }
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            int currentCollectionYear = summarisationMessage.CollectionYear;

            int currentCollectionPeriod = summarisationMessage.CollectionMonth;

            int previousCollectionYear = summarisationMessage.CollectionYear - 101;
            int previousCollectionMonth = 0;

            switch (summarisationMessage.CollectionMonth)
            {
                case 2:
                    previousCollectionMonth = 13;
                    break;
                case 3:
                    previousCollectionMonth = 14;
                    break;
                default:
                    previousCollectionMonth = 0;
                    break;
            }

            using (var contextFactory = _dasContext())
            {
                return await contextFactory.Payments
                             .Where(p => p.Ukprn == ukprn && p.ContractType == 1
                                    && (
                                        (p.AcademicYear == currentCollectionYear && p.CollectionPeriod == currentCollectionPeriod)
                                        ||
                                        (p.AcademicYear == previousCollectionYear && p.CollectionPeriod == previousCollectionMonth))
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
                                            CollectionMonth = pd.CollectionPeriod,
                                            CollectionYear = pd.AcademicYear,
                                            Value = pd.Amount
                                        }
                                     }
                                 }).ToList()
                             }).ToListAsync(cancellationToken);
            }
        }

        public Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
