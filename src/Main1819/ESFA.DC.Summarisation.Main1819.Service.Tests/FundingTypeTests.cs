using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.Summarisation.Main1819.Service.Tests
{
    public class FundingTypeTests 
    {
        private IJsonSerializationService jsonSerializationService;

        public FundingTypeTests()
        {
            jsonSerializationService = new JsonSerializationService();
        }

        [Fact]
        public void FundingTypesCount()
        {

            FundingTypesProvider fundingTypesProvider = new FundingTypesProvider(jsonSerializationService);

            var count = fundingTypesProvider.Provide().ToList().Count();

            Assert.Equal(10, count);
        }
    }
}
