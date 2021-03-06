﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Configuration.Interface;
using ESFA.DC.Summarisation.Constants;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.External.FCS.Model;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Data.Persist;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Model;
using ESFA.DC.Summarisation.Service;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Summarisation.ESF.Service.Tests
{
    public class SummarisationWrapperTests
    {
        private const int learningDeliveryRecords = 2;

        private const int contracts = 2;

        private const decimal periodValue = 10;

        [Fact]
        public async Task Summarise_CheckESFMissingRecordsAdded()
        {
            int ukprn = 10001639;
            List<ISummarisationConfigProvider<FundingType>> fundingTypesProviders = new List<ISummarisationConfigProvider<FundingType>>();

            var fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());
            fundingTypesProviders.Add(fundingTypesProvider);

            var summarisationMessageMock = new Mock<ISummarisationMessage>();

            summarisationMessageMock.SetupGet(s => s.Ukprn).Returns(ukprn);
            summarisationMessageMock.SetupGet(s => s.SummarisationTypes).Returns(new List<string>() { "ESF_SuppData" });
            summarisationMessageMock.SetupGet(s => s.CollectionType).Returns("ESF");
            summarisationMessageMock.SetupGet(s => s.ProcessType).Returns("Deliverable");
            summarisationMessageMock.SetupGet(s => s.CollectionReturnCode).Returns("ESF01");
            summarisationMessageMock.SetupGet(s => s.CollectionYear).Returns(2018);
            summarisationMessageMock.SetupGet(s => s.CollectionMonth).Returns(11);

            var loggerMock = new Mock<ILogger>();

            var repositoryMock = new Mock<IProviderRepository>();
            repositoryMock.Setup(r => r.ProvideAsync(ukprn, summarisationMessageMock.Object, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetTestProviderData(ukprn)));

            repositoryMock.Setup(r => r.GetAllProviderIdentifiersAsync(CollectionTypeConstants.ESF.ToString(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetProviderList(ukprn)));

            Func<IProviderRepository> providerRepositoryFunc = () =>
            {
                return repositoryMock.Object;
            };

            var dataStorePersistenceServiceMock = new Mock<IDataStorePersistenceService>();

            CollectionPeriod collectionPeriod01 = new CollectionPeriod()
            {
                ActualsSchemaPeriod = 201810,
                CollectionYear = 1819,
                CollectionMonth = 3,
                CalendarYear = 2018,
                CalendarMonth = 10
            };
            CollectionPeriod collectionPeriod02 = new CollectionPeriod()
            {
                ActualsSchemaPeriod = 201811,
                CollectionYear = 1819,
                CollectionMonth = 4,
                CalendarYear = 2018,
                CalendarMonth = 11
            };

            IEnumerable<CollectionPeriod> collectionPeriods = new List<CollectionPeriod>()
            {
                collectionPeriod01,
                collectionPeriod02
            };

            Dictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fspContractAllocations = new Dictionary<string, IReadOnlyCollection<IFcsContractAllocation>>();
            List<IFcsContractAllocation> fcsContractAllocations = new List<IFcsContractAllocation>()
            {
                new FcsContractAllocation()
                {
                    ContractAllocationNumber = "ESF-2228",
                    DeliveryOrganisation = "ORG0001109",
                    ContractStartDate = 201808,
                    ContractEndDate = 201811,
                    DeliveryUkprn = 10001639,
                    FundingStreamPeriodCode = "ESF1420",
                    Id = 137964
                }
            };
            fspContractAllocations.Add("ESF1420", fcsContractAllocations);
            var fcsRepositoryMock = new Mock<IFcsRepository>();
            fcsRepositoryMock.Setup(r => r.RetrieveAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult((IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>>)fspContractAllocations));

            List<ISummarisationService> summarisationServices = new List<ISummarisationService>()
            {
                new SummarisationDeliverableProcess()
            };

            var collectionPeriodsProvider = new[] { new CollectionPeriodsProvider(new JsonSerializationService()) };

            IEnumerable<Summarisation.Data.Output.Model.SummarisedActual> providerActuals
                = new List<Summarisation.Data.Output.Model.SummarisedActual>()
                {
                    new Summarisation.Data.Output.Model.SummarisedActual()
                    {
                        ActualValue = 5.56M,
                        ActualVolume = 3,
                        CollectionReturnId = 16,
                        ContractAllocationNumber = "ESF-2228",
                        DeliverableCode = 7,
                        FundingStreamPeriodCode = "ESF2320",
                        OrganisationId = "ORG0001109",
                        Period = 201809,
                        PeriodTypeCode = "AY2"
                    }
                };

            var summarisedActualsRepositoryMock = new Mock<ISummarisedActualsProcessRepository>();
            summarisedActualsRepositoryMock.Setup(r => r.GetLastCollectionReturnForCollectionTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetLatestCollectionReturn()));
            summarisedActualsRepositoryMock.Setup(r => r.GetSummarisedActualsForCollectionReturnAndOrganisationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(providerActuals/*GetSummarisedActuals()*/));
            
            var result = await NewSummarisationWrapper(
                fundingTypesProviders: fundingTypesProviders,
                fcsRepository: fcsRepositoryMock.Object,
                summarisedActualsProcessRepository: summarisedActualsRepositoryMock.Object,
                collectionPeriodsProviders: collectionPeriodsProvider,
                summarisationServices: summarisationServices,
                dataStorePersistenceService: dataStorePersistenceServiceMock.Object,
                repositoryFactory: providerRepositoryFunc,
                dataOptions: null,
                logger: loggerMock.Object)
                .Summarise(summarisationMessageMock.Object, CancellationToken.None);

            result.Should().NotBeNullOrEmpty();
            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        private async Task Summarise_CheckNoESFMissingRecordsAdded()
        {
            int ukprn = 1000090;
            List<ISummarisationConfigProvider<FundingType>> fundingTypesProviders = new List<ISummarisationConfigProvider<FundingType>>();

            var fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());
            fundingTypesProviders.Add(fundingTypesProvider);

            var summarisationMessageMock = new Mock<ISummarisationMessage>();

            summarisationMessageMock.SetupGet(s => s.Ukprn).Returns(ukprn);
            summarisationMessageMock.SetupGet(s => s.SummarisationTypes).Returns(new List<string>() { "ESF_SuppData" });
            summarisationMessageMock.SetupGet(s => s.CollectionType).Returns("ESF");
            summarisationMessageMock.SetupGet(s => s.ProcessType).Returns("Deliverable");
            summarisationMessageMock.SetupGet(s => s.CollectionReturnCode).Returns("ESF01");
            summarisationMessageMock.SetupGet(s => s.CollectionYear).Returns(2018);
            summarisationMessageMock.SetupGet(s => s.CollectionMonth).Returns(11);

            var loggerMock = new Mock<ILogger>();

            var repositoryMock = new Mock<IProviderRepository>();
            repositoryMock.Setup(r => r.ProvideAsync(ukprn, summarisationMessageMock.Object, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetTestProviderData(ukprn)));

            repositoryMock.Setup(r => r.GetAllProviderIdentifiersAsync(CollectionTypeConstants.ESF, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetProviderList(ukprn)));

            Func<IProviderRepository> providerRepositoryFunc = () =>
            {
                return repositoryMock.Object;
            };

            var dataStorePersistenceServiceMock = new Mock<IDataStorePersistenceService>();

            CollectionPeriod collectionPeriod01 = new CollectionPeriod()
            {
                ActualsSchemaPeriod = 201810,
                CollectionYear = 1819,
                CollectionMonth = 10,
                CalendarYear = 2018,
                CalendarMonth = 10

            };
            CollectionPeriod collectionPeriod02 = new CollectionPeriod()
            {
                ActualsSchemaPeriod = 201811,
                CollectionYear = 1819,
                CollectionMonth = 11,
                CalendarYear = 2018,
                CalendarMonth = 11
            };

            IEnumerable<CollectionPeriod> collectionPeriods = new List<CollectionPeriod>()
            {
                collectionPeriod01,
                collectionPeriod02
            };

            Dictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fspContractAllocations = new Dictionary<string, IReadOnlyCollection<IFcsContractAllocation>>();
            List<IFcsContractAllocation> fcsContractAllocations = new List<IFcsContractAllocation>()
            {
                new FcsContractAllocation()
                {
                    ContractAllocationNumber = "ESF-0001",
                    DeliveryOrganisation = "ORG0001109",
                    ContractStartDate = 201810,
                    ContractEndDate = 201811,
                    DeliveryUkprn = 10001639,
                    FundingStreamPeriodCode = "ESF1420",
                    Id = 137964
                }
            };
            fspContractAllocations.Add("ESF1420", fcsContractAllocations);
            var fcsRepositoryMock = new Mock<IFcsRepository>();
            fcsRepositoryMock.Setup(r => r.RetrieveAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult((IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>>)fspContractAllocations));

            List<ISummarisationService> summarisationServices = new List<ISummarisationService>()
            {
                new SummarisationDeliverableProcess()
            };

            var collectionPeriodsProvider = new[] { new CollectionPeriodsProvider(new JsonSerializationService()) };

            IEnumerable<Summarisation.Data.Output.Model.SummarisedActual> providerActuals
                = new List<Summarisation.Data.Output.Model.SummarisedActual>()
                {
                    new Summarisation.Data.Output.Model.SummarisedActual()
                    {
                        ActualValue = 5.56M,
                        ActualVolume = 3,
                        CollectionReturnId = 16,
                        ContractAllocationNumber = "ESF-2228",
                        DeliverableCode = 7,
                        FundingStreamPeriodCode = "ESF2320",
                        OrganisationId = "ORG0001109",
                        Period = 201809,
                        PeriodTypeCode = "AY2"
                    }
                };

            var summarisedActualsRepositoryMock = new Mock<ISummarisedActualsProcessRepository>();
            summarisedActualsRepositoryMock.Setup(r => r.GetLastCollectionReturnForCollectionTypeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetLatestCollectionReturn()));
            summarisedActualsRepositoryMock.Setup(r => r.GetSummarisedActualsForCollectionReturnAndOrganisationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(providerActuals/*GetSummarisedActuals()*/));

            var result = await NewSummarisationWrapper(
                fundingTypesProviders: fundingTypesProviders,
                fcsRepository: fcsRepositoryMock.Object,
                summarisedActualsProcessRepository: summarisedActualsRepositoryMock.Object,
                collectionPeriodsProviders: collectionPeriodsProvider,
                summarisationServices: summarisationServices,
                dataStorePersistenceService: dataStorePersistenceServiceMock.Object,
                repositoryFactory: providerRepositoryFunc,
                dataOptions: null,
                logger: loggerMock.Object)
                .Summarise(summarisationMessageMock.Object, CancellationToken.None);

            result.Count().Should().Be(1);
        }
        
        private SummarisationWrapper NewSummarisationWrapper(
            IFcsRepository fcsRepository = null,
            ISummarisedActualsProcessRepository summarisedActualsProcessRepository = null,
            IEnumerable<ISummarisationConfigProvider<FundingType>> fundingTypesProviders = null,
            IEnumerable<ISummarisationConfigProvider<CollectionPeriod>> collectionPeriodsProviders = null,
            IEnumerable<ISummarisationService> summarisationServices = null,
            IDataStorePersistenceService dataStorePersistenceService = null,
            Func<IProviderRepository> repositoryFactory = null,
            ISummarisationDataOptions dataOptions = null,
            ILogger logger = null,
            ISummarisationMessage summarisationMessage = null)
        {
            if (dataOptions == null)
            {
                dataOptions = new SummarisationDataOptions()
                {
                    DataRetrievalMaxConcurrentCalls = "4"
                };
            }

            return new SummarisationWrapper(
                fcsRepository: fcsRepository,
                summarisedActualsProcessRepository: summarisedActualsProcessRepository,
                fundingTypesProviders: fundingTypesProviders,
                collectionPeriodsProviders: collectionPeriodsProviders,
                summarisationServices: summarisationServices,
                dataStorePersistenceService: dataStorePersistenceService,
                repositoryFactory: repositoryFactory,
                dataOptions: dataOptions,
                logger: logger);
        }

        private IReadOnlyCollection<FundingStream> GetESFFundingStreamsData()
        {
            List<ISummarisationConfigProvider<FundingType>> fundingTypesProviders = new List<ISummarisationConfigProvider<FundingType>>();

            var fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());
            fundingTypesProviders.Add(fundingTypesProvider);
            return fundingTypesProviders
                .FirstOrDefault(w => w.CollectionType.Equals(CollectionTypeConstants.ESF.ToString(), StringComparison.OrdinalIgnoreCase))
                .Provide().Where(x => x.SummarisationType.Equals(SummarisationTypeConstants.ESF_SuppData, StringComparison.OrdinalIgnoreCase))
                .SelectMany(fs => fs.FundingStreams)
                .ToList();
        }

        private IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> GetContractAllocations(HashSet<string> fspCodes)
        {
            Dictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fspContractAllocations = new Dictionary<string, IReadOnlyCollection<IFcsContractAllocation>>();

            if (fspCodes == null)
            {
                fspCodes = GetFundingStreamPeriodCodes();
            }

            foreach (var fspCode in fspCodes)
            {
                List<IFcsContractAllocation> allocations = new List<IFcsContractAllocation>();

                foreach (var ukprn in GetTestProviders())
                {
                    var fcsContractAllocation = new FcsContractAllocation
                    {
                        ContractAllocationNumber = $"Alloc{ukprn}",
                        DeliveryUkprn = ukprn,
                        DeliveryOrganisation = $"Org{ukprn}",
                        FundingStreamPeriodCode = fspCode
                    };

                    allocations.Add(fcsContractAllocation);
                }

                fspContractAllocations.Add(fspCode, allocations);
            }

            return fspContractAllocations;
        }

        private CollectionReturn GetLatestCollectionReturn()
        {
            return new CollectionReturn()
            {
                Id = 1,
                CollectionType = "ESF",
                CollectionReturnCode = "ESF01"
            };
         }

        private IEnumerable<Summarisation.Data.Output.Model.SummarisedActual> GetSummarisedActuals()
        {
            return new List<Summarisation.Data.Output.Model.SummarisedActual>()
            {
                new Summarisation.Data.Output.Model.SummarisedActual()
                {
                    OrganisationId = "Org1",
                    UoPCode = null,
                    FundingStreamPeriodCode = "APPS1819",
                    Period = 201801,
                    DeliverableCode = 1,
                    ActualVolume = 0,
                    ActualValue = 100,
                    PeriodTypeCode = "AY",
                    ContractAllocationNumber = "CA-1111"
                },
                new Summarisation.Data.Output.Model.SummarisedActual()
                {
                    OrganisationId = "Org1",
                    UoPCode = string.Empty,
                    FundingStreamPeriodCode = "AEBC1819",
                    Period = 201801,
                    DeliverableCode = 1,
                    ActualVolume = 0,
                    ActualValue = 200,
                    PeriodTypeCode = "AY",
                    ContractAllocationNumber = "CA-2222"
                }
            };
        }

        private IReadOnlyCollection<IProvider> GetTestProvidersData(string lineType)
        {
            List<IProvider> providersData = new List<IProvider>();

            HashSet<int> providers = GetTestProviders();

            foreach (var ukprn in providers)
            {
                providersData.Add(GetTestProviderData(ukprn));
            }

            return providersData;
        }

        private IReadOnlyCollection<IProvider> GetTestProvidersData(int pageSize, int pageNumber, string lineType)
        {
            var providersData = GetTestProvidersData(lineType).OrderBy(x => x.UKPRN).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return providersData;
        }

        private IProvider GetTestProviderData(int ukprn)
        {
            return new Provider()
            {
                UKPRN = ukprn,
                LearningDeliveries = GetLearningDeliveries(ukprn)
            };
        }

        private List<LearningDelivery> GetLearningDeliveries(int ukprn)
        {
            List<LearningDelivery> learningDeliveries = new List<LearningDelivery>()
            {
                new LearningDelivery()
                {
                    ConRefNumber = "ESF-2228",
                    PeriodisedData = GetPeriodisedData()
                }
            };

            for (int i = 1; i <= contracts; i++)
            {
                LearningDelivery learningDelivery = new LearningDelivery()
                {

                    ConRefNumber = $"All{ukprn}-{i}",
                    PeriodisedData = GetPeriodisedData()
                };

                learningDeliveries.Add(learningDelivery);
            }

            return learningDeliveries;
        }

        private List<PeriodisedData> GetPeriodisedData()
        {
            HashSet<string> attributes = GetAllAttributes();

            HashSet<string> deliverableCodes = GetAllDeliverableCodes();

            List<PeriodisedData> periodisedDatas = new List<PeriodisedData>();

            foreach (var deliverableCode in deliverableCodes)
            {
                PeriodisedData periodisedData = new PeriodisedData()
                {
                    DeliverableCode = deliverableCode,
                    Periods = GetPeriodsData(2)
                };

                periodisedDatas.Add(periodisedData);
            }

            return periodisedDatas;
        }

        private List<Period> GetPeriodsData(int lotSize)
        {
            List<Period> periods = new List<Period>();
            for (int i = 1; i <= lotSize; i++)
            {
                foreach (var collectionPeriod in GetCollectionPeriods())
                {
                    Period period = new Period()
                    {
                        //PeriodId = collectionPeriod.Period,
                        CalendarMonth = collectionPeriod.CalendarMonth,
                        CalendarYear = collectionPeriod.CalendarYear,
                        Value = periodValue,
                        Volume = 1
                    };
                    periods.Add(period);
                }

            }

            return periods;
        }

        private List<FundingType> GetFundingTypes()
        {
            FundingTypesProvider fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());

            return fundingTypesProvider.Provide().ToList();
        }

        private List<CollectionPeriod> GetCollectionPeriods()
        {
            var collectionPeriodsProvider = new CollectionPeriodsProvider(new JsonSerializationService());

            return collectionPeriodsProvider.Provide().ToList();
        }
        
        private HashSet<string> GetAllAttributes()
        {
            return new HashSet<string> { "StartEarnings",
                                    "AchievementEarnings",
                                    "AdditionalProgCostEarnings",
                                    "ProgressionEarnings" };
        }

        private HashSet<int> GetTestProviders()
        {
            return new HashSet<int> { 10000001, 10000002, 10000003, 10001639 };
        }

        private HashSet<string> GetAllDeliverableCodes()
        {
            return new HashSet<string> { "AC01",
                                        "CG01",
                                        "CG02",
                                        "FS01",
                                        "NR01",
                                        "PG01",
                                        "PG02",
                                        "PG03",
                                        "PG04",
                                        "PG05",
                                        "PG06",
                                        "RQ01",
                                        "SD01",
                                        "SD02",
                                        "SD03",
                                        "SD04",
                                        "SD05",
                                        "SD06",
                                        "SD07",
                                        "SD08",
                                        "SD09",
                                        "SD10",
                                        "ST01",
                                        "SU01",
                                        "SU02",
                                        "SU03",
                                        "SU04",
                                        "SU05",
                                        "SU11",
                                        "SU12",
                                        "SU13",
                                        "SU14",
                                        "SU15",
                                        "SU21",
                                        "SU22",
                                        "SU23",
                                        "SU24"
            };
        }

        private IList<int> GetProviderList(int ukprn)
        {
            return new List<int> { ukprn };
        }

        private HashSet<string> GetFundingStreamPeriodCodes()
        {
            return new HashSet<string>
            {
                "ESF1420"
            };
        }
    }
}
