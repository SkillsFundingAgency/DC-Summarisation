using ESFA.DC.Summarisation.Main1819.Service.Config.Interface;

namespace ESFA.DC.Summarisation.Main1819.Service.Config
{
    public class Main1819SummarisationServiceConfiguration : IMain1819SummarisationServiceConfiguration
    {
        public string SummarisationSqlConnectionString { get; set; }
    }
}
