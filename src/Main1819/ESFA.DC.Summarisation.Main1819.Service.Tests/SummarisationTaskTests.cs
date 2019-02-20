using ESFA.DC.Summarisation.Data.Input.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Interfaces;
using Xunit;
using FluentAssertions;
using ESFA.DC.Summarisation.Main1819.Service.Tasks;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Main1819.Service.Providers;
using ESFA.DC.Summarisation.Configuration;

namespace ESFA.DC.Summarisation.Main1819.Service.Tests
{
    public class SummarisationTaskTests
    {
        [Fact]
        public void TestSummarisatonTask()
        {
            Provider provider = GetData().First(x => x.UKPRN == 10023139);
            HashSet<string> attributes = new HashSet<string> { "AchievePayment", "BalancePayment", "OnProgPayment", "EmpOutcomePay" };
            FundingType fundingType = NewProvider().Provide().First(x => x.Key == "ProgFundingFM35andEAS");

            //SummarisationTask task = new SummarisationTask("ProgFundingFM35andEAS", providers, attributes,fundingTypes);

            var task = new SummarisationTask();

            var results = task.Summarise(fundingType, provider);

            //var results = task.Summarise(fundingType, provider, attributes);

            //task.ExecuteAsync();
        }

        [Fact]
        public void PerformanceTest()
        {
            Provider provider = GetData().First(x => x.UKPRN == 10023139);

            for (int i = 0; i < 18; i++)
            {
                provider.LearningDeliveries.AddRange(provider.LearningDeliveries);
            }

            HashSet<string> attributes = new HashSet<string> { "AchievePayment", "BalancePayment", "OnProgPayment", "EmpOutcomePay" };
            FundingType fundingType = NewProvider().Provide().First(x => x.Key == "ProgFundingFM35andEAS");

            //SummarisationTask task = new SummarisationTask("ProgFundingFM35andEAS", providers, attributes,fundingTypes);

            var task = new SummarisationTask();

            var results = task.Summarise(fundingType, provider);

            //var results = task.Summarise(fundingType, provider, attributes);

            //task.ExecuteAsync();
        }

        [Fact]
        public void GetDataTest()
        {
            var result = GetData();

        }

        private  List<Provider> GetData()
        {
           string TestDataFileName  = "ESFA.DC.Summarisation.Main1819.Service.Tests.JsonFiles.InputTestData.json";
           IJsonSerializationService _jsonSerializationService = new JsonSerializationService();

            List<Provider> result;

            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(TestDataFileName))
            {
                return result = _jsonSerializationService.Deserialize<List<Provider>>(stream);
            }
        }

        private FundingTypesProvider NewProvider()
        {
            return new FundingTypesProvider(new JsonSerializationService());
        }

    }
}
