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
    public class ProviderFundingStreamsService : IProviderFundingStreamsService
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<ISummarisationConfigProvider<FundingType>> _fundingTypesProviders;
        private IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> _fcsContractAllocations;
        private bool _isFCSContractsLoaded = false;

        public ProviderFundingStreamsService(
            IEnumerable<ISummarisationConfigProvider<FundingType>> fundingTypesProviders,
            ILogger logger)
        {
            _fundingTypesProviders = fundingTypesProviders;
            _logger = logger;
        }

        public async Task<List<FundingStream>> GetProviderFundingStreams(int UKPRN, string collectionType, string summarisationType, List<IFcsContractAllocation> providerAllocations, CancellationToken cancellationToken)
        {
            var providerFundingStreams = new List<FundingStream>();
            
            var fundingStreams = GetFundingTypesData(collectionType, summarisationType);

            if (fundingStreams == null)
            {
                _logger.LogInfo($"Summarisation Wrapper: Summarising UKPRN: {UKPRN} End; Funding streams found null for Summarisation Type: {summarisationType} ");
                return providerFundingStreams;
            }

            foreach (var fs in fundingStreams)
            {
                if (providerAllocations.Any(x => x.DeliveryUkprn == UKPRN && x.FundingStreamPeriodCode.Equals(fs.PeriodCode, StringComparison.OrdinalIgnoreCase)))
                {
                    providerFundingStreams.Add(fs);
                }
            }

            return providerFundingStreams;
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
