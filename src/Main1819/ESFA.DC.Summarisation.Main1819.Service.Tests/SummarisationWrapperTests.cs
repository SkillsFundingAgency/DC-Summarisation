using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.External.FCS.Model;
using ESFA.DC.Summarisation.Data.Input.Interface;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Data.Persist;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Interface;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Main1819.Service.Providers;
using ESFA.DC.Summarisation.Service;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Summarisation.Main1819.Service.Tests
{
    public class SummarisationWrapperTests
    {
        [Theory]
        [InlineData("ILR_FM35", FundModel.FM35)]
        [InlineData("ILR_ALB", FundModel.ALB)]
        public async Task SummmariseProviders(string lineType, FundModel fundModel)
        {
            var cancellationToken = CancellationToken.None;

            var repositoryMock = new Mock<IProviderRepository>();
            repositoryMock.Setup(r => r.RetrieveProvidersAsync(1, 1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetTestProvidersData(1, 1, lineType)));
            repositoryMock.Setup(r => r.RetrieveProvidersAsync(1, 2, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetTestProvidersData(1, 2, lineType)));
            repositoryMock.Setup(r => r.RetrieveProvidersAsync(1, 3, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetTestProvidersData(1, 3, lineType)));

            repositoryMock.Setup(r => r.RetrieveProviderPageCountAsync(1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(3));

            var providerRepositories = new List<IProviderRepository> { repositoryMock.Object };

            var fcsRepositoryMock = new Mock<IFcsRepository>();

            var dataStorePersistenceServiceMock = new Mock<IDataStorePersistenceService>();

            var fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());
            var fundingStreams = fundingTypesProvider.Provide().SelectMany(x => x.FundingStreams.Where(y => y.FundModel == fundModel)).ToList();

            var collectionPeriodsProvider = new CollectionPeriodsProvider(new JsonSerializationService());

            var collectionPeriods = collectionPeriodsProvider.Provide().ToList();

            var summarisationContextMock = new Mock<ISummarisationContext>();

            summarisationContextMock.SetupGet(s => s.CollectionType).Returns("ILR1819");
            summarisationContextMock.SetupGet(s => s.CollectionReturnCode).Returns("R01");

            ISummarisationService summarisationService = new SummarisationService();

            var logger = new Mock<ILogger>();

            var wrapper = new SummarisationWrapper(fcsRepositoryMock.Object, fundingTypesProvider, collectionPeriodsProvider, providerRepositories, summarisationService, dataStorePersistenceServiceMock.Object);
            var result = await wrapper.SummariseProviders(fundingStreams, repositoryMock.Object, collectionPeriods, GetContractAllocations(null), logger.Object, cancellationToken);

            foreach (var ukprn in GetTestProviders())
            {
                if (fundModel == FundModel.FM35)
                {
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1819" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(12);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1819" && s.DeliverableCode == 11 && s.ActualValue != 0).Should().Be(12);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1819" && s.DeliverableCode == 14 && s.ActualValue != 0).Should().Be(12);

                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEBC1819" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(12);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEBC1819" && s.DeliverableCode == 5 && s.ActualValue != 0).Should().Be(12);

                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEB-TOL1819" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(12);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEB-TOL1819" && s.DeliverableCode == 5 && s.ActualValue != 0).Should().Be(12);

                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1819" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(12);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1819" && s.DeliverableCode == 12 && s.ActualValue != 0).Should().Be(12);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1819" && s.DeliverableCode == 15 && s.ActualValue != 0).Should().Be(12);

                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEBC1819" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(12);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEBC1819" && s.DeliverableCode == 6 && s.ActualValue != 0).Should().Be(12);

                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEB-TOL1819" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(12);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEB-TOL1819" && s.DeliverableCode == 6 && s.ActualValue != 0).Should().Be(12);
                }
                else if (fundModel == FundModel.ALB)
                {
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLB1819" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(12);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLBC1819" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(12);

                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLB1819" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(12);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLBC1819" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(12);
                }
            }
        }

        [Theory]
        [InlineData("ILR_FM35", FundModel.FM35)]
        [InlineData("ILR_ALB", FundModel.ALB)]
        public async Task SummmariseProviders_Nocontract(string lineType, FundModel fundModel)
        {
            var cancellationToken = CancellationToken.None;

            var repositoryMock = new Mock<IProviderRepository>();
            repositoryMock.Setup(r => r.RetrieveProvidersAsync(1, 1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetTestProvidersData(1, 1, lineType)));
            repositoryMock.Setup(r => r.RetrieveProvidersAsync(1, 2, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetTestProvidersData(1, 2, lineType)));
            repositoryMock.Setup(r => r.RetrieveProvidersAsync(1, 3, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetTestProvidersData(1, 3, lineType)));

            repositoryMock.Setup(r => r.RetrieveProviderPageCountAsync(1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(3));

            var providerRepositories = new List<IProviderRepository> { repositoryMock.Object };

            var fcsRepositoryMock = new Mock<IFcsRepository>();

            var dataStorePersistenceService = new Mock<IDataStorePersistenceService>();

            var fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());
            var fundingStreams = fundingTypesProvider.Provide().SelectMany(x => x.FundingStreams.Where(y => y.FundModel == fundModel)).ToList();

            var collectionPeriodsProvider = new CollectionPeriodsProvider(new JsonSerializationService());

            var collectionPeriods = collectionPeriodsProvider.Provide().ToList();

            var summarisationContextMock = new Mock<ISummarisationContext>();

            summarisationContextMock.SetupGet(s => s.CollectionType).Returns("ILR1819");
            summarisationContextMock.SetupGet(s => s.CollectionReturnCode).Returns("R01");

            ISummarisationService summarisationService = new SummarisationService();

            var fspCodes = new HashSet<string>();

            var logger = new Mock<ILogger>();

            var wrapper = new SummarisationWrapper(fcsRepositoryMock.Object, fundingTypesProvider, collectionPeriodsProvider, providerRepositories, summarisationService, dataStorePersistenceService.Object);
            var result = await wrapper.SummariseProviders(fundingStreams, repositoryMock.Object, collectionPeriods, GetContractAllocations(fspCodes), logger.Object, cancellationToken);

            foreach (var ukprn in GetTestProviders())
            {
                if (fundModel == FundModel.FM35)
                {
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1819" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(0);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1819" && s.DeliverableCode == 11 && s.ActualValue != 0).Should().Be(0);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1819" && s.DeliverableCode == 14 && s.ActualValue != 0).Should().Be(0);

                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEBC1819" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(0);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEBC1819" && s.DeliverableCode == 5 && s.ActualValue != 0).Should().Be(0);

                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEB-TOL1819" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(0);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEB-TOL1819" && s.DeliverableCode == 5 && s.ActualValue != 0).Should().Be(0);

                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1819" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(0);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1819" && s.DeliverableCode == 12 && s.ActualValue != 0).Should().Be(0);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "APPS1819" && s.DeliverableCode == 15 && s.ActualValue != 0).Should().Be(0);

                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEBC1819" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(0);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEBC1819" && s.DeliverableCode == 6 && s.ActualValue != 0).Should().Be(0);

                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEB-TOL1819" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(0);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "AEB-TOL1819" && s.DeliverableCode == 6 && s.ActualValue != 0).Should().Be(0);
                }
                else if (fundModel == FundModel.ALB)
                {
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLB1819" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(0);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLBC1819" && s.DeliverableCode == 3 && s.ActualValue != 0).Should().Be(0);

                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLB1819" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(0);
                    result.Count(s => s.OrganisationId == $"Org{ukprn}" && s.FundingStreamPeriodCode == "ALLBC1819" && s.DeliverableCode == 2 && s.ActualValue != 0).Should().Be(0);
                }
            }
        }

        private IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> GetContractAllocations(HashSet<string> fspCodes)
        {
            Dictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fspContractAllocations = new Dictionary<string, IReadOnlyCollection<IFcsContractAllocation>>();

            if (fspCodes == null)
                fspCodes = GetFundingStreamPeriodCodes();

            foreach (var fspCode in fspCodes)
            {
                List<IFcsContractAllocation> allocations = new List<IFcsContractAllocation>();

                foreach (var ukprn in GetTestProviders())
                {
                    var fcsContractAllocation = new FcsContractAllocation();

                    fcsContractAllocation.ContractAllocationNumber = $"Alloc{ukprn}";
                    fcsContractAllocation.DeliveryUkprn = ukprn;
                    fcsContractAllocation.DeliveryOrganisation = $"Org{ukprn}";
                    fcsContractAllocation.FundingStreamPeriodCode = fspCode;

                    allocations.Add(fcsContractAllocation);
                }

                fspContractAllocations.Add(fspCode, allocations);
            }

            return fspContractAllocations;
        }

        private IReadOnlyCollection<IProvider> GetTestProvidersData(string lineType)
        {
            List<IProvider> providersData = new List<IProvider>();

            HashSet<int> providers = GetTestProviders();

            foreach (var ukprn in providers)
            {
                providersData.Add(GetTestProviderData(ukprn, lineType));
            }

            return providersData;
        }

        private IReadOnlyCollection<IProvider> GetTestProvidersData(int pageSize, int pageNumber, string lineType)
        {
            var providersData = GetTestProvidersData(lineType).OrderBy(x => x.UKPRN).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return providersData;
        }

        private Provider GetTestProviderData(int ukprn, string lineType)
        {
            return new Provider()
            {
                UKPRN = ukprn,
                LearningDeliveries = GetLearningDeliveries(lineType)
            };
        }

        private List<LearningDelivery> GetLearningDeliveries(string lineType)
        {
            List<LearningDelivery> learningDeliveries = new List<LearningDelivery>();

            foreach (var item in GetFundLines(lineType))
            {
                for (int i = 1; i <= 2; i++)
                {
                    LearningDelivery learningDelivery = new LearningDelivery
                    {
                        LearnRefNumber = "learnref" + i,
                        AimSeqNumber = i,
                        Fundline = item.Fundline,
                        PeriodisedData = item.LineType == "EAS" ? GetPeriodisedDataNoAttributes(1) : GetPeriodisedData(1)
                    };

                    learningDeliveries.Add(learningDelivery);
                }
            }

            return learningDeliveries;
        }

        private List<PeriodisedData> GetPeriodisedData(int lotSize)
        {
            HashSet<string> attributes = GetAllAttributes();

            List<PeriodisedData> periodisedDatas = new List<PeriodisedData>();

            for (int j = 1; j <= lotSize; j++)
            {
                foreach (var item in attributes)
                {
                    PeriodisedData periodisedData = new PeriodisedData()
                    {
                        AttributeName = item,
                        Periods = GetPeriodsData(1)
                    };

                    periodisedDatas.Add(periodisedData);
                }
            }

            return periodisedDatas;
        }

        private List<PeriodisedData> GetPeriodisedDataNoAttributes(int lotSize)
        {
            List<PeriodisedData> periodisedDatas = new List<PeriodisedData>();

            for (int j = 1; j <= lotSize; j++)
            {
                PeriodisedData periodisedData = new PeriodisedData()
                {
                    Periods = GetPeriodsData(1)
                };

                periodisedDatas.Add(periodisedData);
            }

            return periodisedDatas;
        }

        private List<Period> GetPeriodsData(int lotSize)
        {
            List<Period> periods = new List<Period>();
            for (int j = 1; j <= lotSize; j++)
            {
                for (int i = 1; i <= 12; i++)
                {
                    Period period = new Period() { PeriodId = i, Value = i * 10 };
                    periods.Add(period);
                }
            }

            return periods;
        }

        private List<FundLine> GetFundLines(string lineType)
        {
            List<FundLine> fundLines = new List<FundLine>();

            fundLines.Add(new FundLine() { Fundline = "16-18 Apprenticeship", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "16-18 Trailblazer Apprenticeship", LineType = "ILR_TBL" });
            fundLines.Add(new FundLine() { Fundline = "16-18 Traineeships (Adult funded)", LineType = "ILR_FM25" });
            fundLines.Add(new FundLine() { Fundline = "19+ Traineeships (Adult funded)", LineType = "ILR_FM25" });
            fundLines.Add(new FundLine() { Fundline = "19-23 Apprenticeship", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "19-23 Trailblazer Apprenticeship", LineType = "ILR_TBL" });
            fundLines.Add(new FundLine() { Fundline = "19-24 Traineeship", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "19-24 Traineeship (non-procured)", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "19-24 Traineeship (procured from Nov 2017)", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "24+ Apprenticeship", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "24+ Trailblazer Apprenticeship", LineType = "ILR_TBL" });
            fundLines.Add(new FundLine() { Fundline = "Advanced Learner Loans Bursary", LineType = "ILR_ALB" });
            fundLines.Add(new FundLine() { Fundline = "AEB - Other Learning", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "AEB - Other Learning (non-procured)", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "AEB - Other Learning (procured from Nov 2017)", LineType = "ILR_FM35" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 16-18 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 16-18 Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 16-18 Traineeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 19-23 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 19-23 Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 19-24 Traineeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 19-24 Traineeships (From Nov 2017)", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 24+ Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: 24+ Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: Advanced Learner Loans Bursary", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: AEB-Other Learning", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Audit Adjustments: AEB-Other Learning (From Nov 2017)", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 16-18 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 16-18 Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 16-18 Traineeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 19-23 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 19-23 Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 19-24 Traineeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 19-24 Traineeships (From Nov 2017)", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 24+ Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: 24+ Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: AEB-Other Learning", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Authorised Claims: AEB-Other Learning (From Nov 2017)", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Discretionary Bursary: 16-19 Traineeships Bursary", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 16-18 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 16-18 Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 16-18 Traineeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 19-23 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 19-23 Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 19-24 Traineeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 19-24 Traineeships (From Nov 2017)", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 24+ Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: 24+ Trailblazer Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: AEB-Other Learning", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Learning Support: AEB-Other Learning (From Nov 2017)", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Excess Support: Advanced Learner Loans Bursary", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Free Meals: 16-19 Traineeships Bursary", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Learner Support: 16-18 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Learner Support: 19-23 Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Learner Support: 19-24 Traineeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Learner Support: 19-24 Traineeships (From Nov 2017)", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Learner Support: 24+ Apprenticeships", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Vulnerable Bursary: 16-19 Traineeships Bursary", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Princes Trust: AEB-Other Learning", LineType = "EAS" });
            fundLines.Add(new FundLine() { Fundline = "Princes Trust: AEB-Other Learning (From Nov 2017)", LineType = "EAS" });

            if (string.IsNullOrEmpty(lineType))
            {
                return fundLines;
            }
            else
            {
                return fundLines.Where(fl => fl.LineType == lineType).ToList();
            }
        }

        private HashSet<string> GetAllAttributes()
        {
            return new HashSet<string> { "AchievePayment",
                                        "BalancePayment",
                                        "OnProgPayment",
                                        "EmpOutcomePay",
                                        "LearnSuppFundCash",
                                        "LnrOnProgPay",
                                        "AreaUpliftBalPayment",
                                        "AreaUpliftOnProgPayment",
                                        "ALBSupportPayment",
                                        "CoreGovContPayment",
                                        "MathEngBalPayment",
                                        "MathEngOnProgPayment",
                                        "SmallBusPayment",
                                        "YoungAppPayment",
                                        "AchPayment",
                                        "LearnDelCarLearnPilotOnProgPayment",
                                        "LearnDelCarLearnPilotBalPayment" };
        }

        private HashSet<int> GetTestProviders()
        {
            return new HashSet<int> { 10000001, 10000002, 10000003 };
        }

        private HashSet<string> GetFundingStreamPeriodCodes()
        {
            return new HashSet<string>
            {
                "16-18TRN1819",
                "AEBC1819",
                "AEB-TOL1819",
                "ALLB1819",
                "ALLBC1819",
                "APPS1819",
                "CLP1819"
            };
        }
    }
}
