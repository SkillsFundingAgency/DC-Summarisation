using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Reflection;

namespace ESFA.DC.Summarisation.Main1819.Service
{
    public class CollectionPeriodsProvider : ICollectionPeriodsProvider
    {
        private readonly IJsonSerializationService _jsonSerializationService;

        private const string collectionPeriodsName = "ESFA.DC.Summarisation.Main1819.Service.JsonFiles.CollectionPeriods.json";

        public CollectionPeriodsProvider(IJsonSerializationService jsonSerializationService)
        {
            _jsonSerializationService = jsonSerializationService;
        }

        public IEnumerable<CollectionPeriod> Provide()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(collectionPeriodsName))
            {
                return _jsonSerializationService.Deserialize<List<CollectionPeriod>>(stream);
            }
        }
    }
}
