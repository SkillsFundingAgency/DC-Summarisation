using ESFA.DC.ESF.Database.EF.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.ESF.Data.Providers
{
    public class ESFProvider_R1 : ILearningDeliveryProvider
    {
        private readonly Func<IESF_DataStoreEntities> _esf;

        public string SummarisationType => nameof(Configuration.Enum.SummarisationType.ESF_SuppData);

        public string CollectionType => nameof(Configuration.Enum.CollectionType.ESF);

        public ESFProvider_R1(Func<IESF_DataStoreEntities> esf)
        {
            _esf = esf;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, CancellationToken cancellationToken)
        {
            var ukprnString = ukprn.ToString();

            using (var esfContext = _esf())
            {
                var preSummarised = await esfContext.SupplementaryDatas
                                .Where(sd => sd.SourceFile.UKPRN == ukprnString)
                                .GroupBy(g => new { ConRefNumber = g.ConRefNumber.Trim(), g.DeliverableCode, g.CalendarYear, g.CalendarMonth })
                                .Select(obj => new 
                                {
                                    ConRefNumber = obj.Key.ConRefNumber,
                                    DeliverableCode = obj.Key.DeliverableCode,
                                    CalendarYear = obj.Key.CalendarYear,
                                    CalendarMonth = obj.Key.CalendarMonth,
                                    Value = obj.Sum(p => p.CostType.Equals(SummarisationConstants.UnitCost, StringComparison.OrdinalIgnoreCase) ? (p.SupplementaryDataUnitCost.Value ?? p.Value) : p.CostType.Equals(SummarisationConstants.UnitCostDeduction, StringComparison.OrdinalIgnoreCase) ? (p.SupplementaryDataUnitCost.Value ?? p.Value) : p.Value),
                                    Volume = obj.Sum(p => p.CostType.Equals(SummarisationConstants.UnitCost, StringComparison.OrdinalIgnoreCase) ? 1 : p.CostType.Equals(SummarisationConstants.UnitCostDeduction, StringComparison.OrdinalIgnoreCase) ? -1 : 0),
                                }).ToListAsync(cancellationToken);


                return preSummarised
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
            }
        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var esfContext = _esf())
            {
                return await esfContext.SupplementaryDatas.Select(s => Convert.ToInt32(s.SourceFile.UKPRN)).Distinct().ToListAsync(cancellationToken);
            }
            
        }

        public Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken) => ProvideAsync(ukprn, cancellationToken);
    }
}
