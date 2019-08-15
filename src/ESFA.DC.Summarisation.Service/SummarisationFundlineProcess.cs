using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Data.Output.Model;
using ESFA.DC.Summarisation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.Summarisation.Service
{
    public class SummarisationFundlineProcess : ISummarisationService
    {
        public string ProcessType => nameof(Configuration.Enum.ProcessType.Fundline);

        public IEnumerable<SummarisedActual> Summarise(
            List<FundingStream> fundingStreams,
            IProvider provider,
            IEnumerable<IFcsContractAllocation> allocations,
            IEnumerable<CollectionPeriod> collectionPeriods,
            ISummarisationMessage summarisationMessage)
        {
            return fundingStreams.SelectMany(fs => Summarise(fs, provider, allocations, collectionPeriods));
        }

        public IEnumerable<SummarisedActual> Summarise(
            FundingStream fundingStream,
            IProvider provider,
            IEnumerable<IFcsContractAllocation> allocations,
            IEnumerable<CollectionPeriod> collectionPeriods)
        {
            var summarisedActuals = new List<SummarisedActual>();

            foreach (var fundLine in fundingStream.FundLines)
            {
                IEnumerable<IPeriodisedData> periodisedData;

                if (fundingStream.ApprenticeshipContractType == 1 || fundingStream.ApprenticeshipContractType == 2)
                {
                    periodisedData = provider
                   .LearningDeliveries
                   .Where(ld => ld.Fundline.Equals(fundLine.Fundline, StringComparison.OrdinalIgnoreCase) )
                   .SelectMany(x => x.PeriodisedData
                                        .Where(pd => pd.ApprenticeshipContractType == fundingStream.ApprenticeshipContractType
                                                                && fundingStream.FundingSources.Contains(pd.FundingSource)
                                                                && fundingStream.TransactionTypes.Contains(pd.TransactionType)));
                }
                else
                {
                     periodisedData = provider
                    .LearningDeliveries
                    .Where(ld => ld.Fundline.Equals(fundLine.Fundline, StringComparison.OrdinalIgnoreCase))
                    .SelectMany(x => x.PeriodisedData);
                }

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
                        PeriodTypeCode = SummarisationConstants.PeriodTypeCode
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
