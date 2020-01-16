using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Service
{
    public class ProviderContractsService : IProviderContractsService
    {
        public async Task<IProviderFundingStreamsAllocations> GetProviderContracts(int UKPRN, ICollection<FundingStream> fundingStreams, IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> contractAllocations, CancellationToken cancellationToken)
        {
            var providerFundingStreams = new List<FundingStream>();
            var allocations = new List<IFcsContractAllocation>();

            foreach (var fs in fundingStreams)
            {
                if (contractAllocations.ContainsKey(fs.PeriodCode) && contractAllocations[fs.PeriodCode].Any(x => x.DeliveryUkprn == UKPRN))
                {
                    providerFundingStreams.Add(fs);

                    foreach (var allocation in contractAllocations[fs.PeriodCode].Where(x => x.DeliveryUkprn == UKPRN))
                    {
                        if (!allocations.Any(
                            w => w.ContractAllocationNumber.Equals(allocation.ContractAllocationNumber, StringComparison.OrdinalIgnoreCase)
                                && w.FundingStreamPeriodCode.Equals(fs.PeriodCode, StringComparison.OrdinalIgnoreCase)))
                            allocations.Add(allocation);
                    }
                }
            }
            
            return  new ProviderFundingStreamsAllocations() { FcsContractAllocations =  allocations, FundingStreams = providerFundingStreams };
        }
    }
}
