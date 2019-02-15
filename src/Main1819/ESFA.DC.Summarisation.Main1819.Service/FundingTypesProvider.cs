using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Reflection;

namespace ESFA.DC.Summarisation.Main1819.Service
{
    public class FundingTypesProvider : IFundingTypesProvider
    {
        private readonly IJsonSerializationService _jsonSerializationService;

        private const string referenceDataFileName = "ESFA.DC.Summarisation.Main1819.Service.JsonFiles.FundingTypes.json";

        public FundingTypesProvider(IJsonSerializationService jsonSerializationService)
        {
            _jsonSerializationService = jsonSerializationService;
        }

        public IEnumerable<FundingType> Provide()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(referenceDataFileName))
            {
                return _jsonSerializationService.Deserialize<List<FundingType>>(stream);
            }
        }
    }
}
