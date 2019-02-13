using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using System.Linq;
using Xunit;

namespace ESFA.DC.Summarisation.Main1819.Service.Tests
{
    public class CollectionPeriodTests
    {
        private IJsonSerializationService jsonSerializationService;

        public CollectionPeriodTests()
        {
            jsonSerializationService = new JsonSerializationService();
        }
        
        [Fact]
        public void CollectionPeriodCount()
        {

            CollectionPeriodsProvider collectionPeriodsProvider = new CollectionPeriodsProvider(jsonSerializationService);

            var count =  collectionPeriodsProvider.Provide().ToList().Count();

            Assert.Equal(12,count);
        }
    }
}
