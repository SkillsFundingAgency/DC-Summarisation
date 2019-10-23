using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.External.FCS.Model;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Data.Population.Service
{
    public class FcsRepository : IFcsRepository
    {
        private readonly Func<IFcsContext> _fcs;

        public FcsRepository(Func<IFcsContext> fcs)
        {
            _fcs = fcs;
        }

        public async Task<IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>>> RetrieveAsync(CancellationToken cancellationToken)
        {
            using (var fcsContext = _fcs())
            {
                return await fcsContext.ContractAllocations
                    .Where(w => FundingStreamConstants.FundingStreams.Contains(w.FundingStreamPeriodCode))
                    .Select(ca => new FcsContractAllocation
                    {
                        ContractAllocationNumber = ca.ContractAllocationNumber,
                        FundingStreamPeriodCode = ca.FundingStreamPeriodCode,
                        UoPcode = ca.UoPcode,
                        DeliveryUkprn = ca.DeliveryUkprn,
                        DeliveryOrganisation = ca.DeliveryOrganisation,
                        ContractStartDate = ca.StartDate.HasValue ? Convert.ToInt32(ca.StartDate.Value.ToString("yyyyMM")) : 0,
                        ContractEndDate = ca.EndDate.HasValue ? Convert.ToInt32(ca.EndDate.Value.ToString("yyyyMM")) : 0
                    })
                    .GroupBy(ca => ca.FundingStreamPeriodCode)
                    .ToDictionaryAsync(
                        gca => gca.Key,
                        gca => gca.ToList() as IReadOnlyCollection<IFcsContractAllocation>,
                        StringComparer.OrdinalIgnoreCase,
                        cancellationToken);
            }
        }
    }
}
