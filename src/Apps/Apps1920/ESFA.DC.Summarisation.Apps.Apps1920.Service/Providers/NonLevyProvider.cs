using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DASPayments.EF.Interfaces;
using ESFA.DC.Summarisation.Apps.Interfaces;
using ESFA.DC.Summarisation.Apps.Model;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Apps.Apps1920.Service.Providers
{
    public class NonLevyProvider : AbstractLearningProviderProvider, ISummarisationInputDataProvider<LearningProvider>
    {
        private readonly Func<IDASPaymentsContext> _dasContext;

        public string CollectionType => CollectionTypeConstants.APPS;

        public NonLevyProvider(Func<IDASPaymentsContext> dasContext)
        {
            _dasContext = dasContext;
        }

        public async Task<ICollection<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var contextFactory = _dasContext())
            {
                return await contextFactory.Payments.Where(w => w.ContractType == ContractTypeConstants.NonLevy).Select(l => Convert.ToInt32(l.Ukprn)).Distinct().ToListAsync(cancellationToken);
            }
        }

        public async Task<LearningProvider> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            List<int> CollectionYears = new List<int>
            {
                summarisationMessage.CollectionYear,
                summarisationMessage.CollectionYear - 101,
            };

            using (var contextFactory = _dasContext())
            {
                var preSummarisedData = await contextFactory.Payments
                           .Where(p => p.Ukprn == ukprn
                                      && p.ContractType == ContractTypeConstants.NonLevy
                                      && (
                                            CollectionYears.Contains(p.AcademicYear)
                                            || p.LearningAimFundingLineType.Equals(FundlineConstants.Apps1618NonLevyContractProcured, StringComparison.OrdinalIgnoreCase)
                                            || p.LearningAimFundingLineType.Equals(FundlineConstants.Apps19plusNonLevyContractProcured, StringComparison.OrdinalIgnoreCase)
                                        )
                                   )
                                   .Select(q1 => new
                                   {
                                       q1.ContractType,
                                       q1.FundingSource,
                                       q1.TransactionType,
                                       q1.AcademicYear,
                                       q1.DeliveryPeriod,
                                       FundingLineType = q1.AcademicYear == summarisationMessage.CollectionYear ? q1.ReportingAimFundingLineType : q1.LearningAimFundingLineType,
                                       q1.Amount
                                   }
                                   )
                           .GroupBy(x => new { x.ContractType, x.FundingSource, x.TransactionType, x.AcademicYear, x.DeliveryPeriod, x.FundingLineType})
                           .Select(obj => new
                           {
                               ContractType = obj.Key.ContractType,
                               FundingSource = obj.Key.FundingSource,
                               TransactionType = obj.Key.TransactionType,
                               AcademicYear = obj.Key.AcademicYear,
                               DeliveryPeriod = obj.Key.DeliveryPeriod,
                               FundingLineType = obj.Key.FundingLineType,
                               Amount = obj.Sum(s => s.Amount)
                           }).ToListAsync(cancellationToken);

                var learningDeliveries = preSummarisedData
                            .GroupBy(x => x.FundingLineType, StringComparer.OrdinalIgnoreCase)
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
                                            CollectionMonth = pd.DeliveryPeriod,
                                            CollectionYear = pd.AcademicYear,
                                            Value = pd.Amount
                                        }
                                    }
                                }).ToList()
                            }).ToList();

                return BuildLearningProvider(ukprn, learningDeliveries);
            }
        }
    }
}
