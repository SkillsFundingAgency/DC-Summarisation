using System.Collections.Generic;
using System.Reflection;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Main1819.Service.Providers
{
    public abstract class AbstractStaticDataProvider<T> : IStaticDataProvider<T>
    {
        private readonly IJsonSerializationService _jsonSerializationService;

        protected internal abstract string ReferenceDataFileName { get; }

        public AbstractStaticDataProvider(IJsonSerializationService jsonSerializationService)
        {
            _jsonSerializationService = jsonSerializationService;
        }

        public virtual IEnumerable<T> Provide()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(ReferenceDataFileName))
            {
                return _jsonSerializationService.Deserialize<List<T>>(stream);
            }
        }
    }
}
