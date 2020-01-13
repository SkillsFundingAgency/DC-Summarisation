using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Interfaces;
using System;
using System.Collections.Generic;
using ESFA.DC.NCS.EF.Interfaces;
using System.Threading;
using ESFA.DC.Summarisation.Data.Input.Model;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.NCS1920.Data
{
    public class NcsProvider : ILearningDeliveryProvider
    {
        private readonly Func<INcsContext> _ncsContext;

        public string SummarisationType => SummarisationTypeConstants.NCS1920;

        public string CollectionType => CollectionTypeConstants.NCS;

        public NcsProvider(Func<INcsContext> ncsContext)
        {
            _ncsContext = ncsContext;
        }

        public async Task<IList<LearningDelivery>> ProvideAsync(int ukprn, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            using (var ncsContext = _ncsContext())
            {
                return await ncsContext.FundingValues
                        .Where(sv => sv.Ukprn == ukprn)
                         .GroupBy(x => x.TouchpointId)
                        .Select(ld => new LearningDelivery
                        {
                            LearnRefNumber = ld.Key,
                            PeriodisedData = ld.Select(pd => new PeriodisedData
                            {
                                AttributeName = pd.OutcomeId.ToString(),
                                Periods = new List<Period>
                                {
                                    new Period
                                    {
                                        CollectionMonth = Convert.ToInt16(pd.Period),
                                        CollectionYear = pd.CollectionYear,
                                        Value = pd.Value
                                    }
                                }
                            }).ToList()
                        }).ToListAsync(cancellationToken);
            }
        }

        public async Task<IList<int>> ProvideUkprnsAsync(CancellationToken cancellationToken)
        {
            using (var ncsContext = _ncsContext())
            {
                return await ncsContext.FundingValues.Select(l => Convert.ToInt32(l.Ukprn)).Distinct().ToListAsync(cancellationToken);
            }
        }
    }
}