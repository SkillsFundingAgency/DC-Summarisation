using Autofac;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Modules.Abstract
{
    public abstract class AbstractProviderModule : Module
    {
        protected Type RepositoryType { get; set; }

        protected IEnumerable<Type> Repositories { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            foreach (var repository in Repositories)
            {
                builder.RegisterType(repository).As(RepositoryType).InstancePerLifetimeScope();
            }
        }
    }
}

