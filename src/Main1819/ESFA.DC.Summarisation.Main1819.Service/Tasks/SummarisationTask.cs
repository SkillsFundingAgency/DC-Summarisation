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
            var summarisedActuals = new List<SummarisedActual>();

            foreach (var fundingStream in fundingType.FundingStreams)
            {
                summarisedActuals.AddRange(SummariseByFundingStream(fundingStream, provider));
            }

            return summarisedActuals;
        }

        public IEnumerable<SummarisedActual> SummariseByFundingStream(FundingStream fundingStream, Provider provider)
        {
            var summarisedActuals = new List<SummarisedActual>();

            foreach (var fundLine in fundingStream.FundLines)
            {
                var learningDeliveries = provider.LearningDeliveries.Where(ld => ld.Fundline == fundLine.Fundline);

                summarisedActuals.AddRange(SummariseByAttribute(learningDeliveries.SelectMany(x => x.PeriodisedData), new HashSet<string>(fundLine.Attributes ?? new List<string>())));
            }

            var returnActuals = new List<SummarisedActual>();

            var grpSummarised = summarisedActuals.GroupBy(grp => grp.Period);

            foreach (var periodGroup in grpSummarised)
            {
                returnActuals.Add(new SummarisedActual
                {
                    DeliverableCode = fundingStream.DeliverableLineCode,
                    FundingStreamPeriodCode = fundingStream.PeriodCode,
                    Period = periodGroup.Key,
                    ActualValue = periodGroup.ToList().Sum(x => x.ActualValue)
                });
            }

            return returnActuals;
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
            var summarisedActuals = new List<SummarisedActual>();

            foreach (var groupedPeriods in periods.GroupBy(pg => pg.PeriodId))
            {
                summarisedActuals.Add(new SummarisedActual
                {
                    Period = groupedPeriods.Key,
                    ActualValue = groupedPeriods.Sum(p => p.Value) ?? 0
                });
            }

            return summarisedActuals;
        }
    }
}
