﻿using Autofac;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Stateless.Context;
using System;
using System.Threading;
using System.Threading.Tasks;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.Summarisation.Stateless
{
    public class JobContextMessageHandler : IMessageHandler<JobContextMessage>
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger _logger;

        public JobContextMessageHandler(ILifetimeScope lifetimeScope, ILogger logger)
        {
            _lifetimeScope = lifetimeScope;
            _logger = logger;
        }

        public async Task<bool> HandleAsync(JobContextMessage message, CancellationToken cancellationToken)
        {
            try
            {
                var messageContext = new JobContextMessageSummarisationContext(message);

                using (var childLifetimeScope = this._lifetimeScope.BeginLifetimeScope(
                    c =>
                    {
                        c.RegisterInstance(message).As<IJobContextMessage>();
                        c.RegisterInstance(messageContext).As<ISummarisationMessage>();
                        }))
                {
                    var executionContext = (ExecutionContext)childLifetimeScope.Resolve<IExecutionContext>();
                    executionContext.JobId = message.JobId.ToString();

                    _logger.LogInfo($"Summarisation Task Starting");

                    var summarisationWrapper = childLifetimeScope.Resolve<ISummarisationWrapper>();

                    await summarisationWrapper.Summarise(cancellationToken);

                    _logger.LogInfo($"Summarisation Task Finished");

                    return true;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError("Summarisation Message Handler Failed", exception);
                throw;
            }
        }
    }
}
