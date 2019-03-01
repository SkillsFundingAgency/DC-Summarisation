using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.output.Model;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main1819.Service.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Summarisation.Main1819.Service
{
    public class SummarisationWrapper
    {
        private IEnumerable<CollectionPeriod> _collectionPeriods;
        private readonly IFcsRepository _fcsRepository;
        private readonly ICollection<IProviderRepository> _repositories;
        private readonly FundingTypesProvider _fundingTypesProvider;
        private readonly CollectionPeriodsProvider _collectionPeriodsProvider;

        private IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> _fcsContractAllocations;

        private const int PageSize = 100;

        public SummarisationWrapper(IFcsRepository fcsRepository,
                                    FundingTypesProvider fundingTypesProvider,
                                    CollectionPeriodsProvider collectionPeriodsProvider,
                                    ICollection<IProviderRepository> repositories)
        {
            _fundingTypesProvider = fundingTypesProvider;
            _fcsRepository = fcsRepository;
            _repositories = repositories;
            
        }

        public async void Summarise()
        {
            _collectionPeriods = _collectionPeriodsProvider.Provide();
            _fcsContractAllocations = await _fcsRepository.RetrieveAsync(CancellationToken.None);

            foreach(var fundModel in Enum.GetValues(typeof(FundModel)).Cast<FundModel>())
            {
                SummariseByFundModel(fundModel);
            }
        }

        private async void SummariseByFundModel(FundModel fundModel)
        {
            var fundingStreams = _fundingTypesProvider.Provide().SelectMany(x => x.FundingStreams.Where(y => y.FundModel == fundModel)).ToList();
            var repository = _repositories.FirstOrDefault(r => r.fundModel == fundModel);

            var actuals = await SummariseProviders(fundingStreams, repository);
        }

        private async Task<IList<SummarisedActual>> SummariseProviders(IList<FundingStream> fundingStreams, IProviderRepository repository)
        {
            var pageNumber = 1;

            var providers = await repository.RetrieveProvidersAsync(PageSize, pageNumber, CancellationToken.None);
            var numberOfPages = await repository.RetrieveProviderPageCountAsync(PageSize, CancellationToken.None);

            var actuals = new List<SummarisedActual>();

            while (pageNumber <= numberOfPages)
            {
                foreach (var provider in providers)
                {
                    var contractFundingStreams = new List<FundingStream>();
                    var allocations = new List<IFcsContractAllocation>();
                    foreach(var fs in fundingStreams)
                    {
                        if (_fcsContractAllocations[fs.PeriodCode].Any(x => x.DeliveryUkprn == provider.UKPRN))
                        {
                            contractFundingStreams.Add(fs);
                            allocations.Add(_fcsContractAllocations[fs.PeriodCode].First(x => x.DeliveryUkprn == provider.UKPRN));
                        }
                    }

                    ISummarisationService summarisationService = new SummarisationService();

                    actuals.AddRange(summarisationService.Summarise(contractFundingStreams, provider, allocations, _collectionPeriods));
                }

                pageNumber++;
                providers = await repository.RetrieveProvidersAsync(PageSize, pageNumber, CancellationToken.None);
            }

            return actuals;
        }
    }
}
