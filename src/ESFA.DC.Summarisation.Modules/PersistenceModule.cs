using Autofac;
using ESFA.DC.Summarisation.Data.Persist;
using ESFA.DC.Summarisation.Data.Persist.Mapper;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Data.Persist.Persist;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Modules
{
    public class PersistenceModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummarisedActualsPersist>().As<ISummarisedActualsPersist>();
            containerBuilder.RegisterType<DataStorePersistenceService>().As<IDataStorePersistenceService>();
            containerBuilder.RegisterType<CollectionReturnMapper>().As<ICollectionReturnMapper>();
        }
    }
}