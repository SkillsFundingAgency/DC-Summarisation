﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;

namespace ESFA.DC.Summarisation.Service
{
    public class SummarisationWrapper : ISummarisationWrapper
    {
        private readonly IDataStorePersistenceService _dataStorePersistenceService;
        private readonly ILogger _logger;
        private readonly IEnumerable<ISummarisationProcess> _summarisationProcesses;

        public SummarisationWrapper(
            IDataStorePersistenceService dataStorePersistenceService,
            ILogger logger,
            IEnumerable<ISummarisationProcess> summarisationProcesses)
        {
            _dataStorePersistenceService = dataStorePersistenceService;
            _logger = logger;
            _summarisationProcesses = summarisationProcesses;
        }

        public async Task<ICollection<SummarisedActual>> Summarise(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var summarisationProcess = _summarisationProcesses.Single(p => p.ProcessType.Equals(summarisationMessage.ProcessType, StringComparison.OrdinalIgnoreCase));

            var summarisedActuals = await summarisationProcess.CollateAndSummariseAsync(summarisationMessage, cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Storing data to Summarised Actuals Start");

            await _dataStorePersistenceService.StoreSummarisedActualsDataAsync(
                summarisedActuals.ToList(),
                summarisationMessage,
                cancellationToken);

            _logger.LogInfo($"Summarisation Wrapper: Storing data to Summarised Actuals End");

            return summarisedActuals;
        }
    }
}
