﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.Output.Model;
using ESFA.DC.Summarisation.Model;

namespace ESFA.DC.Summarisation.Data.Repository.Interface
{
    public interface ISummarisedActualsProcessRepository
    {
        Task<CollectionReturn> GetLastCollectionReturnForCollectionTypeAsync(string collectionType, CancellationToken cancellationToken);

        Task<IEnumerable<Output.Model.SummarisedActual>> GetLatestSummarisedActualsAsync(int collectionReturnId, CancellationToken cancellationToken);

        Task<IEnumerable<Output.Model.SummarisedActual>> GetSummarisedActualsForCollectionReturnAndOrganisationAsync(int collectionReturnId, string organisationId, CancellationToken cancellationToken);

        Task<IEnumerable<Output.Model.SummarisedActual>> GetSummarisedActualsForCollectionRetrunAndFSPsAsync(int collectionReturnId, IEnumerable<string> fundingStreams, CancellationToken cancellationToken);
    }
}
