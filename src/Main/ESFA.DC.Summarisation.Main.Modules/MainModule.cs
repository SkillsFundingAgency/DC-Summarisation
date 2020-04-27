using Autofac;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main.Interfaces;
using ESFA.DC.Summarisation.Main.Main1920.Modules;
using ESFA.DC.Summarisation.Main.Model;
using ESFA.DC.Summarisation.Main.Service;

namespace ESFA.DC.Summarisation.Main.Modules
{
    public class MainModule : Module
    {
        private readonly ISummarisationDataOptions _summarisationDataOptions;

        public MainModule(ISummarisationDataOptions summarisationDataOptions)
        {
            _summarisationDataOptions = summarisationDataOptions;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummarisationProcess>().As<ISummarisationProcess>().Keyed<ISummarisationProcess>(ProcessTypeConstants.Fundline);
            containerBuilder.RegisterType<ProviderSummarisationService>().As<IProviderSummarisationService<LearningProvider>>();
            containerBuilder.RegisterType<SummarisationFundlineProcess>().As<ISummarisationService>();
            containerBuilder.RegisterType<ProviderRepository>().As<IInputDataRepository<LearningProvider>>();
            containerBuilder.RegisterType<ProviderContractsService>().As<IProviderContractsService>();
            containerBuilder.RegisterType<FundingDataRemovedService>().As<IFundingDataRemovedService>();

            containerBuilder.RegisterModule(new Main1920Module(_summarisationDataOptions));
        }
    }
}
