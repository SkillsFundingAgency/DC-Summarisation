using System.Data.SqlClient;
using Autofac;
using ESF.DC.Summarisation.Main1819.Data.Repository;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.ServiceFabric.Helpers;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Configuration.Interface;
using ESFA.DC.Summarisation.Data.Persist;
using ESFA.DC.Summarisation.Data.Persist.BulkInsert;
using ESFA.DC.Summarisation.Data.Persist.Mapper;
using ESFA.DC.Summarisation.Data.Persist.Mapper.Interface;
using ESFA.DC.Summarisation.Data.Persist.Persist;
using ESFA.DC.Summarisation.Data.Persist.Persist.Interface;
using ESFA.DC.Summarisation.Data.Population.Service;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main1819.Data.Repository;
using ESFA.DC.Summarisation.Main1819.Service.Providers;
using ESFA.DC.Summarisation.Model;
using ESFA.DC.Summarisation.Service;
using Microsoft.EntityFrameworkCore;
using ISummarisationContext = ESFA.DC.Summarisation.Model.Interface.ISummarisationContext;

namespace ESFA.DC.Summarisation.Modules
{
    public class SummarisationModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            var configHelper = new ConfigurationHelper();

            var referenceDataOptions = configHelper.GetSectionValues<SummarisationDataOptions>("ReferenceDataSection");
            containerBuilder.RegisterInstance(referenceDataOptions).As<ISummarisationDataOptions>().SingleInstance();

            containerBuilder.RegisterType<SummarisationWrapper>().As<ISummarisationWrapper>();
            containerBuilder.RegisterType<SummarisationService>().As<ISummarisationService>();

            containerBuilder.RegisterType<FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();

            containerBuilder.Register(c =>
            {
                var connectionString = c.Resolve<ISummarisationDataOptions>().ILR1819ConnectionString;
                return new Fm35Repository(() => new SqlConnection(connectionString));
            }).As<IProviderRepository>();

            containerBuilder.Register(c =>
            {
                var connectionString = c.Resolve<ISummarisationDataOptions>().EAS1819ConnectionString;
                return new EasRepository(() => new SqlConnection(connectionString));
            }).As<IProviderRepository>();

            containerBuilder.RegisterType<AlbRepository>().As<IProviderRepository>();

            containerBuilder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();

            containerBuilder.RegisterType<FcsRepository>().As<IFcsRepository>();

            containerBuilder.RegisterType<BulkInsert>().As<IBulkInsert>();
            containerBuilder.RegisterType<SummarisedActualsPersist>().As<ISummarisedActualsPersist>();

            containerBuilder.RegisterType<CollectionReturnMapper>().As<ICollectionReturnMapper>();

            containerBuilder.RegisterType<DataStorePersistenceService>().As<IDataStorePersistenceService>();

            containerBuilder.Register(c => new SqlConnection(c.Resolve<ISummarisationDataOptions>().SummarisedActualsConnectionString)).As<SqlConnection>();

            containerBuilder.Register(c =>
            {
                DbContextOptions<FcsContext> options = new DbContextOptionsBuilder<FcsContext>()
                .UseSqlServer(c.Resolve<ISummarisationDataOptions>().FCSConnectionString).Options;
                return new FcsContext(options);
            }).As<IFcsContext>().InstancePerDependency();

            containerBuilder.Register(c =>
            {
                DbContextOptions<ILR1819_DataStoreEntities> options = new DbContextOptionsBuilder<ILR1819_DataStoreEntities>()
                .UseSqlServer(c.Resolve<ISummarisationDataOptions>().ILR1819ConnectionString).Options;
                return new ILR1819_DataStoreEntities(options);
            }).As<IIlr1819RulebaseContext>().InstancePerDependency();

            containerBuilder.Register(c =>
            {
                DbContextOptions<SummarisationContext> options = new DbContextOptionsBuilder<SummarisationContext>()
                .UseSqlServer(c.Resolve<ISummarisationDataOptions>().SummarisedActualsConnectionString).Options;
                return new SummarisationContext(options);
            }).As<ISummarisationContext>().As<SummarisationContext>()
            .InstancePerDependency();
        }
    }
}
