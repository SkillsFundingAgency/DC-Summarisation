using ESFA.DC.Summarisation.Data.Output.Model;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Model;
using ESFA.DC.Summarisation.Model.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SummarisedActual = ESFA.DC.Summarisation.Data.Output.Model.SummarisedActual;

namespace ESFA.DC.Summarisation.Data.Repository
{
    public class SummarisedActualsProcessRepository : ISummarisedActualsProcessRepository
    {
        private readonly ISummarisationContext _summarisationContext;

        public SummarisedActualsProcessRepository(ISummarisationContext summarisationContext)
        {
            _summarisationContext = summarisationContext;
        }

        public async Task<CollectionReturn> GetLastCollectionReturnForCollectionTypeAsync(string collectionType, CancellationToken cancellationToken)
        {
            return await _summarisationContext.CollectionReturns
                                .Where(cr => cr.CollectionType == collectionType)
                                .OrderByDescending(o => o.Id)
                                .Select(s => new CollectionReturn { Id = s.Id, CollectionReturnCode = s.CollectionReturnCode,  CollectionType = s.CollectionType })
                                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<Output.Model.SummarisedActual>> GetSummarisedActualsForCollectionReturnAndOrganisationAsync(int collectionReturnId, string organisationId, CancellationToken cancellationToken)
        {
            return await this._summarisationContext.SummarisedActuals
                .Where(sa => sa.CollectionReturnId == collectionReturnId && sa.OrganisationId == organisationId)
                .Select(o => new Output.Model.SummarisedActual
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

        public async Task<IEnumerable<Output.Model.SummarisedActual>> GetLatestSummarisedActualsAsync(int collectionReturnId, CancellationToken cancellationToken)
        {
            return await _summarisationContext.SummarisedActuals
                             .Where(sa => sa.CollectionReturnId == collectionReturnId)
                             .Select(o => new Output.Model.SummarisedActual
                             {
                                    OrganisationId = o.OrganisationId,
                                    UoPCode = o.UoPCode,
                                    FundingStreamPeriodCode = o.FundingStreamPeriodCode,
                                    Period = o.Period,
                                    DeliverableCode = o.DeliverableCode,
                                    ActualVolume = o.ActualVolume,
                                    ActualValue = o.ActualValue,
                                    PeriodTypeCode = o.PeriodTypeCode,
                                    ContractAllocationNumber = o.ContractAllocationNumber
                             }).ToListAsync(cancellationToken);
        }
    }
}
