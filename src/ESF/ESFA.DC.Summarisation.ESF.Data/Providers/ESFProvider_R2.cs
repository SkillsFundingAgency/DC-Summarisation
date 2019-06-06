using ESFA.DC.ESF.R2.Database.EF.Interfaces;
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
        private readonly IESFR2Context _esf;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.ESF_SuppData);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.ESF);

        public ESFProvider_R2(IESFR2Context esf)
        {
            _esf = esf;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            return await _esf.SupplementaryDatas
                                        .Join(_esf.SourceFiles,
                                                    sd => sd.SourceFileId,
                                                    sf => sf.SourceFileId,
                                                    (sd, sf) => new { sf.Ukprn, sd.ConRefNumber, sd.DeliverableCode, sd.CalendarYear, sd.CalendarMonth, sd.CostType, sd.ReferenceType, sd.Reference, sd.Value })
                                        .GroupJoin(_esf.SupplementaryDataUnitCosts,
                                               sd => new { sd.ConRefNumber, sd.DeliverableCode, sd.CalendarYear, sd.CalendarMonth, sd.CostType, sd.ReferenceType, sd.Reference },
                                               sdu => new { sdu.ConRefNumber, sdu.DeliverableCode, sdu.CalendarYear, sdu.CalendarMonth, sdu.CostType, sdu.ReferenceType, sdu.Reference },
                                               (sd, sdu) => new { sd.Ukprn, sd.ConRefNumber, sd.DeliverableCode, sd.CalendarYear, sd.CalendarMonth, sd.CostType, sd.ReferenceType, sd.Reference, sd.Value, sdu })
                                        .SelectMany(z => z.sdu.DefaultIfEmpty(), (sd, sdu) => new { sd.Ukprn, sd.ConRefNumber, sd.DeliverableCode, sd.CalendarYear, sd.CalendarMonth, sd.CostType, sd.ReferenceType, sd.Reference, sd.Value, SDUnitCostValue = sdu != null ? sdu.Value : null})
                                        .Where(w => w.Ukprn == ukprn.ToString())
                                        .GroupBy(g => g.ConRefNumber)
                                        .Select(ld => new LearningDelivery
                                        {
                                            LearnRefNumber = "",
                                            AimSeqNumber = 0,
                                            ConRefNumber = ld.Key,
                                            PeriodisedData = ld.GroupBy(x => x.DeliverableCode).Select(pd => new PeriodisedData
                                            {
                                                DeliverableCode = pd.Key,
                                                AttributeName = "",
                                                Periods = pd.Select(p => new Period
                                                {
                                                    CalendarMonth = p.CalendarMonth,
                                                    CalendarYear = p.CalendarYear,
                                                    Value = p.SDUnitCostValue ?? p.Value,
                                                    Volume = p.CostType == "Unit Cost" ? 1 : p.CostType == "Unit Cost Deduction" ? -1 : 0
                                                }).ToList()
                                            }).ToList()
                                        }).ToListAsync(cancellationToken);

        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            return await _esf.SupplementaryDatas
                .Join(_esf.SourceFiles, sd => sd.SourceFileId, sf => sf.SourceFileId, (sd, sf) => new { sf.Ukprn})
                .Select(s => Convert.ToInt32(s.Ukprn)).Distinct().ToListAsync(cancellationToken);

        }

    }
}
