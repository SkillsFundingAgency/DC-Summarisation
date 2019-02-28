using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ESFA.DC.Summarisation.Main1819.Service
{
    public class SummarisationWrapper
    {
        private readonly List<IProvider> _providers;
        private readonly List<FundingType> _fundingTypes;
        private readonly IFcsRepository _fcsRepository;

        private IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> _fcsContractAllocations;

        public SummarisationWrapper(List<FundingType> fundingTypes,
                                    IFcsRepository fcsRepository,
                                    List<IProvider> providers)
        {
            _providers = providers;
            _fundingTypes = fundingTypes;
            _fcsRepository = fcsRepository;
        }

        public async void Summarise()
        {
            _fcsContractAllocations = await _fcsRepository.RetrieveAsync(CancellationToken.None);
            foreach (var fundingType in _fundingTypes)
            {
                foreach (var provider in _providers)
                {
                    if (_fcsContractAllocations["fspc"].Any(x => x.DeliveryUkprn == provider.UKPRN))
                    {
                        ISummarisationService summarisationService = new SummarisationService();

                        summarisationService.Summarise(fundingType, provider);
                    }
                }
            }
        }

      
    }
}
