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
    public class SummarisationFundlineProcess : ISummarisationService
    {
        public string ProcessType => ProcessTypeConstants.Fundline;

        public ICollection<SummarisedActual> Summarise(
            ICollection<FundingStream> fundingStreams,
            ILearningProvider provider,
            ICollection<IFcsContractAllocation> allocations,
            ICollection<CollectionPeriod> collectionPeriods,
            ISummarisationMessage summarisationMessage)
        {
            var summarisedActuals = new List<SummarisedActual>();

            foreach (var fs in fundingStreams)
            {
                var fundingStreamSummarisedActuals = Summarise(fs, provider, allocations, collectionPeriods);

                summarisedActuals.AddRange(fundingStreamSummarisedActuals);
            }

            return summarisedActuals;
        }

        public ICollection<SummarisedActual> Summarise(
            FundingStream fundingStream,
            ILearningProvider provider,
            ICollection<IFcsContractAllocation> allocations,
            ICollection<CollectionPeriod> collectionPeriods)
        {
            var summarisedActuals = new List<SummarisedActual>();

            foreach (var fundLine in fundingStream.FundLines)
            {
                IEnumerable<IPeriodisedData> periodisedData;
               
                periodisedData = provider
                    .LearningDeliveries
                    .Where(ld => ld.Fundline.Equals(fundLine.Fundline, StringComparison.OrdinalIgnoreCase))
                    .SelectMany(x => x.PeriodisedData);

                var periods = GetPeriodsForFundLine(periodisedData, fundLine);

                summarisedActuals.AddRange(SummarisePeriods(periods));
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
                        Period = collectionPeriods.First(cp => cp.Period == g.Key).ActualsSchemaPeriod,
                        ActualValue = Math.Round(g.Sum(x => x.ActualValue),2),
                        ContractAllocationNumber = fcsAllocations[fundingStream.PeriodCode].ContractAllocationNumber,
                        PeriodTypeCode = PeriodTypeCodeConstants.CalendarMonth
                    }).ToList();
        }

        public ICollection<IPeriod> GetPeriodsForFundLine(IEnumerable<IPeriodisedData> periodisedData, FundLine fundLine)
        {
            if (fundLine.UseAttributes)
            {
                periodisedData = periodisedData.Where(pd => fundLine.Attributes.Contains(pd.AttributeName));
            }

            return periodisedData.SelectMany(fpd => fpd.Periods).ToList();
        }

        public ICollection<SummarisedActual> SummarisePeriods(ICollection<IPeriod> periods)
        {
            return periods
                .GroupBy(pg => pg.PeriodId)
                .Select(g => new SummarisedActual
                {
                    Period = g.Key,
                    ActualValue = g.Where(p => p.Value.HasValue).Sum(p => p.Value.Value)
                }).ToList();
        }
    }
}
