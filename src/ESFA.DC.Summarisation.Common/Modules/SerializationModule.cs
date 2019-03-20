using Autofac;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Common.Modules
{
    public class SerializationModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>();
            containerBuilder.RegisterType<JsonSerializationService>()
                .As<IJsonSerializationService>()
                .As<ISerializationService>();
        }
    }
}