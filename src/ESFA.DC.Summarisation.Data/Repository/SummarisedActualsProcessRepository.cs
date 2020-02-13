using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Model;
using ESFA.DC.Summarisation.Model.Interface;
using ESFA.DC.Summarisation.Service.Model;
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

        public async Task<Service.Model.CollectionReturn> GetLastCollectionReturnForCollectionTypeAsync(string collectionType, string collectionReturnCode, CancellationToken cancellationToken)
        {
            using (var contextFactory = _summarisationContext())
            {
                return await contextFactory.CollectionReturns
                                    .Where(cr => cr.CollectionType == collectionType && cr.CollectionReturnCode != collectionReturnCode)
                                    .OrderByDescending(o => o.Id)
                                    .Select(s => new Service.Model.CollectionReturn { Id = s.Id, CollectionReturnCode = s.CollectionReturnCode, CollectionType = s.CollectionType })
                                    .FirstOrDefaultAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<Service.Model.SummarisedActual>> GetSummarisedActualsAsync(int collectionReturnId, string organisationId, CancellationToken cancellationToken)
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

        public async Task<IEnumerable<Service.Model.SummarisedActual>> GetSummarisedActualsAsync(int collectionReturnId, string organisationId, string uopCode, CancellationToken cancellationToken)
        {
            using (var contextFactory = _summarisationContext())
            {
                return await contextFactory.SummarisedActuals
                .Where(sa => sa.CollectionReturnId == collectionReturnId && sa.OrganisationId == organisationId && sa.UoPCode == uopCode)
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
