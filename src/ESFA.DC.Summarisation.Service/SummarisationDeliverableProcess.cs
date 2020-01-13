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
    public class SummarisationDeliverableProcess : ISummarisationService
    {
        public string ProcessType => ProcessTypeConstants.Deliverable;

        public IEnumerable<SummarisedActual> Summarise(List<FundingStream> fundingStreams, ILearningProvider provider, IEnumerable<IFcsContractAllocation> allocations, IEnumerable<CollectionPeriod> collectionPeriods, ISummarisationMessage summarisationMessage)
        {
            return allocations.Where(w => w.FundingStreamPeriodCode.Equals("ESF1420", StringComparison.OrdinalIgnoreCase)).SelectMany(all => Summarise(fundingStreams, provider, all, collectionPeriods));
        }

        public IEnumerable<SummarisedActual> Summarise(List<FundingStream> fundingStreams, ILearningProvider provider, IFcsContractAllocation allocation, IEnumerable<CollectionPeriod> collectionPeriods)
        {
            return fundingStreams.SelectMany(fs => Summarise(fs, provider, allocation, collectionPeriods));
        }

        public IEnumerable<SummarisedActual> Summarise(FundingStream fundingStream, ILearningProvider provider, IFcsContractAllocation allocation, IEnumerable<CollectionPeriod> collectionPeriods)
        {
            var summarisedActuals = new List<SummarisedActual>();

            foreach (var fundLine in fundingStream.FundLines)
            {
                var periodisedData = provider
                    .LearningDeliveries
                    .Where(ld => ld.ConRefNumber != null && ld.ConRefNumber.Trim().Equals(allocation.ContractAllocationNumber, StringComparison.OrdinalIgnoreCase))
                    .SelectMany(x => x.PeriodisedData.Where(w => w.DeliverableCode == fundLine.DeliverableCode));

                var periods = GetPeriodsForFundLine(periodisedData, fundLine);

                summarisedActuals.AddRange(SummarisePeriods(periods, fundLine, collectionPeriods, allocation));
            }

            return summarisedActuals
                .GroupBy(grp => grp.Period)
                .Select(g =>
                    new SummarisedActual
                    {
                        OrganisationId = allocation.DeliveryOrganisation,
                        DeliverableCode = fundingStream.DeliverableLineCode,
                        FundingStreamPeriodCode = fundingStream.PeriodCode,
                        Period = g.Key,
                        ActualValue = Math.Round(g.Sum(x => x.ActualValue),2),
                        ActualVolume = g.Sum(x => x.ActualVolume),
                        ContractAllocationNumber = allocation.ContractAllocationNumber,
                        PeriodTypeCode = PeriodTypeCodeConstants.CalendarMonth
                    });
        }

        public IEnumerable<IPeriod> GetPeriodsForFundLine(IEnumerable<IPeriodisedData> periodisedData, FundLine fundLine)
        {
            if (fundLine.UseAttributes)
            {
                periodisedData = periodisedData.Where(pd => fundLine.Attributes.Contains(pd.AttributeName));
            }

            return periodisedData.SelectMany(fpd => fpd.Periods);
        }

        public IEnumerable<SummarisedActual> SummarisePeriods(IEnumerable<IPeriod> periods, FundLine fundLine, IEnumerable<CollectionPeriod> collectionPeriods, IFcsContractAllocation allocation)
        {
            var filteredCollectonPeriods = collectionPeriods.Where(cp => cp.ActualsSchemaPeriod >= allocation.ContractStartDate && cp.ActualsSchemaPeriod <= allocation.ContractEndDate);

            var summarisedPeriods = periods
                       .GroupBy(g => new { g.CalendarYear, g.CalendarMonth })
                       .Select(pg => new
                       {
                           CalendarYear = pg.Key.CalendarYear,
                           CalendarMonth = pg.Key.CalendarMonth,
                           ActualValue = pg.Where(w => w.Value.HasValue).Sum(sw => sw.Value.Value),
                           ActualVolume = fundLine.CalculateVolume ? pg.Where(w => w.Volume.HasValue).Sum(sw => sw.Volume.Value) : 0
                       });

            if (summarisedPeriods.All(w => w.ActualValue == 0 && w.ActualVolume == 0))
            {
                return new List<SummarisedActual>();
            }

            return (filteredCollectonPeriods
              .GroupJoin(summarisedPeriods,
                       cp => new { cp.CalendarYear, cp.CalendarMonth },
                       p => new { p.CalendarYear, p.CalendarMonth },
                       (cp, p) => new { Period = p, CollectionPeriod = cp }
                   )).SelectMany(grp => grp.Period.DefaultIfEmpty(), (outGrp, outPeriod) => new
                   {
                       ActualsSchemaPeriod = outGrp.CollectionPeriod.ActualsSchemaPeriod,
                       ActualValue = outPeriod == null ? decimal.Zero : outPeriod.ActualValue,
                       ActualVolume = outPeriod == null ? 0 : outPeriod.ActualVolume,
                   })
              .GroupBy(pg => pg.ActualsSchemaPeriod)
              .Select(g => new SummarisedActual
              {
                  Period = g.Key,
                  ActualValue = g.Sum(sw => sw.ActualValue),
                  ActualVolume = g.Sum(sw => sw.ActualVolume),
              });
        }
    }
}