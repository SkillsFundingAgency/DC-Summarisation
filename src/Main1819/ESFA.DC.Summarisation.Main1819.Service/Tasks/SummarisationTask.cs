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

namespace ESFA.DC.Summarisation.Main1819.Service.Tasks
{
    public class SummarisationTask
    {
        private readonly List<Provider> _providers;

        private readonly List<string> _attributes;

        private readonly List<FundingType> _fundingTypes;

        private readonly string _key ;

        public SummarisationTask(string key, List<Provider> providers,
            List<string> attributes,
            List<FundingType> fundingTypes)
        {
            _key = key;
            _providers = providers;
            _attributes = attributes;
            _fundingTypes = fundingTypes;
        }

        public string TaskName => "Summarisation";

        public void ExecuteAsync()
        {
            var fundlingLineTypes = _fundingTypes.Where(f => f.Key == _key).SelectMany(ft => ft.FundingStreams
                                                          .SelectMany(fs => fs.FundLines
                                                                                .Select(fl => new
                                                                                {
                                                                                    fl.Fundline,
                                                                                    fs.DeliverableLineCode,
                                                                                    fs.PeriodCode
                                                                                }
                                                                                           )
                                                                    )
                                            );
            var periodisedValuesSummationQuery = _providers.Select(pr =>
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


        }
      
    }
}
