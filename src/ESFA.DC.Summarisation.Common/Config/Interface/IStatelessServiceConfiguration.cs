using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Common.Config.Interface
{
    public interface IStatelessServiceConfiguration
    {
        string ServiceBusConnectionString { get; }

        string TopicName { get; }

        string SubscriptionName { get; }

        string TopicMaxConcurrentCalls { get; }

        string TopicMaxCallbackTimeSpanMinutes { get; }

        string JobStatusQueueName { get; }

        string JobStatusMaxConcurrentCalls { get; }

        string AuditQueueName { get; }

        string AuditMaxConcurrentCalls { get; }

        string LoggerConnectionString { get; }
    }
}
