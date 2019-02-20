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
                SummariseByFundingStream(fundingStream, provider, summarisedActuals);
            }

            return summarisedActuals;
        }

        public void SummariseByFundingStream(FundingStream fundingStream, Provider provider, List<SummarisedActual> summarisedActuals)
        {
            foreach (var fundLine in fundingStream.FundLines)
            {
                var test = new FundingLineType
                {
                    DeliverableLineCode = fundingStream.DeliverableLineCode,
                    PeriodCode = fundingStream.PeriodCode,
                    FundLine = fundLine.Fundline
                };

                var learningDeliveries = provider.LearningDeliveries.Where(ld => ld.Fundline == fundLine.Fundline);

                summarisedActuals.AddRange(SummariseByFundLine(learningDeliveries, test, new HashSet<string>(fundLine.Attributes ?? new List<string>())));
            }
        }

        //public IEnumerable<SummarisedActual> Summarise(FundingType fundingType, Provider provider, HashSet<string> attributes)
        //{
        //    var fundingLineTypes = fundingType.FundingStreams
        //        .SelectMany(fs => fs.FundLines
        //            .Select(fl => new FundingLineType
        //            {
        //                FundLine = fl.Fundline,
        //                DeliverableLineCode = fs.DeliverableLineCode,
        //                PeriodCode = fs.PeriodCode
        //            }));

        //    var groupedLearningDeliveries = provider.LearningDeliveries.GroupBy(ld => ld.Fundline);

        //    var summarisedActuals = new List<SummarisedActual>();

        //    foreach (var group in groupedLearningDeliveries)
        //    {
        //        var fundinglineType = fundingLineTypes.First(flt => flt.FundLine == group.Key);

        //        summarisedActuals.AddRange(SummariseByFundLine(group.ToList(), fundinglineType, attributes));
        //    }

        //    return summarisedActuals;
        //}

        public IEnumerable<SummarisedActual> SummariseByFundLine(IEnumerable<LearningDelivery> learningDeliveries, FundingLineType fundinglineType, HashSet<string> attributes)
        {
            var summarisedActuals = new List<SummarisedActual>();

            var sumByAttribute = SummariseByAttribute(learningDeliveries.SelectMany(x => x.PeriodisedData), attributes);

            var grpSummarised = sumByAttribute.GroupBy(grp => grp.Period);

            foreach (var periodGroup in grpSummarised)
            {
                summarisedActuals.Add(new SummarisedActual
                {
                    FundingStreamPeriodCode = fundinglineType.PeriodCode,
                    DeliverableCode = fundinglineType.DeliverableLineCode,
                    Period = periodGroup.Key,
                    ActualValue = periodGroup.ToList().Sum(x => x.ActualValue)
                });
            }

            return summarisedActuals;
        }

        public IEnumerable<SummarisedActual> SummariseByAttribute(IEnumerable<PeriodisedData> periodisedData, HashSet<string> attributes)
        {
            var filteredPeriodisedData = periodisedData.Where(pd => attributes.Contains(pd.AttributeName));

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

    public class FundingLineType
    {
        public string FundLine { get; set; }

        public int DeliverableLineCode { get; set; }

        public string PeriodCode { get; set; }
    }
}
