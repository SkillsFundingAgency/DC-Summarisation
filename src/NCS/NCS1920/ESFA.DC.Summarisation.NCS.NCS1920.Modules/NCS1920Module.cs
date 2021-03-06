﻿using Autofac;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.NCS.Model.Config;
using ESFA.DC.Summarisation.NCS.NCS1920.Service.Providers;

namespace ESFA.DC.Summarisation.NCS.NCS1920.Modules
{
    public class NCS1920Module : Module
    {
        public NCS1920Module()
        {
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();
        }
    }
}
