using ESFA.DC.NCS.EF.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.NCS.Interfaces;
using ESFA.DC.Summarisation.NCS.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.NCS.Service
{
    public class NCSProvider : ISummarisationInputDataProvider
    {
        private readonly Func<INcsContext> _ncsContext;

        public NCSProvider(Func<INcsContext> ncsContext)
        {
            _ncsContext = ncsContext;
        }

        public async Task<ICollection<FundingValue>> ProvideAsync(TouchpointProvider touchpointProvider, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var maxDate = new DateTime(summarisationMessage.CollectionYear % 100 + 2000, 3, 31);

            using (var ncsContext = _ncsContext())
            {
                var fundingValues = await ncsContext.FundingValues
                        .Where(fv => fv.Ukprn == touchpointProvider.UKPRN 
                                    && fv.TouchpointId == touchpointProvider.TouchpointId 
                                    && fv.CollectionYear == summarisationMessage.CollectionYear
                                    && fv.OutcomeEffectiveDate <= maxDate)
                        .Select(s => new FundingValue
                        {
                            CollectionYear = s.CollectionYear,
                            CalendarMonth = s.OutcomeEffectiveDateMonth,
                            OutcomeType = s.OutcomeType,
                            Value = s.Value
                        }).ToListAsync(cancellationToken);
                        

                return fundingValues;
            }
        }

        public async Task<ICollection<TouchpointProvider>> ProvideUkprnsAsync(int collectionYear, CancellationToken cancellationToken)
        {
            using (var ncsContext = _ncsContext())
            {
                return await ncsContext.FundingValues
                    .Where(fv => fv.CollectionYear == collectionYear)
                    .Select(l => new TouchpointProvider { UKPRN = l.Ukprn, TouchpointId = l.TouchpointId }).Distinct().ToListAsync(cancellationToken);
            }
        }
    }
}