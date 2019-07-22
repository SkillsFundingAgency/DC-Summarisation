﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Data.Output.Model;

namespace ESFA.DC.Summarisation.Interfaces
{
    public interface ISummarisationWrapper
    {
        Task<IEnumerable<SummarisedActual>> Summarise(CancellationToken cancellationToken);
    }
}
