using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.output.Model;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Main1819.Service
{
    public class SummarisationService : ISummarisationService
    {
        public IEnumerable<SummarisedActual> Summarise(FundingType fundingType, Provider provider)
        {
            return fundingType.FundingStreams.SelectMany(fs => SummariseByFundingStream(fs, provider));
        }

        public IEnumerable<SummarisedActual> SummariseByFundingStream(FundingStream fundingStream, Provider provider)
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
                        DeliverableCode = fundingStream.DeliverableLineCode,
                        FundingStreamPeriodCode = fundingStream.PeriodCode,
                        Period = g.Key,
                        ActualValue = g.Sum(x => x.ActualValue)
                    });
        }

        public IEnumerable<Period> GetPeriodsForFundLine(IEnumerable<PeriodisedData> periodisedData, FundLine fundLine)
        {
            if (fundLine.UseAttributes)
            {
                periodisedData = periodisedData.Where(pd => fundLine.Attributes.Contains(pd.AttributeName));
            }

            return periodisedData.SelectMany(fpd => fpd.Periods);
        }
                 
        public IEnumerable<SummarisedActual> SummarisePeriods(IEnumerable<Period> periods)
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
