using System.Diagnostics;
using ESFA.DC.Summarisation.Main1819.Service.Config;

namespace ESFA.DC.Summarisation.Main1819.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var stopWatch = new Stopwatch();

            var main1819SummarisationServiceConfiguration = new Main1819SummarisationServiceConfiguration
            {
                SummarisationSqlConnectionString = @"Server=(local);Database=ESFA.DC.Summarisation.Database;Trusted_Connection=True;"
            };
        }
    }
}
