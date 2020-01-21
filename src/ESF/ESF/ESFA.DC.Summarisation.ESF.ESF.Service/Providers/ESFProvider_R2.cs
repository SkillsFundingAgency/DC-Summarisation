using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ESF.R2.Database.EF.Interfaces;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.ESF.ESF.Service.Providers
{
    public class ESFProvider_R2 : AbstractLearningProviderProvider, ISummarisationInputDataProvider<ILearningProvider>
    {
        private readonly Func<IESFR2Context> _esf;

        public string CollectionType => CollectionTypeConstants.ESF;

        public ESFProvider_R2(Func<IESFR2Context> esf)
        {
            _esf = esf;
        }

        public async Task<ILearningProvider> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            using (var esfr2Context = _esf())
            {
                var esfData = await esfr2Context.SupplementaryDatas
                                .Where(sd => sd.SourceFile.Ukprn == ukprn.ToString())
                                 .Select(obj => new
                                 {
                                     ConRefNumber = obj.ConRefNumber,
                                     DeliverableCode = obj.DeliverableCode,
                                     CalendarYear = obj.CalendarYear,
                                     CalendarMonth = obj.CalendarMonth,
                                     CostType = obj.CostType,
                                     Value = obj.Value,
                                     UnitCostValue = obj.SupplementaryDataUnitCost.Value,
                                 }).ToListAsync(cancellationToken);

                var preSummarised = esfData
                                .GroupBy(g => new { ConRefNumber = g.ConRefNumber.Trim(), g.DeliverableCode, g.CalendarYear, g.CalendarMonth })
                                .Select(obj => new
                                {
                                    ConRefNumber = obj.Key.ConRefNumber,
                                    DeliverableCode = obj.Key.DeliverableCode,
                                    CalendarYear = obj.Key.CalendarYear,
                                    CalendarMonth = obj.Key.CalendarMonth,
                                    Value = obj.Sum(p => p.CostType.Equals(ESFConstants.UnitCost, StringComparison.OrdinalIgnoreCase) ? (p.UnitCostValue ?? p.Value) : p.CostType.Equals(ESFConstants.UnitCostDeduction, StringComparison.OrdinalIgnoreCase) ? (p.UnitCostValue ?? p.Value) * -1 : p.Value),
                                    Volume = obj.Sum(p => p.CostType.Equals(ESFConstants.UnitCost, StringComparison.OrdinalIgnoreCase) ? 1 : p.CostType.Equals(ESFConstants.UnitCostDeduction, StringComparison.OrdinalIgnoreCase) ? -1 : 0),
                                }).ToList();

                var learningDeliveries =  preSummarised
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
                                } as IPeriod).ToList()
                            }).ToList()
                        }).ToList();

                return BuildLearningProvider(ukprn, learningDeliveries);
            }
        }

        public async Task<ICollection<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var esfr2Context = _esf())
            {
                return await esfr2Context.SupplementaryDatas
                    .Join(esfr2Context.SourceFiles, sd => sd.SourceFileId, sf => sf.SourceFileId, (sd, sf) => new { sf.Ukprn })
                    .Select(s => Convert.ToInt32(s.Ukprn)).Distinct().ToListAsync(cancellationToken);
            }
        }

        public Task<ILearningProvider> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken) => ProvideAsync(ukprn, cancellationToken);

    }
}
