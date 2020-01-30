using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Generic.Interfaces;

namespace ESFA.DC.Summarisation.Generic.Service
{
    public class SummarisationProcess : ISummarisationProcess
    {
        public string ProcessType => ProcessTypeConstants.Generic;
        private readonly int _dataRetrievalMaxConcurrentCalls;
        
        private readonly IGenericCollectionRepository _genericCollectionRepository;
        private readonly IProviderSummarisationService<IEnumerable<SummarisedActual>> _providerSummarisationService;
        private readonly ILogger _logger;
        

        public SummarisationProcess(
            IGenericCollectionRepository genericCollectionRepository,
            IProviderSummarisationService<IEnumerable<SummarisedActual>> providerSummarisationService,
            ISummarisationDataOptions dataOptions,
            ILogger logger)
        {
            _logger = logger;
            _genericCollectionRepository = genericCollectionRepository;
            _providerSummarisationService = providerSummarisationService;
            _dataRetrievalMaxConcurrentCalls = 4;
            int.TryParse(dataOptions.DataRetrievalMaxConcurrentCalls, out _dataRetrievalMaxConcurrentCalls);
        }

        public async Task<ICollection<SummarisedActual>> CollateAndSummariseAsync(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Summarisation Message: CollectionType : {summarisationMessage.CollectionType}, CollectionReturnCode: {summarisationMessage.CollectionReturnCode}, CollectionYear: {summarisationMessage.CollectionYear}, ReturnPeriod: {summarisationMessage.CollectionMonth}");

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Generic Collection Data Start");

            var genericCollectionData = await _genericCollectionRepository.RetrieveAsync(summarisationMessage.CollectionType, cancellationToken);

            var summarisedActuals = await _providerSummarisationService.Summarise(genericCollectionData, summarisationMessage, cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Retrieving Retrieving Generic Collection Data End");

            _logger.LogInfo($"Summarisation Wrapper: Summarisation End");

            return summarisedActuals;
        }
    }
}
