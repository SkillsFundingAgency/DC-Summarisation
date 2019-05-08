using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.Summarisation;
using ESFA.DC.Summarisation.Main1819.Data.Repository;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ESFA.DC.Summarisation.Data.Tests.Repository
{
    public class TblRepositoryTests
    {
        [Fact]
        public void SummarisationType_Check()
        {
            NewProvider().SummarisationType.Should().Be("Main1819_TBL");
        }

        [Fact]
        public void SummarisationType_CheckWithConstantValue()
        {
            NewProvider().SummarisationType.Should().Be(nameof(Configuration.Enum.SummarisationType.Main1819_TBL));
        }

        [Fact]
        public void CollectionType_Check()
        {
            NewProvider().SummarisationType.Should().Be("ILR1819");
        }

        [Fact]
        public void CollectionType_CheckWithConstantValue()
        {
            NewProvider().SummarisationType.Should().Be(nameof(Configuration.Enum.CollectionType.ILR1819));
        }

        private TblProvider NewProvider(IIlr1819RulebaseContext ilrContext = null)
        {
            return new TblProvider(ilrContext);
        }
    }
}
