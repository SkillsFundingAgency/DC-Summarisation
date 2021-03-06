﻿using Autofac;
using ESFA.DC.Summarisation.Apps.Apps1920.Service;
using ESFA.DC.Summarisation.Apps.Model.Config;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Apps.Apps1920.Modules
{
    public class Apps1920Module : Module
    {
        private readonly ISummarisationDataOptions _summarisationDataOptions;

        public Apps1920Module(ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisationDataOptions = summarisationDataOptions;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<FundingTypesProvider>().As<ISummarisationConfigProvider<FundingType>>();
            containerBuilder.RegisterType<CollectionPeriodsProvider>().As<ISummarisationConfigProvider<CollectionPeriod>>();
        }
    }
}
