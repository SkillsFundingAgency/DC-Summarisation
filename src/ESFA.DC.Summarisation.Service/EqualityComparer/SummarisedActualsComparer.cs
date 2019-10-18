using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.Summarisation.Data.Output.Model;

namespace ESFA.DC.Summarisation.Service.EqualityComparer
{
    public class SummarisedActualsComparer : IEqualityComparer<SummarisedActual>
    {
        public bool Equals(SummarisedActual x, SummarisedActual y)
        {
            return y != null && x != null 
                && x.DeliverableCode == y.DeliverableCode
                && x.Period == y.Period
                && x.OrganisationId == y.OrganisationId
                && x.FundingStreamPeriodCode == y.FundingStreamPeriodCode
                && x.UoPCode == y.UoPCode;
        }

        public int GetHashCode(SummarisedActual obj)
        {
            // Check whether the object is null. 
            if (Object.ReferenceEquals(obj, null))
                return 0;

            return (obj.OrganisationId == null ? 0 : obj.OrganisationId.GetHashCode()) ^
                   (obj.FundingStreamPeriodCode == null ? 0 : obj.FundingStreamPeriodCode.GetHashCode()) ^
                   obj.DeliverableCode.GetHashCode() ^
                   obj.Period.GetHashCode() ^
                   (obj.UoPCode == null ? 0 : obj.UoPCode.GetHashCode());
        }
    }
}
