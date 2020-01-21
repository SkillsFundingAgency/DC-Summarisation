using Autofac;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main.Interfaces;
using ESFA.DC.Summarisation.Main.Service;

namespace ESFA.DC.Summarisation.Main.Modules
{
    public class MainModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummarisationProcess>().As<ISummarisationProcess>().Keyed<ISummarisationProcess>(ProcessTypeConstants.Fundline);
            containerBuilder.RegisterType<ProviderSummarisationService>().As<IProviderSummarisationService<LearningProvider>>();
            containerBuilder.RegisterType<SummarisationFundlineProcess>().As<ISummarisationService>();
            containerBuilder.RegisterType<ProviderRepository>().As<IInputDataRepository<LearningProvider>>();
        }
    }
}
