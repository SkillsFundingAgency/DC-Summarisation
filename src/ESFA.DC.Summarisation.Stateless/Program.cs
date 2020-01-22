using System;
using System.Diagnostics;
using System.Threading;
using Autofac;
using Autofac.Integration.ServiceFabric;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.Summarisation.Modules;
using ESFA.DC.ServiceFabric.Common.Config;
using ESFA.DC.ServiceFabric.Common.Modules;
using ESFA.DC.Queueing;
using ESFA.DC.Summarisation.Stateless.Config;
using ESFA.DC.Summarisation.Stateless.Config.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Queueing.Interface;

namespace ESFA.DC.Summarisation.Stateless
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.
                var builder = BuildContainerBuilder();

                builder.RegisterServiceFabricSupport();

                builder.RegisterStatelessService<ServiceFabric.Common.Stateless>("ESFA.DC.Summarisation.StatelessType");

                using (var container = builder.Build())
                {
                    var manager = container.Resolve<IJobContextManager<JobContextMessage>>();

                    ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(ServiceFabric.Common.Stateless).Name);

                    // Prevents this host process from terminating so services keep running.
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static ContainerBuilder BuildContainerBuilder()
        {
            var containerBuilder = new ContainerBuilder();

            var serviceFabricConfigurationService = new ServiceFabricConfigurationService();

            var statelessServiceConfiguration = serviceFabricConfigurationService.GetConfigSectionAsStatelessServiceConfiguration();

            // get ServiceBus, Azurestorage config values and register container
            var serviceBusOptions = serviceFabricConfigurationService.GetConfigSectionAs<ServiceBusConfig>("StatelessServiceConfiguration");
            containerBuilder.RegisterInstance(serviceBusOptions).As<IServiceBusConfig>().SingleInstance();

            var topicSubscribeConfig = new TopicConfiguration(
                serviceBusOptions.ServiceBusConnectionString,
                serviceBusOptions.TopicName,
                serviceBusOptions.SubscriptionName,
                1,
                maximumCallbackTimeSpan: TimeSpan.FromMinutes(60));

            containerBuilder.Register(c =>
            {
                var topicSubscriptionSevice =
                    new TopicSubscriptionSevice<JobContextMessage>(
                        topicSubscribeConfig,
                        c.Resolve<ISerializationService>(),
                        c.Resolve<ILogger>());
                return topicSubscriptionSevice;
            }).As<ITopicSubscriptionService<JobContextMessage>>();

            containerBuilder.RegisterModule(new StatelessServiceModule(statelessServiceConfiguration));
            containerBuilder.RegisterModule<SerializationModule>();
            containerBuilder.RegisterType<JobContextMessageHandler>().As<IMessageHandler<JobContextMessage>>();

            var summarisationDataOptions = serviceFabricConfigurationService.GetConfigSectionAs<SummarisationDataOptions>("ReferenceDataSection");

            containerBuilder.RegisterModule(new SummarisationModule(summarisationDataOptions));

            return containerBuilder;
        }
    }
}
