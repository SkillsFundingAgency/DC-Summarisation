using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.Repository.Interface;
using ESFA.DC.Summarisation.Main1819.Service.Providers;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Data.Input.Interface;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Summarisation.Data.External.FCS.Interface;
using ESFA.DC.Summarisation.Data.External.FCS.Model;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Serialization.Json;
using FluentAssertions;

namespace ESFA.DC.Summarisation.Main1819.Service.Tests
{

    public class SummarisationWrapperTests
    {
        [Fact]
        public async Task SummmariseProviders()
        {
            var cancellationToken = CancellationToken.None;

            var fm35RepositoryMock = new Mock<IProviderRepository>();
            fm35RepositoryMock.Setup(r => r.RetrieveProvidersAsync(1, 1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetTestProvidersData(1,1)));
            fm35RepositoryMock.Setup(r => r.RetrieveProvidersAsync(1, 2, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetTestProvidersData(1, 2)));
            fm35RepositoryMock.Setup(r => r.RetrieveProvidersAsync(1, 3, It.IsAny<CancellationToken>())).Returns(Task.FromResult(GetTestProvidersData(1, 3)));

            fm35RepositoryMock.Setup(r => r.RetrieveProviderPageCountAsync(1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(3));

            var providerRepositories = new List<IProviderRepository>();
            providerRepositories.Add(fm35RepositoryMock.Object);

            var fcsRepositoryMock = new Mock<IFcsRepository>();
            //fcsRepositoryMock.Setup(f => f.RetrieveAsync(cancellationToken)).Returns(Task.FromResult(GetContractAllocations()));

            var fundingTypesProvider = new FundingTypesProvider(new JsonSerializationService());
            var fundingStreams = fundingTypesProvider.Provide().SelectMany(x => x.FundingStreams.Where(y => y.FundModel == FundModel.FM35)).ToList();

            var collectionPeriodsProvider = new CollectionPeriodsProvider(new JsonSerializationService());

            var collectionPeriods = collectionPeriodsProvider.Provide().ToList();

            ISummarisationService summarisationService = new SummarisationService();

            var wrapper = new SummarisationWrapper(fcsRepositoryMock.Object, fundingTypesProvider, collectionPeriodsProvider, providerRepositories, summarisationService);
            var result = await wrapper.SummariseProviders(fundingStreams, fm35RepositoryMock.Object, collectionPeriods, GetContractAllocations(), CancellationToken.None);

            //var wrapperMock = new Mock<ISummarisationWrapper>();
            //awatiwrapperMock.Setup(w => w.SummariseProviders(fundingStreams, fm35RepositoryMock.Object, collectionPeriods, GetContractAllocations()));

            result.Count.Should().BeGreaterThan(1);

        }

        private IReadOnlyDictionary<string, IReadOnlyCollection<IFcsContractAllocation>> GetContractAllocations()
        {
            Dictionary<string, IReadOnlyCollection<IFcsContractAllocation>> fspContractAllocations = new Dictionary<string, IReadOnlyCollection<IFcsContractAllocation>>();

            foreach (var fspCode in GetFundingStreamPeriodCodes())
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

        private IReadOnlyCollection<IProvider> GetTestProvidersData()
        {
            List<IProvider> providersData = new List<IProvider>();

            HashSet<int> providers = GetTestProviders();

            foreach (var item in providers)
            {
                providersData.Add(GetTestProviderData(item));
            }

            return providersData;
        }

        private IReadOnlyCollection<IProvider> GetTestProvidersData(int pageSize, int pageNumber)
        {
            var providersData = GetTestProvidersData().OrderBy(x => x.UKPRN).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return providersData;
        }

        private Provider GetTestProviderData(int ukprn)
        {
            return new Provider()
            {
                UKPRN = ukprn,
                LearningDeliveries = GetLearningDeliveries()
            };
        }
        private List<ILearningDelivery> GetLearningDeliveries()
        {
            List<ILearningDelivery> learningDeliveries = new List<ILearningDelivery>();

            foreach (var item in GetFundLines("ILR_FM35"))
            {
                for (int i = 1; i <= 2; i++)
                {
                    LearningDelivery learningDelivery = new LearningDelivery()
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


        private List<IPeriodisedData> GetPeriodisedData(int lotSize)
        {
            HashSet<string> attributes = GetAllAttributes();

            List<IPeriodisedData> periodisedDatas = new List<IPeriodisedData>();

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

        private List<IPeriodisedData> GetPeriodisedDataNoAttributes(int lotSize)
        {
            List<IPeriodisedData> periodisedDatas = new List<IPeriodisedData>();

            for (int j = 1; j <= lotSize; j++)
            {
                IPeriodisedData periodisedData = new PeriodisedData()
                {
                    Periods = GetPeriodsData(1)
                };

                periodisedDatas.Add(periodisedData);
            }

            return periodisedDatas;
        }

        private List<IPeriod> GetPeriodsData(int lotSize)
        {
            List<IPeriod> periods = new List<IPeriod>();
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
                return fundLines.Where(fl => fl.LineType == "ILR_FM35").ToList();
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
