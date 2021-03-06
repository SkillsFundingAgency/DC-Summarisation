﻿using System;
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

namespace ESFA.DC.Summarisation.Apps.Providers
{
    public class LevyProvider : ISummarisationInputDataProvider
    {
        private readonly Func<IDASPaymentsContext> _dasContext;

        public string CollectionType => CollectionTypeConstants.APPS;

        public LevyProvider(Func<IDASPaymentsContext> dasContext)
        {
            _dasContext = dasContext;
        }

        public async Task<ICollection<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var contextFactory = _dasContext())
            {
                return await contextFactory.Payments.Where(w => w.ContractType == 1).Select(l => Convert.ToInt32(l.Ukprn)).Distinct().ToListAsync(cancellationToken);
            }
        }

        public async Task<ICollection<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
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
                var preSummarisedData = await contextFactory.Payments
                             .Where(p => p.Ukprn == ukprn && p.ContractType == ContractTypeConstants.Levy
                                    && (
                                        (p.AcademicYear == currentCollectionYear && p.CollectionPeriod == currentCollectionPeriod)
                                        ||
                                        (p.AcademicYear == previousCollectionYear && p.CollectionPeriod == previousCollectionMonth))
                                     )
                            .GroupBy(x => new { x.ContractType, x.FundingSource, x.TransactionType, x.AcademicYear, x.CollectionPeriod, x.ReportingAimFundingLineType })
                            .Select(obj => new
                            {
                                ContractType = obj.Key.ContractType,
                                FundingSource = obj.Key.FundingSource,
                                TransactionType = obj.Key.TransactionType,
                                AcademicYear = obj.Key.AcademicYear,
                                CollectionPeriod = obj.Key.CollectionPeriod,
                                ReportingAimFundingLineType = obj.Key.ReportingAimFundingLineType,
                                Amount = obj.Sum(s => s.Amount),
                            }).ToListAsync(cancellationToken);

                var learningDeliveries = preSummarisedData
                             .GroupBy(x => x.ReportingAimFundingLineType, StringComparer.OrdinalIgnoreCase)
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
                                            Value = pd.Amount,
                                        },
                                     },
                                 }).ToList(),
                             }).ToList();

                return learningDeliveries;
            }
        }
    }
}
