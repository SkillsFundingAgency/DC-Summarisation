using ESFA.DC.ESF.R2.Database.EF.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.ESF.Data.Providers
{
    public class ESFProvider_R2 : ILearningDeliveryProvider
    {
        private readonly Func<IESFR2Context> _esf;

        public string SummarisationType => ConstantKeys.SummarisationType_ESF_SuppData;

        public string CollectionType => ConstantKeys.CollectionType_ESF;

        public ESFProvider_R2(Func<IESFR2Context> esf)
        {
            _esf = esf;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            using (var esfr2Context = _esf())
            {

                var preSummarised = await esfr2Context.SupplementaryDatas
                                .Where(sd => sd.SourceFile.Ukprn == ukprn.ToString())
                                .GroupBy(g => new { ConRefNumber = g.ConRefNumber.Trim(), g.DeliverableCode, g.CalendarYear, g.CalendarMonth })
                                .Select(obj => new
                                {
                                    ConRefNumber = obj.Key.ConRefNumber,
                                    DeliverableCode = obj.Key.DeliverableCode,
                                    CalendarYear = obj.Key.CalendarYear,
                                    CalendarMonth = obj.Key.CalendarMonth,
                                    Value = obj.Sum(p => p.CostType.Equals(ConstantKeys.UnitCost, StringComparison.OrdinalIgnoreCase) ? (p.SupplementaryDataUnitCost.Value ?? p.Value) : p.CostType.Equals(ConstantKeys.UnitCostDeduction, StringComparison.OrdinalIgnoreCase) ? (p.SupplementaryDataUnitCost.Value ?? p.Value) * -1 : p.Value),
                                    Volume = obj.Sum(p => p.CostType.Equals(ConstantKeys.UnitCost, StringComparison.OrdinalIgnoreCase) ? 1 : p.CostType.Equals(ConstantKeys.UnitCostDeduction, StringComparison.OrdinalIgnoreCase) ? -1 : 0),
                                }).ToListAsync(cancellationToken);

                return  preSummarised
                        .GroupBy(sd => sd.ConRefNumber)
                        .Select(ld => new LearningDelivery
                        {
                            ConRefNumber = ld.Key,
                            PeriodisedData = ld.GroupBy(x => x.DeliverableCode).Select(pd => new PeriodisedData
                            {
                                DeliverableCode = pd.Key,
                                AttributeName = "FakeAttribute",
                                Periods = pd.Select(p => new Period
                                {
                                    CalendarMonth = p.CalendarMonth,
                                    CalendarYear = p.CalendarYear,
                                    Value = p.Value,
                                    Volume = p.Volume
                                }).ToList()
                            }).ToList()
                        }).ToList();
            }
        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var esfr2Context = _esf())
            {
                return await esfr2Context.SupplementaryDatas
                    .Join(esfr2Context.SourceFiles, sd => sd.SourceFileId, sf => sf.SourceFileId, (sd, sf) => new { sf.Ukprn })
                    .Select(s => Convert.ToInt32(s.Ukprn)).Distinct().ToListAsync(cancellationToken);
            }
        }

        public Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken) => ProvideAsync(ukprn, cancellationToken);

    }
}
