﻿using System;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Input.Interface;
using SummarisedActual = ESFA.DC.Summarisation.Data.output.Model.SummarisedActual;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.ESF.Interfaces;

namespace ESFA.DC.Summarisation.ESF.Service
{
    public class ProviderSummarisationService : IProviderSummarisationService<ILearningProvider>
    {
        private readonly ISummarisationService _summarisationService;
        private readonly ILogger _logger;
        private readonly IProviderContractsService _providerContractsService;
        private readonly IProviderFundingDataRemovedService _providerFundingDataRemovedService;        

        public ProviderSummarisationService(
            ISummarisationService summarisationService,
            ILogger logger,
            IProviderContractsService providerContractsService,
            IProviderFundingDataRemovedService providerFundingDataRemovedService)
        {
            _summarisationService = summarisationService;
            _logger = logger;
            _providerContractsService = providerContractsService;
            _providerFundingDataRemovedService = providerFundingDataRemovedService;
        }

        public async Task<ICollection<SummarisedActual>> Summarise(ILearningProvider providerData, ICollection<CollectionPeriod> collectionPeriods, ICollection<FundingType> fundingTypes,  IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> contractAllocations, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var providerActuals = new List<SummarisedActual>();            
            
            foreach (var summarisationType in summarisationMessage.SummarisationTypes)
            {
                if (!summarisationType.Equals(ConstantKeys.ReRunSummarisation, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {providerData.UKPRN}, Fundmodel {summarisationType} Start");

                    var fundingStreams = fundingTypes?
                        .Where(x => x.SummarisationType.Equals(summarisationType, StringComparison.OrdinalIgnoreCase))
                        .SelectMany(fs => fs.FundingStreams)
                        .ToList();

                    var providerfundingstreamsContracts = await _providerContractsService.GetProviderContracts(providerData.UKPRN, fundingStreams, contractAllocations, cancellationToken);
               
                    var summarisedData = _summarisationService.Summarise(providerfundingstreamsContracts.FundingStreams, providerData, providerfundingstreamsContracts.FcsContractAllocations, collectionPeriods, summarisationMessage);

                    providerActuals.AddRange(summarisedData);

                    _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {providerData.UKPRN}, Fundmodel {summarisationType} End");
                }
            }

            if (!summarisationMessage.ProcessType.Equals(ProcessTypeConstants.Payments, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed  Rule UKPRN: {providerData.UKPRN} Start");

                var actualsToCarry = await _providerFundingDataRemovedService.FundingDataRemovedAsync(providerActuals, summarisationMessage, cancellationToken);

                providerActuals.AddRange(actualsToCarry);

                _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed  Rule UKPRN: {providerData.UKPRN} End");
            }

            return providerActuals;
        }
    }
}
