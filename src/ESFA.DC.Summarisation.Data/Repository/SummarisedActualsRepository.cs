using ESFA.DC.Summarisation.Data.DTO;
using ESFA.DC.Summarisation.Data.Output.Model;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Model.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Data.Repository
{
    public class SummarisedActualsRepository : ISummarisedActualsRepository
    {
        private readonly ISummarisationContext _summarisationContext;

        public SummarisedActualsRepository(ISummarisationContext summarisationContext)
        {
            _summarisationContext = summarisationContext;
        }

        public Task<CollectionReturnDto> GetCollectionReturnFor(string collectionType, string collectionReturn, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CollectionReturnSummaryDto>> GetCollectionReturnSummariesForAsync(DateTime? collectionsClosedSince, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CollectionReturnSummaryDto>> GetCollectionReturnSummariesForAsync(string collectionType, DateTime? collectionsClosedSince, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SummarisedActual>> GetLatestSummarisedActualsAsync(string collectionType, CancellationToken cancellationToken)
        {
            int CollectionReturnId = _summarisationContext.CollectionReturns.Where(cr => cr.CollectionType == collectionType).OrderByDescending(o => o.Id).Select(s => s.Id).FirstOrDefault();

            return await _summarisationContext.SummarisedActuals
                             .Where(sa => sa.CollectionReturnId == CollectionReturnId)
                             .Select(o => new SummarisedActual
                             {
                                    OrganisationId = o.OrganisationId,
                                    UoPCode = o.UoPcode,
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
