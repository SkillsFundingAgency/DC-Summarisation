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
        private readonly IFcsRepository _fcsRepository;
        private readonly ILogger _logger;
        private readonly IEnumerable<ISummarisationConfigProvider<FundingType>> _fundingTypesProviders;
        private IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> _fcsContractAllocations;
        private bool _isFCSContractsLoaded = false;

        public ProviderContractsService(IFcsRepository fcsRepository,
            IEnumerable<ISummarisationConfigProvider<FundingType>> fundingTypesProviders,
            ILogger logger)
        {
            _fcsRepository = fcsRepository;
            _fundingTypesProviders = fundingTypesProviders;
        }

        public async Task<IProviderFundingStreamsAllocations> GetProviderContracts(int UKPRN, string collectionType, string summarisationType, CancellationToken cancellationToken)
        {
            if (!_isFCSContractsLoaded)
            {
                _fcsContractAllocations = await _fcsRepository.RetrieveAsync(cancellationToken);
                _isFCSContractsLoaded = true;
            }
            var fundingStreams = GetFundingTypesData(collectionType, summarisationType);

            var providerFundingStreams = new List<FundingStream>();
            var allocations = new List<IFcsContractAllocation>();

            if (fundingStreams == null)
            {
                _logger.LogInfo($"Summarisation Wrapper: Summarising UKPRN: {UKPRN} End; Funding streams found null for Summarisation Type: {summarisationType} ");
                return null;
            }

            foreach (var fs in fundingStreams)
            {
                if (_fcsContractAllocations.ContainsKey(fs.PeriodCode)
                    && _fcsContractAllocations[fs.PeriodCode].Any(x => x.DeliveryUkprn == UKPRN))
                {
                    providerFundingStreams.Add(fs);

                    foreach (var allocation in _fcsContractAllocations[fs.PeriodCode].Where(x => x.DeliveryUkprn == UKPRN))
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

        public IList<FundingStream> GetFundingTypesData(string collectionType, string summarisationType)
        {
            return _fundingTypesProviders
                .FirstOrDefault(w => w.CollectionType.Equals(collectionType, StringComparison.OrdinalIgnoreCase))?
                .Provide().Where(x => x.SummarisationType.Equals(summarisationType, StringComparison.OrdinalIgnoreCase))
                .SelectMany(fs => fs.FundingStreams)
                .ToList();
        }
    }
}
