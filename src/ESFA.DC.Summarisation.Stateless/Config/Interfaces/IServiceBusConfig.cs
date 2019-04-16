using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Stateless.Config.Interfaces
{
    public interface IServiceBusConfig
    {
        string ServiceBusConnectionString { get;  }

        string TopicName { get; }

        string SubscriptionName { get; }

        string JobStatusQueueName { get; }

        string AuditQueueName { get; }

        string LoggerConnectionString { get; }
    }
}
