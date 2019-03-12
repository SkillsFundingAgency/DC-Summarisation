using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.output.Model;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DestinationModel = ESFA.DC.Summarisation.Model;

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

        public SummarisationWrapper(IFcsRepository fcsRepository,
                                    IStaticDataProvider<FundingType> fundingTypesProvider,
                                    IStaticDataProvider<CollectionPeriod> collectionPeriodsProvider,
                                    ICollection<IProviderRepository> repositories,
                                    ISummarisationService summarisationService)
        {
            _fundingTypesProvider = fundingTypesProvider;
            _fcsRepository = fcsRepository;
            _repositories = repositories;
            _summarisationService = summarisationService;
            _collectionPeriodsProvider = collectionPeriodsProvider;            
        }

        public async Task Summarise(CancellationToken cancellationToken)
        {
            var collectionPeriods = _collectionPeriodsProvider.Provide();

            var fcsContractAllocations = await _fcsRepository.RetrieveAsync(cancellationToken);

            foreach(var fundModel in Enum.GetValues(typeof(FundModel)).Cast<FundModel>())
            {
                SummariseByFundModel(fundModel, collectionPeriods, fcsContractAllocations, cancellationToken);
            }
        }

        private async void SummariseByFundModel(
            FundModel fundModel,
            IEnumerable<CollectionPeriod> collectionPeriods,
            IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fcsContractAllocations,
            CancellationToken cancellationToken)
        {
            var fundingStreams = _fundingTypesProvider.Provide().SelectMany(x => x.FundingStreams.Where(y => y.FundModel == fundModel)).ToList();
            var repository = _repositories.FirstOrDefault(r => r.FundModel == fundModel);

            var actuals = await SummariseProviders(fundingStreams, repository, collectionPeriods, fcsContractAllocations, cancellationToken);
        }

        public async Task<IList<SummarisedActual>> SummariseProviders(
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

                    foreach(var fs in fundingStreams)
                    {
                        if (fcsContractAllocations.ContainsKey(fs.PeriodCode) &&  fcsContractAllocations[fs.PeriodCode].Any(x => x.DeliveryUkprn == provider.UKPRN))
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
       
    }
}
