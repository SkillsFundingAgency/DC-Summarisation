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
using ESFA.DC.Summarisation.NCS.Service.Extensions;
using Microsoft.EntityFrameworkCore.SqlServer.Query.ExpressionTranslators.Internal;

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

            var fcsAllocation = allocations.FirstOrDefault(a => a.DeliveryUkprn == providerFundingData.Provider.UKPRN 
                                                && a.UoPcode.Equals(providerFundingData.Provider.TouchpointId,StringComparison.OrdinalIgnoreCase)
                                                && a.FundingStreamPeriodCode.Equals(fundingStream.PeriodCode, StringComparison.OrdinalIgnoreCase));

            if (fcsAllocation == null)
            {
                return summarisedActuals;
            }

            foreach (var outcomeType in fundingStream.OutcomeTypes)
            {
                var fundingValues = providerFundingData
                    .FundingValues
                    .Where(ld => ld.OutcomeType == outcomeType).ToList();
                    
                summarisedActuals.AddRange(SummarisePeriods(fundingValues, collectionPeriods));
            }

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
                        ActualValue = Math.Round(g.Sum(x => x.ActualValue), 2),
                        ContractAllocationNumber = fcsAllocation.ContractAllocationNumber,
                        PeriodTypeCode = PeriodTypeCodeConstants.CalendarMonth
                    }).ToList();
        }

        public ICollection<SummarisedActual> SummarisePeriods(ICollection<FundingValue> fundingValues, ICollection<CollectionPeriod> collectionPeriods)
        {
            var groupedFundingValues = fundingValues
                .GroupBy(fv => fv.CollectionYear)
                .ToDictionary(
                g => g.Key,
                g => g.GroupBy(h => h.CalendarMonth)
                .ToDictionary(
                    i => i.Key,
                    i => i.Sum(p => p.Value)));

            return collectionPeriods
                .Select(cp => new SummarisedActual()
                {
                    Period = cp.ActualsSchemaPeriod,
                    ActualValue = groupedFundingValues.GetValueOrDefault(cp.CollectionYear).GetValueOrDefault(cp.CalendarMonth, 0)
                }).ToList();

        }
    }
}
