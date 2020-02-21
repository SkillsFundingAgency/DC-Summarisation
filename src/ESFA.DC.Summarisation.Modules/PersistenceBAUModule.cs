using Autofac;
using ESFA.DC.Summarisation.Service;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Data.BAU.Persist.Mapper;
using ESFA.DC.Summarisation.Data.BAU.Persist;
using ESFA.DC.Summarisation.Data.BAU.Persist.Interfaces;

namespace ESFA.DC.Summarisation.Modules
{
    public class PersistenceBAUModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<PublishToBAU>().As<IPublishToBAU>();
            containerBuilder.RegisterType<DataStorePersistenceServiceBAU>().As<IDataStorePersistenceServiceBAU>();
            containerBuilder.RegisterType<SummarisedActualBAUMapper>().As<ISummarisedActualBAUMapper>();
            containerBuilder.RegisterType<EventLogMapper>().As<IEventLogMapper>();
            containerBuilder.RegisterType<SummarisedActualsPersistBAU>().As<ISummarisedActualsPersistBAU>();
        }
    }
}