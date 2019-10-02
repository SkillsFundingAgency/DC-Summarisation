using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Output.Model;
using ESFA.DC.Summarisation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.Summarisation.Service
{
    public class SummarisationPaymentsProcess : ISummarisationService
    {
        public string ProcessType => nameof(Configuration.Enum.ProcessType.Payments);

        public IEnumerable<SummarisedActual> Summarise(
            List<FundingStream> fundingStreams,
            IProvider provider,
            IEnumerable<IFcsContractAllocation> allocations,
            IEnumerable<CollectionPeriod> collectionPeriods,
            ISummarisationMessage summarisationMessage)
        {
            return fundingStreams.SelectMany(fs => Summarise(fs, provider, allocations, collectionPeriods, summarisationMessage));
        }

        public IEnumerable<SummarisedActual> Summarise(
            FundingStream fundingStream,
            IProvider provider,
            IEnumerable<IFcsContractAllocation> allocations,
            IEnumerable<CollectionPeriod> collectionPeriods,
            ISummarisationMessage summarisationMessage)
        {
            var summarisedActuals = new List<SummarisedActual>();

            int currentCollectionPeriod = summarisationMessage.CollectionMonth;

            int previousCollectionYear = 0;
            int previousCollectionMonth = 0;

            switch (summarisationMessage.CollectionMonth)
            {
                case 2:
                    previousCollectionMonth = 13;
                    previousCollectionYear = summarisationMessage.CollectionYear - 101;
                    break;
                case 3:
                    previousCollectionMonth = 14;
                    previousCollectionYear = summarisationMessage.CollectionYear - 101;
                    break;
                default:
                    previousCollectionMonth = 0;
                    previousCollectionYear = 0;
                    break;
            }

            if (fundingStream.PeriodCode.Equals(ConstantKeys.Levy1799, StringComparison.OrdinalIgnoreCase) 
                || fundingStream.PeriodCode.Equals(ConstantKeys.NonLevy2019, StringComparison.OrdinalIgnoreCase))
            {
                collectionPeriods = collectionPeriods.Where(w => 
                                                                (w.CollectionYear == summarisationMessage.CollectionYear && w.CollectionMonth == summarisationMessage.CollectionMonth)
                                                                ||
                                                                (w.CollectionYear == previousCollectionYear && w.CollectionMonth == previousCollectionMonth)
                                                            );
            }
            else if (fundingStream.PeriodCode.Equals(ConstantKeys.NonLevy_APPS1920, StringComparison.OrdinalIgnoreCase))
            {
                collectionPeriods = collectionPeriods.Where(w => w.CollectionYear == summarisationMessage.CollectionYear && w.CollectionMonth <= 12);
            }
            else if (fundingStream.PeriodCode.Equals(ConstantKeys.NonLevy_APPS1819, StringComparison.OrdinalIgnoreCase))
            {
                collectionPeriods = collectionPeriods.Where(w => w.CollectionYear == previousCollectionYear && w.CollectionMonth <= 12);
            }
            else if (fundingStream.PeriodCode.Equals(ConstantKeys.NonLevy_ANLAP2018, StringComparison.OrdinalIgnoreCase)
                    || fundingStream.PeriodCode.Equals(ConstantKeys.NonLevy_1618NLAP2018, StringComparison.OrdinalIgnoreCase))
            {
                // No Filter needed. Need every thing from Collection Periods configuration
            }
            else 
            {
                collectionPeriods = collectionPeriods.Where(w => w.CollectionYear == summarisationMessage.CollectionYear || w.CollectionYear == previousCollectionYear);
            }

            foreach (var fundLine in fundingStream.FundLines)
            {
                IEnumerable<IPeriodisedData> periodisedData;

                if (fundLine.LineType.Equals(ConstantKeys.LineType_EAS, StringComparison.OrdinalIgnoreCase))
                {
                    periodisedData = provider
                      .LearningDeliveries
                      .Where(ld => ld.Fundline.Equals(fundLine.Fundline, StringComparison.OrdinalIgnoreCase))
                      .SelectMany(x => x.PeriodisedData);
                }
                else
                {
                    periodisedData = provider
                   .LearningDeliveries
                   .Where(ld => ld.Fundline.Equals(fundLine.Fundline, StringComparison.OrdinalIgnoreCase))
                   .SelectMany(x => x.PeriodisedData
                                        .Where(pd => pd.ApprenticeshipContractType == fundingStream.ApprenticeshipContractType
                                                                && fundingStream.FundingSources.Contains(pd.FundingSource)
                                                                && fundingStream.TransactionTypes.Contains(pd.TransactionType)));
                }

                var periods = GetPeriodsForFundLine(periodisedData, fundLine);

                summarisedActuals.AddRange(SummarisePeriods(periods, collectionPeriods));
            }

            var fcsAllocations = allocations.ToDictionary(a => a.FundingStreamPeriodCode, StringComparer.OrdinalIgnoreCase);

            return summarisedActuals
                .GroupBy(grp => grp.Period)
                .Select(g =>
                    new SummarisedActual
                    {
                        OrganisationId = fcsAllocations[fundingStream.PeriodCode].DeliveryOrganisation,
                        DeliverableCode = fundingStream.DeliverableLineCode,
                        FundingStreamPeriodCode = fundingStream.PeriodCode,
                        Period = g.Key,
                        ActualValue = Math.Round(g.Sum(x => x.ActualValue), 2),
                        ContractAllocationNumber = fcsAllocations[fundingStream.PeriodCode].ContractAllocationNumber,
                        PeriodTypeCode = SummarisationConstants.PeriodTypeCode
                    });

        }

        public IEnumerable<IPeriod> GetPeriodsForFundLine(IEnumerable<IPeriodisedData> periodisedData, FundLine fundLine)
        {
            if (fundLine.UseAttributes)
            {
                periodisedData = periodisedData.Where(pd => fundLine.Attributes.Contains(pd.AttributeName));
            }

            if (fundLine.AcademicYear.HasValue)
                return periodisedData.SelectMany(fpd => fpd.Periods.Where(w => w.CollectionYear == fundLine.AcademicYear));
            else
                return periodisedData.SelectMany(fpd => fpd.Periods);
        }

        public IEnumerable<SummarisedActual> SummarisePeriods(IEnumerable<IPeriod> periods, IEnumerable<CollectionPeriod> collectionPeriods)
        {
            var summarisedPeriods = periods
                       .GroupBy(g => new { g.CollectionYear, g.CollectionMonth })
                       .Select(pg => new
                       {
                           CollectionYear = pg.Key.CollectionYear,
                           CollectionMonth = pg.Key.CollectionMonth,
                           Value = pg.Where(w => w.Value.HasValue).Sum(sw => sw.Value.Value),
                       });

            if (!summarisedPeriods.Any(w => w.Value > 0))
            {
                return new List<SummarisedActual>();
            }

            return (collectionPeriods
              .GroupJoin(summarisedPeriods,
                       cp => new { cp.CollectionYear, cp.CollectionMonth },
                       p => new { p.CollectionYear, p.CollectionMonth },
                       (cp, p) => new { Period = p, CollectionPeriod = cp }
                   )).SelectMany(grp => grp.Period.DefaultIfEmpty(), (outGrp, outPeriod) => new
                   {
                       CollectionYear = outGrp.CollectionPeriod.CollectionYear,
                       CollectionMonth = outGrp.CollectionPeriod.CollectionMonth,
                       ActualsSchemaPeriod = outGrp.CollectionPeriod.ActualsSchemaPeriod,
                       Value = outPeriod == null ? decimal.Zero : outPeriod.Value,
                   })
              .GroupBy(pg => pg.ActualsSchemaPeriod)
              .Select(g => new SummarisedActual
              {
                  Period = g.Key,
                  ActualValue = g.Sum(sw => sw.Value),
              });
        }
    }
}