using Autofac;
using ESFA.DC.Summarisation.Apps.Service;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Apps.Modules
{
    public class AppsModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<SummarisationProcess>().As<ISummarisationProcess>().Keyed<ISummarisationProcess>(ProcessTypeConstants.Fundline);
        }
    }
}