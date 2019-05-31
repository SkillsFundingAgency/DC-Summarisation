using System.Data.SqlClient;
using Autofac;
using ESF.DC.Summarisation.Main1819.Data.Providers;
using ESFA.DC.EAS1819.EF;
using ESFA.DC.EAS1819.EF.Interface;
using ESFA.DC.ESF.Database.EF;
using ESFA.DC.ESF.Database.EF.Interfaces;
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
using ESFA.DC.Summarisation.Data.Repository;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.ESF.Data.Providers;
using ESFA.DC.Summarisation.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main1819.Data.Providers;
using ESFA.DC.Summarisation.Main1819.Data.Repository;
using ESFA.DC.Summarisation.Model;
using ESFA.DC.Summarisation.Service;
using Microsoft.EntityFrameworkCore;
using ESFCollectionPeriodsProvider = ESFA.DC.Summarisation.ESF.Service.CollectionPeriodsProvider;
using ESFFundingTypesProvider = ESFA.DC.Summarisation.ESF.Service.FundingTypesProvider;
using ISummarisationContext = ESFA.DC.Summarisation.Model.Interface.ISummarisationContext;
using Main1819CollectionPeriodsProvider = ESFA.DC.Summarisation.Main1819.Service.Providers.CollectionPeriodsProvider;
using Main1819FundingTypesProvider = ESFA.DC.Summarisation.Main1819.Service.Providers.FundingTypesProvider;

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
            containerBuilder.RegisterType<SummarisationFundlineProcess>().As<ISummarisationService>();
            containerBuilder.RegisterType<SummarisationDeliverableProcess>().As<ISummarisationService>();

            containerBuilder.RegisterType<Main1819FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<ESFFundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();

            containerBuilder.RegisterType<Main1819CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();
            containerBuilder.RegisterType<ESFCollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();

            containerBuilder.RegisterType<AlbProvider>().As<ILearningDeliveryProvider>();
            containerBuilder.RegisterType<TblProvider>().As<ILearningDeliveryProvider>();
            containerBuilder.RegisterType<Fm25Provider>().As<ILearningDeliveryProvider>();
            containerBuilder.RegisterType<Fm35Provider>().As<ILearningDeliveryProvider>();
            containerBuilder.RegisterType<EasProvider>().As<ILearningDeliveryProvider>();

            containerBuilder.RegisterType<ESFProvider_R1>().As<ILearningDeliveryProvider>();

            containerBuilder.RegisterType<ProviderRepository>().As<IProviderRepository>();

            containerBuilder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();

            containerBuilder.RegisterType<FcsRepository>().As<IFcsRepository>();
            containerBuilder.RegisterType<SummarisedActualsProcessRepository>().As<ISummarisedActualsProcessRepository>();

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
                DbContextOptions<EasContext> options = new DbContextOptionsBuilder<EasContext>()
                .UseSqlServer(c.Resolve<ISummarisationDataOptions>().EAS1819ConnectionString).Options;
                return new EasContext(options);
            }).As<IEasdbContext>().InstancePerDependency();

            containerBuilder.Register(c =>
            {
                DbContextOptions<ESF_DataStoreEntities> options = new DbContextOptionsBuilder<ESF_DataStoreEntities>()
                .UseSqlServer(c.Resolve<ISummarisationDataOptions>().ESFNonEFConnectionString).Options;
                return new ESF_DataStoreEntities(options);
            }).As<IESF_DataStoreEntities>().InstancePerDependency();

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
