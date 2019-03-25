﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Output.Model;
using ESFA.DC.Summarisation.Data.Persist;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interfaces;

namespace ESFA.DC.Summarisation.Main1819.Service
{
    public class SummarisationWrapper : ISummarisationWrapper
    {
        private const int PageSize = 1;

        private readonly IFcsRepository _fcsRepository;
        private readonly ICollection<IProviderRepository> _repositories;
        private readonly ISummarisationService _summarisationService;
        private readonly IStaticDataProvider<FundingType> _fundingTypesProvider;
        private readonly IStaticDataProvider<CollectionPeriod> _collectionPeriodsProvider;
        private readonly IDataStorePersistenceService _dataStorePersistenceService;
        private readonly Func<SqlConnection> _sqlConnectionFactory;

        public SummarisationWrapper(
            IFcsRepository fcsRepository,
            IStaticDataProvider<FundingType> fundingTypesProvider,
            IStaticDataProvider<CollectionPeriod> collectionPeriodsProvider,
            ICollection<IProviderRepository> repositories,
            ISummarisationService summarisationService,
            IDataStorePersistenceService dataStorePersistenceService,
            Func<SqlConnection> sqlConnectionFactory)
        {
            _fundingTypesProvider = fundingTypesProvider;
            _fcsRepository = fcsRepository;
            _repositories = repositories;
            _summarisationService = summarisationService;
            _collectionPeriodsProvider = collectionPeriodsProvider;
            _dataStorePersistenceService = dataStorePersistenceService;
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<IEnumerable<SummarisedActual>> Summarise(ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var collectionPeriods = _collectionPeriodsProvider.Provide();

            var fcsContractAllocations = await _fcsRepository.RetrieveAsync(cancellationToken);
            var summarisedActuals = new List<SummarisedActual>();

            foreach (var fundModel in summarisationMessage.FundModels)
            {
                summarisedActuals.AddRange(await SummariseByFundModel(fundModel, collectionPeriods, fcsContractAllocations, cancellationToken));
            }

            var collectionReturn = await _dataStorePersistenceService.StoreCollectionReturnAsync(summarisationMessage, cancellationToken);

            using (var sqlConnection = _sqlConnectionFactory.Invoke())
            {
                await _dataStorePersistenceService.StoreSummarisedActualsDataAsync(summarisedActuals, collectionReturn, sqlConnection, cancellationToken);
            }

            return summarisedActuals;
        }

        public async Task<IEnumerable<SummarisedActual>> SummariseProviders(
            IList<FundingStream> fundingStreams,
            IProviderRepository repository,
            IEnumerable<CollectionPeriod> collectionPeriods,
            IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fcsContractAllocations,
            CancellationToken cancellationToken)
        {
            var pageNumber = 1;

            var numberOfPages = await repository.RetrieveProviderPageCountAsync(PageSize, cancellationToken);

            var actuals = new List<SummarisedActual>();

            while (pageNumber <= numberOfPages)
            {
                var providers = await repository.RetrieveProvidersAsync(PageSize, pageNumber, cancellationToken);

                foreach (var provider in providers)
                {
                    var contractFundingStreams = new List<FundingStream>();
                    var allocations = new List<IFcsContractAllocation>();

                    foreach (var fs in fundingStreams)
                    {
                        if (fcsContractAllocations.ContainsKey(fs.PeriodCode) && fcsContractAllocations[fs.PeriodCode].Any(x => x.DeliveryUkprn == provider.UKPRN))
                        {
                            contractFundingStreams.Add(fs);
                            allocations.Add(fcsContractAllocations[fs.PeriodCode].First(x => x.DeliveryUkprn == provider.UKPRN));
                        }
                    }

                    actuals.AddRange(_summarisationService.Summarise(contractFundingStreams, provider, allocations, collectionPeriods));
                }

                pageNumber++;
            }

            return actuals;
        }

        private async Task<IEnumerable<SummarisedActual>> SummariseByFundModel(
            string fundModel,
            IEnumerable<CollectionPeriod> collectionPeriods,
            IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fcsContractAllocations,
            CancellationToken cancellationToken)
        {
            var fundingStreams = _fundingTypesProvider.Provide().SelectMany(x => x.FundingStreams.Where(y => y.FundModel.ToString() == fundModel)).ToList();
            var repository = _repositories.FirstOrDefault(r => r.FundModel == fundModel);

            return await SummariseProviders(fundingStreams, repository, collectionPeriods, fcsContractAllocations, cancellationToken);
        }
    }
}
