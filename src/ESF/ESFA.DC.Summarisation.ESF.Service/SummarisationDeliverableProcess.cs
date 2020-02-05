using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Summarisation.ESF.Interfaces;
using ESFA.DC.Summarisation.ESF.Model;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using ESFA.DC.Summarisation.ESF.Model.Config;

namespace ESFA.DC.Summarisation.ESF.Service
{
    public class SummarisationDeliverableProcess : ISummarisationService
    {
        private const string FSPcode = "ESF1420";

        public ICollection<SummarisedActual> Summarise(ICollection<FundingStream> fundingStreams, LearningProvider provider, ICollection<FcsContractAllocation> allocations, ICollection<CollectionPeriod> collectionPeriods, ISummarisationMessage summarisationMessage)
        {
            var esfAllocations = allocations.Where(w => w.FundingStreamPeriodCode.Equals(FSPcode, StringComparison.OrdinalIgnoreCase));

            var summarisedActuals = new List<SummarisedActual>();

            foreach (var allocation in esfAllocations)
            {
                var allocationSummarisedActuals = Summarise(fundingStreams, provider, allocation, collectionPeriods);

                summarisedActuals.AddRange(allocationSummarisedActuals);
            }

            return summarisedActuals;
        }

        public ICollection<SummarisedActual> Summarise(ICollection<FundingStream> fundingStreams, LearningProvider provider, FcsContractAllocation allocation, ICollection<CollectionPeriod> collectionPeriods)
        {
            var summarisedActuals = new List<SummarisedActual>();

            foreach (var fundingStream in fundingStreams)
            {
                var fundingStreamSummarisedActuals = Summarise(fundingStream, provider, allocation, collectionPeriods);

                summarisedActuals.AddRange(fundingStreamSummarisedActuals);
            }

            return summarisedActuals;
        }

        public ICollection<SummarisedActual> Summarise(FundingStream fundingStream, LearningProvider provider, FcsContractAllocation allocation, ICollection<CollectionPeriod> collectionPeriods)
        {
            var summarisedActuals = new List<SummarisedActual>();

            foreach (var fundLine in fundingStream.DeliverableLines)
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
                    }).ToList();
        }

        public ICollection<Period> GetPeriodsForFundLine(IEnumerable<PeriodisedData> periodisedData, DeliverableLine fundLine)
        {
            if (fundLine.UseAttributes)
            {
                periodisedData = periodisedData.Where(pd => fundLine.Attributes.Contains(pd.AttributeName));
            }

            return periodisedData.SelectMany(fpd => fpd.Periods).ToList();
        }

        public ICollection<SummarisedActual> SummarisePeriods(ICollection<Period> periods, DeliverableLine fundLine, ICollection<CollectionPeriod> collectionPeriods, FcsContractAllocation allocation)
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
                       }).ToList();

            if (summarisedPeriods.All(w => w.ActualValue == 0 && w.ActualVolume == 0))
            {
                return Array.Empty<SummarisedActual>();
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
              }).ToList();
        }
    }
}