using ESFA.DC.Summarisation.Data.Repository;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Modules.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Summarisation.Modules
{
    public class ProviderRepositoryModule : AbstractProviderModule
    {
        public ProviderRepositoryModule()
        {
            RepositoryType = typeof(IProviderRepository);

            Repositories = new List<Type>
            {
                typeof(Fm35Repository),
                typeof(AlbRepository)
            };
        }
    }
}
