using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.output.Model;

namespace ESFA.DC.Summarisation.Main1819.Service.Tasks
{
    public class SummarisationTask
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
                var periodisedDatas = provider
                    .LearningDeliveries
                    .Where(ld => ld.Fundline == fundLine.Fundline)
                    .SelectMany(x => x.PeriodisedData);

                summarisedActuals.AddRange(SummariseByAttribute(periodisedDatas, new HashSet<string>(fundLine.Attributes ?? new List<string>())));
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

        public IEnumerable<SummarisedActual> SummariseByAttribute(IEnumerable<PeriodisedData> periodisedData, HashSet<string> attributes)
        {
            var filteredPeriodisedData = periodisedData;

            if (attributes.Any())
            {
                filteredPeriodisedData = periodisedData.Where(pd => attributes.Contains(pd.AttributeName));
            }
            
            return SummariseByPeriods(filteredPeriodisedData.SelectMany(fpd => fpd.Periods));
        }
         
        public IEnumerable<SummarisedActual> SummariseByPeriods(IEnumerable<Period> periods)
        {
            return periods
                .GroupBy(pg => pg.PeriodId)
                .Select(g => new SummarisedActual
                {
                    Period = g.Key,
                    ActualValue = g.Sum(p => p.Value) ?? 0
                });
        }
    }
}
