using System;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Apps.Interfaces;
using ESFA.DC.Summarisation.Apps.Model;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using ESFA.DC.Summarisation.Apps.Model.Config;

namespace ESFA.DC.Summarisation.Apps.Service
{
    public class ProviderSummarisationService : IProviderSummarisationService<LearningProvider>
    {
        private readonly ISummarisationService _summarisationService;
        private readonly ILogger _logger;
        private readonly IProviderContractsService _providerContractsService;
        private readonly IProviderFundingDataRemovedService _providerFundingDataRemovedService;

        public ProviderSummarisationService(
            ISummarisationService summarisationService,
            ILogger logger,
            IProviderContractsService providerContractsService,
            IProviderFundingDataRemovedService providerFundingDataRemovedService)
        {
            _summarisationService = summarisationService;
            _logger = logger;
            _providerContractsService = providerContractsService;
            _providerFundingDataRemovedService = providerFundingDataRemovedService;
        }

        public async Task<ICollection<SummarisedActual>> Summarise(LearningProvider providerData, ICollection<CollectionPeriod> collectionPeriods, ICollection<FundingType> fundingTypes, ICollection<FcsContractAllocation> contractAllocations, ISummarisationMessage summarisationMessage, CancellationToken cancellationToken)
        {
            var providerActuals = new List<SummarisedActual>();

            foreach (var summarisationType in summarisationMessage.SummarisationTypes)
            {
                _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {providerData.UKPRN}, Fundmodel {summarisationType} Start");

                var fundingStreams = fundingTypes?
                    .Where(x => x.SummarisationType.Equals(summarisationType, StringComparison.OrdinalIgnoreCase))
                    .SelectMany(fs => fs.FundingStreams)
                    .ToList();

                var providerfundingstreamsContracts = await _providerContractsService.GetProviderContracts(providerData.UKPRN, fundingStreams, contractAllocations, cancellationToken);

                var summarisedData = _summarisationService.Summarise(providerfundingstreamsContracts.FundingStreams, providerData, providerfundingstreamsContracts.FcsContractAllocations, collectionPeriods, summarisationMessage);

                providerActuals.AddRange(summarisedData);

                _logger.LogInfo($"Summarisation Wrapper: Summarising Data of UKPRN: {providerData.UKPRN}, Fundmodel {summarisationType} End");
            }

            _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed  Rule UKPRN: {providerData.UKPRN} Start");

            var organisationId = contractAllocations.Where(w => w.DeliveryUkprn == providerData.UKPRN).Select(s => s.DeliveryOrganisation).FirstOrDefault();

            var actualsToCarry = await _providerFundingDataRemovedService.FundingDataRemovedAsync(organisationId, providerActuals, summarisationMessage, cancellationToken);

            var fundingStreamPeriodCodesNotRequiredForActuals =
                GetFundingStreamsNotRequiredForCarryOver(summarisationMessage.CollectionYear,
                    summarisationMessage.CollectionMonth);

            var filteredActualsToCarry = actualsToCarry.Where(x => !fundingStreamPeriodCodesNotRequiredForActuals.Contains(x.FundingStreamPeriodCode)).ToList();

            providerActuals.AddRange(filteredActualsToCarry);

            _logger.LogInfo($"Summarisation Wrapper: Funding Data Removed  Rule UKPRN: {providerData.UKPRN} End");


            return providerActuals;
        }

        private IEnumerable<string> GetFundingStreamsNotRequiredForCarryOver(int collectionYear, int collectionMonth)
        {
            if (collectionYear == AcademicYearConstants.AcademicYear_2021 && (collectionMonth != CollectionMonthConstants.CollectionMonth_2 || collectionMonth != CollectionMonthConstants.CollectionMonth_3))
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    FundingStreamConstants.Levy1799,
                    FundingStreamConstants.NonLevy2019,
                    FundingStreamConstants.NonLevy_APPS1920
                };
            }

            return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                FundingStreamConstants.Levy1799,
                FundingStreamConstants.NonLevy2019
            };
        }
    }
}
