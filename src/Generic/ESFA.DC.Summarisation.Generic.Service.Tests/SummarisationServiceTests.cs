using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service.Model;
using ESFA.DC.Summarisation.Service.Model.Fcs;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Summarisation.Generic.Service.Tests
{
    public class SummarisationServiceTests
    {
        [Fact]
        public async Task Summarise()
        {
            var cancellationToken = CancellationToken.None;
            var summarisationMessage = Mock.Of<ISummarisationMessage>();
            var inputActuals = TestSummarisedActuals();
            var contractAllocations = new List<FcsContractAllocation>
            {
                new FcsContractAllocation
                {
                    DeliveryUkprn = 1,
                    DeliveryOrganisation = "Org1",
                    FundingStreamPeriodCode = "FSPCode1",
                },
                new FcsContractAllocation
                {
                    DeliveryUkprn = 2,
                    DeliveryOrganisation = "Org2",
                    FundingStreamPeriodCode = "FSPCode2",
                },
            };

            var expectedActuals = new List<SummarisedActual>
            {
                 new SummarisedActual
                 {
                     CollectionReturnId = 101,
                     ContractAllocationNumber = "Contract1",
                     FundingStreamPeriodCode = "FSPCode1",
                     DeliverableCode = 1,
                     Period = 1,
                     PeriodTypeCode = "PType1",
                     OrganisationId = "Org1",
                     ActualValue = 1,
                     ActualVolume = 1,
                 },
                 new SummarisedActual
                 {
                     CollectionReturnId = 101,
                     ContractAllocationNumber = "Contract1",
                     FundingStreamPeriodCode = "FSPCode1",
                     DeliverableCode = 1,
                     Period = 1,
                     PeriodTypeCode = "PType1",
                     OrganisationId = "Org2",
                     ActualValue = 1,
                     ActualVolume = 1,
                 },
            };

            var result = NewService().Summarise(contractAllocations, inputActuals);

            result.Should().BeEquivalentTo(expectedActuals);
        }

        private static ICollection<SummarisedActual> TestSummarisedActuals()
        {
            return new List<SummarisedActual>
            {
                 new SummarisedActual
                 {
                     CollectionReturnId = 101,
                     ContractAllocationNumber = "Contract1",
                     FundingStreamPeriodCode = "FSPCode1",
                     DeliverableCode = 1,
                     Period = 1,
                     PeriodTypeCode = "PType1",
                     OrganisationId = "1",
                     ActualValue = 1,
                     ActualVolume = 1,
                 },
                 new SummarisedActual
                 {
                     CollectionReturnId = 101,
                     ContractAllocationNumber = "Contract1",
                     FundingStreamPeriodCode = "FSPCode1",
                     DeliverableCode = 1,
                     Period = 1,
                     PeriodTypeCode = "PType1",
                     OrganisationId = "2",
                     ActualValue = 1,
                     ActualVolume = 1,
                 },
                 new SummarisedActual
                 {
                     CollectionReturnId = 101,
                     ContractAllocationNumber = "Contract2",
                     FundingStreamPeriodCode = "FSPCode2",
                     DeliverableCode = 2,
                     Period = 2,
                     PeriodTypeCode = "PType2",
                     OrganisationId = "3",
                     ActualValue = 2,
                     ActualVolume = 2,
                 },
            };
        }

        private static SummarisationService NewService()
        {
            return new SummarisationService(Mock.Of<ILogger>());
        }
    }
}
