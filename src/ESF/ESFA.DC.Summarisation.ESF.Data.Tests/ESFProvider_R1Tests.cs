using ESFA.DC.ESF.Database.EF;
using MockQueryable.Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.Summarisation.ESF.Data.Tests
{
    
    public class ESFProvider_R1Tests
    {

        [Fact]
        public async Task ProvideUkprnsAsync_Check()
        {
            var sourceFiles = new List<SourceFile>()
            {
                new SourceFile() { SourceFileId = 1, UKPRN = "1000001", ConRefNumber = "ConRef-1-1" },
                new SourceFile() { SourceFileId = 2, UKPRN = "1000001", ConRefNumber = "ConRef-1-2" },
                new SourceFile() { SourceFileId = 3, UKPRN = "1000002", ConRefNumber = "ConRef-2-1" },
                new SourceFile() { SourceFileId = 4, UKPRN = "1000002", ConRefNumber = "ConRef-2-2" },

            }.AsQueryable().BuildMock();

            var suppData = new List<SupplementaryData>()
            {
                new SupplementaryData() {SourceFileId = 1, ConRefNumber = "ConRef-1-1", DeliverableCode = "D01", CalendarMonth = 1, CalendarYear = 2018, Value = 10}

            }.AsQueryable().BuildMock();
           
        }
    }
}
