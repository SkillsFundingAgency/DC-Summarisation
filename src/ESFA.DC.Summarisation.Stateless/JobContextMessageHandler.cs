using Autofac;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Logging.Interfaces;
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
                using (var childLifetimeScope = _lifetimeScope.BeginLifetimeScope())
                {
                    var executionContext = (ExecutionContext)childLifetimeScope.Resolve<IExecutionContext>();
                    executionContext.JobId = message.JobId.ToString();

                    _logger.LogInfo($"Summarisation Task Starting");

                    // TODO:

                    _logger.LogInfo($"Summarisation Task  Finished");

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
