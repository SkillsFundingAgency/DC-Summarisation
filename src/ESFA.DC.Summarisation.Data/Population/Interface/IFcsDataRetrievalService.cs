using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;

namespace ESFA.DC.Summarisation.Data.Population.Interface
{
    public interface IFcsDataRetrievalService
    {
        /// <summary>
        /// Retrieves the (FCS Contracts) asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// a task running the collection builder.
        /// </returns>
        Task<IReadOnlyCollection<IFcsContractAllocation>> RetrieveAsync(CancellationToken cancellationToken);
    }
}
