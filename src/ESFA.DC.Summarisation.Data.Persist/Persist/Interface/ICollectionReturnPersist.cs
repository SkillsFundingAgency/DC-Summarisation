using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Persist.Persist.Interface
{
    public interface ICollectionReturnPersist
    {
        Task<CollectionReturn> Save(Output.Model.CollectionReturn collectionReturn, CancellationToken cancellationToken);
    }
}