using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Output.Model;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.Summarisation.Service
{
    public class SummarisationDeliverableProcess : ISummarisationService
    {
        public string ProcessType => nameof(Configuration.Enum.ProcessType.Deliverable);

        public IEnumerable<SummarisedActual> Summarise(List<FundingStream> fundingStreams, IProvider provider, IEnumerable<IFcsContractAllocation> allocations, IEnumerable<CollectionPeriod> collectionPeriods)
        {
            return allocations.Where(w => w.FundingStreamPeriodCode == "ESF1420").SelectMany(all => Summarise(fundingStreams, provider, all, collectionPeriods));
        }

        public IEnumerable<SummarisedActual> Summarise(List<FundingStream> fundingStreams, IProvider provider, IFcsContractAllocation allocation, IEnumerable<CollectionPeriod> collectionPeriods)
        {
            return fundingStreams.SelectMany(fs => Summarise(fs, provider, allocation, collectionPeriods));
        }

        public IEnumerable<SummarisedActual> Summarise(FundingStream fundingStream, IProvider provider, IFcsContractAllocation allocation, IEnumerable<CollectionPeriod> collectionPeriods)
        {
            var summarisedActuals = new List<SummarisedActual>();

            foreach (var fundLine in fundingStream.FundLines)
            {
                var periodisedData = provider
                    .LearningDeliveries
                    .Where(ld => ld.ConRefNumber == allocation.ContractAllocationNumber)
                    .SelectMany(x => x.PeriodisedData.Where(w => w.DeliverableCode == fundLine.DeliverableCode));

                var periods = GetPeriodsForFundLine(periodisedData, fundLine);

                summarisedActuals.AddRange(SummarisePeriods(periods, fundLine, collectionPeriods));
            }

            return summarisedActuals
                .GroupBy(grp => grp.Period)
                .Select(g =>
                    new SummarisedActual
                    {
                        OrganisationId = allocation.DeliveryOrganisation,
                        DeliverableCode = fundingStream.DeliverableLineCode,
                        FundingStreamPeriodCode = fundingStream.PeriodCode,
                        Period = collectionPeriods.First(cp => cp.Period == g.Key).ActualsSchemaPeriod,
                        ActualValue = g.Sum(x => x.ActualValue),
                        ActualVolume = g.Sum(x => x.ActualVolume),
                        ContractAllocationNumber = allocation.ContractAllocationNumber,
                        PeriodTypeCode = "AY"
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

        public IEnumerable<SummarisedActual> SummarisePeriods(IEnumerable<IPeriod> periods, FundLine fundLine, IEnumerable<CollectionPeriod> collectionPeriods)
        {
            return periods
                .Join(collectionPeriods, p => new {p.CalendarMonth, p.CalendarYear }, cp => new { cp.CalendarMonth, cp.CalendarYear } , (p, cp) => new {cp.Period, p.Value, p.Volume } )
                .GroupBy(pg => pg.Period)
                .Select(g => new SummarisedActual
                {
                    Period = g.Key,
                    ActualValue = g.Where(w => w.Value.HasValue).Sum(sw => sw.Value.Value),
                    ActualVolume = fundLine.CalculateVolume ? g.Where(w => w.Volume.HasValue).Sum(sw => sw.Volume.Value) :  0
                });
        }
    }
}