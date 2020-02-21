using System.Collections.Generic;
using ESFA.DC.Summarisation.Data.BAU.Persist.Interfaces;
using ESFA.DC.Summarisation.Data.BAU.Persist.Model;
using ESFA.DC.Summarisation.Service.Model;
using System.Linq;

namespace ESFA.DC.Summarisation.Data.BAU.Persist.Mapper
{
    public class SummarisedActualBAUMapper : ISummarisedActualBAUMapper
    {
        public ICollection<SummarisedActualBAU> Map(IEnumerable<SummarisedActual> summarisedActuals, string collectionType, string collectionReturnCode)
        {
           return summarisedActuals.Select(x => new SummarisedActualBAU
            {
                CollectionReturnCode = collectionReturnCode,
                OrganisationId = x.OrganisationId,
                PeriodTypeCode = x.PeriodTypeCode,
                Period = x.Period,
                FundingStreamPeriodCode = x.FundingStreamPeriodCode,
                CollectionType = collectionType,
                ContractAllocationNumber = x.ContractAllocationNumber,
                UoPCode = x.UoPCode,
                DeliverableCode = x.DeliverableCode,
                ActualValue = x.ActualValue,
                ActualVolume = x.ActualVolume
            }).ToList();
        }
    }
}
