using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using ESFA.DC.Summarisation.NCS.Interfaces;
using ESFA.DC.Summarisation.NCS.Model.Config;
using ESFA.DC.Summarisation.NCS.Model;

namespace ESFA.DC.Summarisation.NCS.Service
{
    public class SummarisationNCSProcess : ISummarisationService
    {
        public ICollection<SummarisedActual> Summarise(
            ICollection<FundingStream> fundingStreams,
            TouchpointProviderFundingData provider,
            ICollection<FcsContractAllocation> allocations,
            ICollection<CollectionPeriod> collectionPeriods)
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
            TouchpointProviderFundingData providerFundingData,
            ICollection<FcsContractAllocation> allocations,
            ICollection<CollectionPeriod> collectionPeriods)
        {
            var summarisedActuals = new List<SummarisedActual>();

            foreach (var outcomeType in fundingStream.OutcomeTypes)
            {
                var fundingValues = providerFundingData
                    .FundingValues
                    .Where(ld => ld.OutcomeType == outcomeType).ToList();
                    
                summarisedActuals.AddRange(SummarisePeriods(fundingValues, collectionPeriods));
            }

            var fcsAllocation = allocations.First(a => a.DeliveryUkprn == providerFundingData.Provider.UKPRN 
                                                && a.UoPcode.Equals(providerFundingData.Provider.TouchpointId,StringComparison.OrdinalIgnoreCase)
                                                && a.FundingStreamPeriodCode.Equals(fundingStream.PeriodCode, StringComparison.OrdinalIgnoreCase));

            return summarisedActuals
                .GroupBy(grp => grp.Period)
                .Select(g =>
                    new SummarisedActual
                    {
                        OrganisationId = fcsAllocation.DeliveryOrganisation,
                        UoPCode = fcsAllocation.UoPcode,
                        DeliverableCode = fundingStream.DeliverableLineCode,
                        FundingStreamPeriodCode = fundingStream.PeriodCode,
                        Period = g.Key,
                        ActualValue = Math.Round(g.Sum(x => x.ActualValue),2),
                        ContractAllocationNumber = fcsAllocation.ContractAllocationNumber,
                        PeriodTypeCode = PeriodTypeCodeConstants.CalendarMonth
                    }).ToList();
        }

        public ICollection<SummarisedActual> SummarisePeriods(ICollection<FundingValue> fundingValues, ICollection<CollectionPeriod> collectionPeriods)
        {
            var aggregatedFundingValues = fundingValues
                .GroupBy(g => new { g.CollectionYear, g.CalendarMonth })
                .Select(s => new 
                {
                    s.Key.CollectionYear,
                    s.Key.CalendarMonth,
                    Value = s.Sum(p => p.Value)
                });

            return (collectionPeriods
              .GroupJoin(aggregatedFundingValues,
                       cp => new { cp.CollectionYear, cp.CalendarMonth },
                       p => new { p.CollectionYear, p.CalendarMonth },
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
              }).ToList();
        }
    }
}
