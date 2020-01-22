using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Model;
using ESFA.DC.Summarisation.Model.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Data.Repository
{
    public class SummarisedActualsProcessRepository : IExistingSummarisedActualsRepository
    {
        private readonly Func<ISummarisationContext> _summarisationContext;

        public SummarisedActualsProcessRepository(Func<ISummarisationContext> summarisationContext)
        {
            _summarisationContext = summarisationContext;
        }

        public async Task<CollectionReturn> GetLastCollectionReturnForCollectionTypeAsync(string collectionType, CancellationToken cancellationToken)
        {
            using (var contextFactory = _summarisationContext())
            {
                return await contextFactory.CollectionReturns
                                    .Where(cr => cr.CollectionType == collectionType)
                                    .OrderByDescending(o => o.Id)
                                    .Select(s => new CollectionReturn { Id = s.Id, CollectionReturnCode = s.CollectionReturnCode, CollectionType = s.CollectionType })
                                    .FirstOrDefaultAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<Service.Model.SummarisedActual>> GetSummarisedActualsForCollectionReturnAndOrganisationAsync(int collectionReturnId, string organisationId, CancellationToken cancellationToken)
        {
            using (var contextFactory = _summarisationContext())
            {
                return await contextFactory.SummarisedActuals
                .Where(sa => sa.CollectionReturnId == collectionReturnId && sa.OrganisationId == organisationId)
                .Select(o => new Service.Model.SummarisedActual
                {
                    Period = o.Period,
                    FundingStreamPeriodCode = o.FundingStreamPeriodCode,
                    UoPCode = o.UoPCode,
                    DeliverableCode = o.DeliverableCode,
                    OrganisationId = o.OrganisationId,
                    ActualValue = o.ActualValue,
                    ActualVolume = o.ActualVolume,
                    ContractAllocationNumber = o.ContractAllocationNumber,
                    PeriodTypeCode = o.PeriodTypeCode
                }).ToListAsync(cancellationToken);
            }
        }
    }
}
