using Autofac;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main.Interfaces;
using ESFA.DC.Summarisation.Service.Model.Config;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using ESFA.DC.EAS2021.EF;
using ESFA.DC.EAS2021.EF.Interface;
using ESFA.DC.ILR2021.DataStore.EF;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using ESFA.DC.Summarisation.Main2021.Service.Providers;

namespace ESFA.DC.Summarisation.Main.Main2021.Modules
{
    public class Main2021Module : Module
    {
        private readonly ISummarisationDataOptions _summarisationDataOptions;

        public Main2021Module(ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisationDataOptions = summarisationDataOptions;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();

            containerBuilder.RegisterType<Fm35Provider>().As<ISummarisationInputDataProvider>();
            containerBuilder.RegisterType<EasProvider>().As<ISummarisationInputDataProvider>();
            containerBuilder.RegisterType<Fm25Provider>().As<ISummarisationInputDataProvider>();
            containerBuilder.RegisterType<AlbProvider>().As<ISummarisationInputDataProvider>();
            containerBuilder.RegisterType<TblProvider>().As<ISummarisationInputDataProvider>();

            var ILRDataStoreEntitiesConfig = BuildDbContextOptions<ILR2021_DataStoreEntities>(_summarisationDataOptions.ILR2021ConnectionString, _summarisationDataOptions.SqlCommandTimeoutSeconds);

            containerBuilder.Register(c => new ILR2021_DataStoreEntities(ILRDataStoreEntitiesConfig)).As<IIlr2021Context>().ExternallyOwned();

            var EASDataStoreEntitiesConfig = BuildDbContextOptions<EasContext>(_summarisationDataOptions.EAS2021ConnectionString, _summarisationDataOptions.SqlCommandTimeoutSeconds);

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
