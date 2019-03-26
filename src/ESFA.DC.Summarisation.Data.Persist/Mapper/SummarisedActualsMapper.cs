using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist.Mapper
{
    public class SummarisedActualsMapper : ISummarisedActualsMapper
    {
        public IEnumerable<SummarisedActual> MapSummarisedActuals(IEnumerable<Output.Model.SummarisedActual> actuals, CollectionReturn collectionReturn)
        {
            return actuals.Select(actual => new SummarisedActual
            {
                OrganisationId = actual.OrganisationId,
                UoPcode = actual.UoPcode,
                FundingStreamPeriodCode = actual.FundingStreamPeriodCode,
                Period = actual.Period,
                DeliverableCode = actual.DeliverableCode,
                ActualVolume = actual.ActualVolume,
                ActualValue = actual.ActualValue,
                PeriodTypeCode = actual.PeriodTypeCode,
                ContractAllocationNumber = actual.ContractAllocationNumber,
                CollectionReturnId = collectionReturn.Id
            });
        }

        public IEnumerable<SummarisedActual> MapSummarisedActuals(IEnumerable<Output.Model.SummarisedActual> actuals, int collectionReturnId)
        {
            return actuals.Select(actual => new SummarisedActual
            {
                OrganisationId = actual.OrganisationId,
                UoPcode = actual.UoPcode,
                FundingStreamPeriodCode = actual.FundingStreamPeriodCode,
                Period = actual.Period,
                DeliverableCode = actual.DeliverableCode,
                ActualVolume = actual.ActualVolume,
                ActualValue = actual.ActualValue,
                PeriodTypeCode = actual.PeriodTypeCode,
                ContractAllocationNumber = actual.ContractAllocationNumber,
                CollectionReturnId = collectionReturnId
            });
        }

        public IEnumerable<Output.Model.SummarisedActual> MapSummarisedActualsOutputModel(IEnumerable<Output.Model.SummarisedActual> actuals, int collectionReturnId)
        {
            return actuals.Select(actual => new Output.Model.SummarisedActual
            {
                OrganisationId = actual.OrganisationId,
                UoPcode = actual.UoPcode,
                FundingStreamPeriodCode = actual.FundingStreamPeriodCode,
                Period = actual.Period,
                DeliverableCode = actual.DeliverableCode,
                ActualVolume = actual.ActualVolume,
                ActualValue = actual.ActualValue,
                PeriodTypeCode = actual.PeriodTypeCode,
                ContractAllocationNumber = actual.ContractAllocationNumber,
                CollectionReturnId = collectionReturnId
            });
        }
    }
}