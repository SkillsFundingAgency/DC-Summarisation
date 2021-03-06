﻿using System.Collections.Generic;
using System.Reflection;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Main.Main2021.Service.Providers
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

        public virtual ICollection<T> Provide()
        {
            using (var stream = Assembly.GetManifestResourceStream(ReferenceDataFileName))
            {
                return _jsonSerializationService.Deserialize<T[]>(stream);
            }
        }
    }
}
