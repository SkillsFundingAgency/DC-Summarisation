using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Reflection;

namespace ESFA.DC.Summarisation.Service.Providers
{
    public abstract class AbstractSummarisationConfigProvider<T> : ISummarisationConfigProvider<T>
    {
        private readonly IJsonSerializationService _jsonSerializationService;

        public AbstractSummarisationConfigProvider(IJsonSerializationService jsonSerializationService)
        {
            _jsonSerializationService = jsonSerializationService;
        }

        public virtual string CollectionType { get; }

        protected internal abstract string ReferenceDataFileName { get; }

        protected internal abstract Assembly Assembly { get; }

        public virtual IEnumerable<T> Provide()
        {
            using (var stream = Assembly.GetManifestResourceStream(ReferenceDataFileName))
            {
                return _jsonSerializationService.Deserialize<IEnumerable<T>>(stream);
            }
        }
    }
}
