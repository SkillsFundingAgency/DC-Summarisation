using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.output.Model;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;

namespace ESFA.DC.Summarisation.Main1819.Service
{
    public class SummarisationService : ISummarisationService
    {
        public IEnumerable<SummarisedActual> Summarise(FundingType fundingType, IProvider provider, IEnumerable<IFcsContractAllocation> allocations, IEnumerable<CollectionPeriod> collectionPeriods)
        {
            return fundingType.FundingStreams.SelectMany(fs => Summarise(fs, provider, allocations, collectionPeriods));
        }

        public IEnumerable<SummarisedActual> Summarise(List<FundingStream> fundingStreams , IProvider provider, IEnumerable<IFcsContractAllocation> allocations, IEnumerable<CollectionPeriod> collectionPeriods)
        {
            return fundingStreams.SelectMany(fs => Summarise(fs, provider, allocations, collectionPeriods));
        }

        public IEnumerable<SummarisedActual> Summarise(FundingStream fundingStream, IProvider provider, IEnumerable<IFcsContractAllocation> allocations, IEnumerable<CollectionPeriod> collectionPeriods)
        {
            var summarisedActuals = new List<SummarisedActual>();

            foreach (var fundLine in fundingStream.FundLines)
            {
                var periodisedData = provider
                    .LearningDeliveries
                    .Where(ld => ld.Fundline == fundLine.Fundline)
                    .SelectMany(x => x.PeriodisedData);

                var periods = GetPeriodsForFundLine(periodisedData, fundLine);

                summarisedActuals.AddRange(SummarisePeriods(periods));
            }

            return summarisedActuals
                .GroupBy(grp => grp.Period)
                .Select(g =>
                    new SummarisedActual
                    {
                        OrganisationId = allocations.First(a => a.FundingStreamPeriodCode == fundingStream.PeriodCode)?.DeliveryOrganisation,
                        DeliverableCode = fundingStream.DeliverableLineCode,
                        FundingStreamPeriodCode = fundingStream.PeriodCode,
                        Period = collectionPeriods.First(cp => cp.Period == g.Key).ActualsSchemaPeriod,
                        ActualValue = g.Sum(x => x.ActualValue),
                        ContractAllocationNumber = allocations.First(a => a.FundingStreamPeriodCode == fundingStream.PeriodCode)?.ContractAllocationNumber,
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
                 
        public IEnumerable<SummarisedActual> SummarisePeriods(IEnumerable<IPeriod> periods)
        {
            return periods
                .GroupBy(pg => pg.PeriodId)
                .Select(g => new SummarisedActual
                {
                    Period = g.Key,
                    ActualValue = g.Where(p => p.Value.HasValue).Sum(p => p.Value.Value)
                });
        }
    }
}
