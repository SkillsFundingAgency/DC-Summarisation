using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.GenericCollection.EF.Interface;
using ESFA.DC.Summarisation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Data.Repository
{
    public class GenericCollectionRepository : IGenericCollectionRepository
    {
        private readonly Func<IGenericCollectionContext> _genericCollection;

        public GenericCollectionRepository(Func<IGenericCollectionContext> genericCollection)
        {
            _genericCollection = genericCollection;
        }

        public async Task<ICollection<Service.Model.SummarisedActual>> RetrieveAsync(string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _genericCollection())
            {
                var summarisedActuals = await context.SummarisedActuals
                    .Where(w => 
                    string.Equals(w.CollectionType, collectionType, StringComparison.OrdinalIgnoreCase))
                    .ToListAsync(cancellationToken);

                return summarisedActuals
                    .Select(sa => new Service.Model.SummarisedActual
                    {
                        ContractAllocationNumber = sa.ContractAllocationNumber,
                        FundingStreamPeriodCode = sa.FundingStreamPeriodCode,
                        OrganisationId = sa.OrganisationId,
                        PeriodTypeCode = sa.PeriodTypeCode,
                        Period = sa.Period,
                        DeliverableCode = sa.DeliverableCode,
                        ActualValue = sa.ActualValue,
                        ActualVolume = sa.ActualVolume,
                    }).ToList();
            }
        }
    }
}
