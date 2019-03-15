using Autofac;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.Persist;
using ESFA.DC.Summarisation.Data.Persist.BulkInsert;
using ESFA.DC.Summarisation.Data.Persist.Mapper;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Data.Persist.Persist;
using ESFA.DC.Summarisation.Data.Persist.Persist.Interface;
using ESFA.DC.Summarisation.Data.Population.Service;
using ESFA.DC.Summarisation.Data.Repository;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main1819.Service;
using ESFA.DC.Summarisation.Main1819.Service.Providers;

namespace ESFA.DC.Summarisation.Modules
{
    public class SummarisationModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummarisationWrapper>().As<ISummarisationWrapper>();
            containerBuilder.RegisterType<SummarisationService>().As<ISummarisationService>();

            containerBuilder.RegisterType<Fm35Repository>().As<IProviderRepository>();
            containerBuilder.RegisterType<AlbRepository>().As<IProviderRepository>();

            containerBuilder.RegisterType<FundingTypesProvider>().As<IStaticDataProvider<FundingType>>();
            containerBuilder.RegisterType<CollectionPeriodsProvider>().As<IStaticDataProvider<CollectionPeriod>>();

            containerBuilder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();

            containerBuilder.RegisterType<FcsRepository>().As<IFcsRepository>();

            containerBuilder.RegisterType<BulkInsert>().As<IBulkInsert>();
            containerBuilder.RegisterType<SummarisedActualsMapper>().As<ISummarisedActualsMapper>();
            containerBuilder.RegisterType<SummarisedActualsPersist>().As<ISummarisedActualsPersist>();

            containerBuilder.RegisterType<CollectionReturnMapper>().As<ICollectionReturnMapper>();
            containerBuilder.RegisterType<CollectionReturnPersist>().As<ICollectionReturnPersist>();

            containerBuilder.RegisterType<DataStorePersistenceService>().As<IDataStorePersistenceService>();
        }
    }
}
