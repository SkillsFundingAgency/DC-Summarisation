using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Summarisation.Data.Repository
{
    public class FcsRepository : IFcsRepository
    {
        private readonly Func<IFcsContext> _fcs;

        public FcsRepository(Func<IFcsContext> fcs)
        {
            _fcs = fcs;
        }

        public async Task<ICollection<FcsContractAllocation>> RetrieveContractAllocationsAsync(IEnumerable<string> fundingStreamPeriodCodes,  CancellationToken cancellationToken)
        {
            var fspCodes = fundingStreamPeriodCodes.ToList();

            using (var fcsContext = _fcs())
            {
                var contractAllocations = await fcsContext.ContractAllocations
                    .Where(w => fspCodes.Contains(w.FundingStreamPeriodCode))
                    .Select(ca => new
                    {
                        ca.ContractAllocationNumber,
                        ca.FundingStreamPeriodCode,
                        ca.UoPcode,
                        ca.DeliveryUkprn,
                        ca.DeliveryOrganisation,
                        ca.StartDate,
                        ca.EndDate,
                    }).ToListAsync(cancellationToken);

                return contractAllocations
                    .Select(ca => new FcsContractAllocation
                    {
                        ContractAllocationNumber = ca.ContractAllocationNumber,
                        FundingStreamPeriodCode = ca.FundingStreamPeriodCode,
                        UoPcode = ca.UoPcode,
                        DeliveryUkprn = ca.DeliveryUkprn,
                        DeliveryOrganisation = ca.DeliveryOrganisation,
                        ContactStartDate = BuildFormattedDate(ca.StartDate),
                        ContractEndDate = BuildFormattedDate(ca.EndDate),
                        ActualsSchemaPeriodStart = BuildActualSchemaPeriod(ca.StartDate),
                        ActualsSchemaPeriodEnd = BuildActualSchemaPeriod(ca.EndDate),
                    }).ToList();
            }
        }

        private int BuildFormattedDate(DateTime? dateTime)
        {
            return dateTime.HasValue ? Convert.ToInt32(dateTime.Value.ToString("yyyyMMdd")) : 0;
        }

        private int BuildActualSchemaPeriod(DateTime? dateTime)
        {
            return dateTime.HasValue ? Convert.ToInt32(dateTime.Value.ToString("yyyyMM")) : 0;
        }
    }
}
