using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Summarisation.Data.Persist.Mapper;
using ESFA.DC.Summarisation.Model;
using FluentAssertions;
using SummarisedActual = ESFA.DC.Summarisation.Data.Output.Model.SummarisedActual;
using Xunit;

namespace ESFA.DC.Summarisation.Data.Persist.Tests.MapperTests
{
    public class SummarisedActualsMapperTests
    {
        [Fact]
        public void SummarisedActualsMapper()
        {
            var collectionReturn = new CollectionReturn { CollectionReturnCode = "R01", CollectionType = "ILR", Id = 1 };
            var summarisedActuals = new List<SummarisedActual>
            {
                new SummarisedActual
                {
                    ActualValue = 1.00m,
                    ActualVolume = 1,
                    ContractAllocationNumber = "ABC001",
                    DeliverableCode = 1,
                    FundingStreamPeriodCode = "FSPC001",
                    OrganisationId = "Org001",
                    PeriodTypeCode = "R01",
                    Period = 1,
                },
                new SummarisedActual
                {
                    ActualValue = 2.00m,
                    ActualVolume = 2,
                    ContractAllocationNumber = "ABC002",
                    DeliverableCode = 5,
                    FundingStreamPeriodCode = "FSPC002",
                    OrganisationId = "Org002",
                    PeriodTypeCode = "R02",
                    Period = 5,
                }
            };

            var mappedActuals = Mapper().MapSummarisedActuals(summarisedActuals, collectionReturn);

            mappedActuals.Count().Should().Be(2);
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org001").CollectionReturnId.Should().Be(1);
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org001").ActualValue.Should().Be(1.00m);
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org001").ActualVolume.Should().Be(1);
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org001").ContractAllocationNumber.Should().Be("ABC001");
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org001").DeliverableCode.Should().Be(1);
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org001").FundingStreamPeriodCode.Should().Be("FSPC001");
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org001").PeriodTypeCode.Should().Be("R01");
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org001").Period.Should().Be(1);

            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org002").CollectionReturnId.Should().Be(1);
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org002").ActualValue.Should().Be(2.00m);
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org002").ActualVolume.Should().Be(2);
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org002").ContractAllocationNumber.Should().Be("ABC002");
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org002").DeliverableCode.Should().Be(5);
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org002").FundingStreamPeriodCode.Should().Be("FSPC002");
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org002").PeriodTypeCode.Should().Be("R02");
            mappedActuals.FirstOrDefault(x => x.OrganisationId == "Org002").Period.Should().Be(5);
        }

        private SummarisedActualsMapper Mapper() => new SummarisedActualsMapper();
    }
}
