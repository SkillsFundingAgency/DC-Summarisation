using Autofac;
using ESFA.DC.Summarisation.Apps.Interfaces;
using ESFA.DC.Summarisation.Apps.Service;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Apps.Modules
{
    public class AppsModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummarisationProcess>().As<ISummarisationProcess>().Keyed<ISummarisationProcess>(ProcessTypeConstants.Fundline);
            containerBuilder.RegisterType<ProviderSummarisationService>().As<IProviderSummarisationService<LearningProvider>>();
            containerBuilder.RegisterType<SummarisationPaymentsProcess>().As<ISummarisationService>();
            containerBuilder.RegisterType<ProviderRepository>().As<IInputDataRepository<LearningProvider>>();
        }
    }
}