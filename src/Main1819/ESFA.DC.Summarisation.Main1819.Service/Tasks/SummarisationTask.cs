using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.output.Model;

namespace ESFA.DC.Summarisation.Main1819.Service.Tasks
{
    public class SummarisationTask
    {        
        public IEnumerable<SummarisedActual> Summarise(FundingType fundingType, IEnumerable<Provider> providers)
        {
           /* var fundlingLineTypes = fundingType.FundingStreams
                .SelectMany(fs => fs.FundLines
                .Select(fl => new
                {
                    fl.Fundline,
                    fs.DeliverableLineCode,
                    fs.PeriodCode
                }));

            var periodisedValuesSummationQuery = providers.Select(pr =>
                                                                        new
                                                                        {
                                                                            pr.UKPRN,
                                                                            SummedPeriodisedVaues = pr.LearningDeliveries
                                                                                                        .SelectMany(ld => ld.PeriodisedData
                                                                                                                .Where(pdw => _attributes.Contains(pdw.AttributeName))
                                                                                                                .SelectMany(pds => pds.Periods
                                                                                                                                        .GroupBy(pg => new { pg.Id })
                                                                                                                                        .Select(pgS => new
                                                                                                                                        {
                                                                                                                                            ld.Fundline,
                                                                                                                                            pgS.Key.Id,
                                                                                                                                            Value = pgS.Select(r => r.Value).Sum()//Sum of attributes
                                                                                                                                        }
                                                                                                                                               )
                                                                                                                        )
                                                                                                    )
                                                                                                    .GroupBy(ldg => new { ldg.Fundline, ldg.Id })
                                                                                                    .Select
                                                                                                    (ldgS => new
                                                                                                    {
                                                                                                        ldgS.Key.Fundline,
                                                                                                        ldgS.Key.Id,
                                                                                                        Value = ldgS.Select(ldgsV => ldgsV.Value).Sum()//Sum at Learning Delivery
                                                                                                    }
                                                                                                   )
                                                                                                   .Join(fundlingLineTypes, ld => ld.Fundline, ft => ft.Fundline, (LD, FT) => new { LD, FT })
                                                                                                   .Select(res =>
                                                                                                       new
                                                                                                       {

                                                                                                           FSPCode = res.FT.PeriodCode,
                                                                                                           DLC = res.FT.DeliverableLineCode,
                                                                                                           Period = res.LD.Id,
                                                                                                           ActualValue = res.LD.Value,
                                                                                                       }
                                                                                                   )
                                                                        }
                                                                );

            var result = periodisedValuesSummationQuery.ToList();
            */

            return null;

        }

       



        public IEnumerable<SummarisedActual> SummariseByFundLine(IList<LearningDelivery> learningDelivery, FundingType fundingType, HashSet<string> attribute)
        {
            var fundlingLineTypes = fundingType.FundingStreams
                .SelectMany(fs => fs.FundLines
                .Select(fl => new
                {
                    fl.Fundline,
                    fs.DeliverableLineCode,
                    fs.PeriodCode
                }));

            var learningDeliveryByFundline = learningDelivery.GroupBy(grp => grp.Fundline);

            List<SummarisedActual> returnActuals = new List<SummarisedActual>();

            foreach(var groupedDeliveries in learningDeliveryByFundline)
            {
                var periodisedData = groupedDeliveries.SelectMany(x => x.PeriodisedData);
                returnActuals.AddRange(SummariseByAttribute(periodisedData, attribute));
            }

            return returnActuals;
        }

        public IEnumerable<SummarisedActual> SummariseByAttribute(IEnumerable<PeriodisedData> periodisedData, HashSet<string> attribute)
        {
            var periodisedDataByAttribute = periodisedData.Where(pd => attribute.Contains(pd.AttributeName));

            var periodList = periodisedDataByAttribute.SelectMany(pd => pd.Periods);

            return SummariseByPeriod(periodList);
        }

        public IEnumerable<SummarisedActual> SummariseByPeriod(IEnumerable<Period> periods)
        {
            return periods.GroupBy(grp => grp.PeriodId).Select(pgS => new SummarisedActual
            {
                Period = pgS.Key,
                ActualValue = pgS.Sum(sv => sv.Value).Value
            });
        }

        public IEnumerable<SummarisedActual> SummariseByPeriosedData(PeriodisedData periodisedData)
        {
            var result = periodisedData.Periods.GroupBy(grp => grp.PeriodId).Select(pgS => new SummarisedActual
            {
                Period = pgS.Key,
                ActualValue = pgS.Sum(sv => sv.Value).Value
            });

            return result;
        }
    }
}
