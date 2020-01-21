using ESFA.DC.Summarisation.Stateless.Config.Interfaces;

namespace ESFA.DC.Summarisation.Stateless.Config
{
    public class ServiceBusConfig : IServiceBusConfig
    {
        public string ServiceBusConnectionString { get; set; }

        public string TopicName { get; set; }

        public string SubscriptionName { get; set; }

        public string JobStatusQueueName { get; set; }

        public string AuditQueueName { get; set; }

        public string LoggerConnectionString { get; set; }
    }
}
