﻿using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Reflection;

namespace ESFA.DC.Summarisation.Service.Providers
{
    public abstract class AbstractStaticDataProvider<T> : IStaticDataProvider<T>
    {
        private readonly IJsonSerializationService _jsonSerializationService;

        public AbstractStaticDataProvider(IJsonSerializationService jsonSerializationService)
        {
            _jsonSerializationService = jsonSerializationService;
        }

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
