﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Stateless.Context;
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
                var summarisationMessage = new JobContextMessageSummarisationContext(message);

                using (var childLifetimeScope = this._lifetimeScope.BeginLifetimeScope(
                    c =>
                    {
                    }))
                {
                    var executionContext = (ExecutionContext)childLifetimeScope.Resolve<IExecutionContext>();
                    executionContext.JobId = message.JobId.ToString();

                    _logger.LogInfo($"Summarisation Task Starting");

                    var summarisationWrapper = childLifetimeScope.Resolve<ISummarisationWrapper>();

                    await summarisationWrapper.Summarise(summarisationMessage, cancellationToken);

                    _logger.LogInfo($"Summarisation Task Finished");

                    if (summarisationMessage.PublishToBAU)
                    {
                        _logger.LogInfo($"Publish to BAU Task Starting");

                        var publishToBAU = childLifetimeScope.Resolve<IPublishToBAU>();

                        await publishToBAU.PublishAsync(summarisationMessage, cancellationToken);

                        _logger.LogInfo($"Publish to BAU Task Finished");
                    }

                    return true;
                }
            }
            catch (OutOfMemoryException oom)
            {
                Environment.FailFast("Summarisation Service Out Of Memory", oom);
                _logger.LogError("Summarisation Service Out Of Memory", oom);
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError("Summarisation Message Handler Failed", exception);
                throw;
            }
        }
    }
}
