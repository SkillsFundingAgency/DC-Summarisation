using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ESF.Database.EF.Interfaces;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.ESF.Interfaces;
using ESFA.DC.Summarisation.ESF.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.ESF.ESF.Service.Providers
{
    public class ESFProvider_R1 : AbstractLearningProviderProvider, ISummarisationInputDataProvider
    {
        private readonly Func<IESF_DataStoreEntities> _esf;
        private Func<IESF_DataStoreEntities> esf;

        public string CollectionType => CollectionTypeConstants.ESF;

        public ESFProvider_R1(Func<IESF_DataStoreEntities> esf)
        {
            _esf = esf;
        }

        public async Task<ICollection<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            var ukprnString = ukprn.ToString();

            using (var esfContext = _esf())
            {
                var esfData = await esfContext.SupplementaryDatas
                                .Where(sd => sd.SourceFile.UKPRN == ukprnString)
                                 .Select(obj => new
                                 {
                                     ConRefNumber = obj.ConRefNumber.Trim(),
                                     DeliverableCode = obj.DeliverableCode,
                                     CalendarYear = obj.CalendarYear,
                                     CalendarMonth = obj.CalendarMonth,
                                     CostType = obj.CostType,
                                     Value = obj.Value,
                                     UnitCostValue = obj.SupplementaryDataUnitCost.Value
                                 }).ToListAsync(cancellationToken);

                var preSummarised = esfData
                                .GroupBy(g => new { ConRefNumber = g.ConRefNumber, g.DeliverableCode, g.CalendarYear, g.CalendarMonth })
                                .Select(obj => new 
                                {
                                    ConRefNumber = obj.Key.ConRefNumber,
                                    DeliverableCode = obj.Key.DeliverableCode,
                                    CalendarYear = obj.Key.CalendarYear,
                                    CalendarMonth = obj.Key.CalendarMonth,
                                    Value = obj.Sum(p => p.CostType.Equals(ESFConstants.UnitCost, StringComparison.OrdinalIgnoreCase) ? (p.UnitCostValue ?? p.Value) : p.CostType.Equals(ESFConstants.UnitCostDeduction, StringComparison.OrdinalIgnoreCase) ? (p.UnitCostValue ?? p.Value) : p.Value),
                                    Volume = obj.Sum(p => p.CostType.Equals(ESFConstants.UnitCost, StringComparison.OrdinalIgnoreCase) ? 1 : p.CostType.Equals(ESFConstants.UnitCostDeduction, StringComparison.OrdinalIgnoreCase) ? -1 : 0),
                                }).ToList();


                var learningDeliveries = preSummarised
                                .GroupBy(sd => sd.ConRefNumber, StringComparer.OrdinalIgnoreCase)
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

                return learningDeliveries;
            }
        }

        public async Task<ICollection<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var esfContext = _esf())
            {
                return await esfContext.SupplementaryDatas.Select(s => Convert.ToInt32(s.SourceFile.UKPRN)).Distinct().ToListAsync(cancellationToken);
            }
            
        }

        public Task<ICollection<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken) => ProvideAsync(ukprn, cancellationToken);
    }
}
