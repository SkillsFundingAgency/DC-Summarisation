using System.Collections.Generic;

namespace ESFA.DC.Summarisation.Common.Config.Interface
{
    public interface IServiceFabricConfigurationService
    {
        IDictionary<string, string> GetConfigSectionAsDictionary(string sectionName);

        T GetConfigSectionAs<T>(string sectionName);

        IStatelessServiceConfiguration GetConfigSectionAsStatelessServiceConfiguration();
    }
}
