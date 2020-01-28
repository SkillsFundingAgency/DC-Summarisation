using ESFA.DC.Summarisation.Main.Interfaces;
using ESFA.DC.Summarisation.Main.Model;
using ESFA.DC.Summarisation.Service.Model.Config;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Main.Service
{
    public class ProviderContractsService : IProviderContractsService
    {
        public async Task<ProviderFundingStreamsAllocations> GetProviderContracts(int UKPRN, ICollection<FundingStream> fundingStreams, ICollection<FcsContractAllocation> contractAllocations, CancellationToken cancellationToken)
        {
            var providerFundingStreams = new List<FundingStream>();
            var allocations = new List<FcsContractAllocation>();

            var contractAllocationsDictionary = contractAllocations
                .GroupBy(ca => ca.FundingStreamPeriodCode)
                .ToDictionary(
                    gca => gca.Key,
                    gca => gca.ToList(),
                    StringComparer.OrdinalIgnoreCase);

            foreach (var fs in fundingStreams)
            {
                if (contractAllocationsDictionary.ContainsKey(fs.PeriodCode) && contractAllocationsDictionary[fs.PeriodCode].Any(x => x.DeliveryUkprn == UKPRN))
                {
                    providerFundingStreams.Add(fs);

                    foreach (var allocation in contractAllocationsDictionary[fs.PeriodCode].Where(x => x.DeliveryUkprn == UKPRN))
                    {
                        if (!allocations.Any(
                            w => w.ContractAllocationNumber.Equals(allocation.ContractAllocationNumber, StringComparison.OrdinalIgnoreCase)
                                && w.FundingStreamPeriodCode.Equals(fs.PeriodCode, StringComparison.OrdinalIgnoreCase)))
                        {
                            allocations.Add(allocation);
                        }
                    }
                }
            }

            return new ProviderFundingStreamsAllocations() { FcsContractAllocations = allocations, FundingStreams = providerFundingStreams };
        }
    }
}
