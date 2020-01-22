using Autofac;
using ESFA.DC.EAS1920.EF;
using ESFA.DC.EAS1920.EF.Interface;
using ESFA.DC.ILR1920.DataStore.EF;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main.Model;
using ESFA.DC.Summarisation.Main1920.Service.Providers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Main.Main1920.Modules
{
    public class Main1920Module : Module
    {
        private readonly ISummarisationDataOptions _summarisationDataOptions;

        public Main1920Module(ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisationDataOptions = summarisationDataOptions;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();

            containerBuilder.RegisterType<Fm35Provider>().As<ISummarisationInputDataProvider<LearningProvider>>();
            containerBuilder.RegisterType<EasProvider>().As<ISummarisationInputDataProvider<LearningProvider>>();
            containerBuilder.RegisterType<Fm25Provider>().As<ISummarisationInputDataProvider<LearningProvider>>();
            containerBuilder.RegisterType<AlbProvider>().As<ISummarisationInputDataProvider<LearningProvider>>();
            containerBuilder.RegisterType<TblProvider>().As<ISummarisationInputDataProvider<LearningProvider>>();

            var ILRDataStoreEntitiesConfig = BuildDbContextOptions<ILR1920_DataStoreEntities>(_summarisationDataOptions.ILR1920ConnectionString, _summarisationDataOptions.SqlCommandTimeoutSeconds);

            containerBuilder.Register(c => new ILR1920_DataStoreEntities(ILRDataStoreEntitiesConfig)).As<IIlr1920RulebaseContext>().ExternallyOwned();

            var EASDataStoreEntitiesConfig = BuildDbContextOptions<EasContext>(_summarisationDataOptions.EAS1920ConnectionString, _summarisationDataOptions.SqlCommandTimeoutSeconds);

            containerBuilder.Register(c => new EasContext(EASDataStoreEntitiesConfig)).As<IEasdbContext>().ExternallyOwned();
        }

        private DbContextOptions<T> BuildDbContextOptions<T>(string connectionString, string sqlCommandTimeOut) where T : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<T>();

            optionsBuilder.UseSqlServer(
                connectionString,
                options =>
                {
                    options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>());
                    options.CommandTimeout(int.Parse(sqlCommandTimeOut));
                });

            return optionsBuilder.Options;
        }
    }
}
