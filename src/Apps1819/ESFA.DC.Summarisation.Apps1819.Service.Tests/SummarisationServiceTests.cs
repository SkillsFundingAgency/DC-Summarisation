using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Service;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ESFA.DC.Summarisation.Apps1819.Service.Tests
{
    public class SummarisationServiceTests
    {
        private const int periodsToGenerate = 5;
        private const decimal amount = 10m;

        [Fact]
        public void SummariseByPeriods()
        {
            var task = new SummarisationFundlineProcess();

            var result = task.SummarisePeriods(GetPeriodsData());

            result.Count().Should().Be(12);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(5 * item.Period * amount);
            }
        }

        private List<Period> GetPeriodsData()
        {
            List<Period> periods = new List<Period>();

            for (int j = 1; j <= periodsToGenerate; j++)
            {
                for (int i = 1; i <= 12; i++)
                {
                    Period period = new Period() { PeriodId = i, Value = i * amount };
                    periods.Add(period);
                }
            }

            return periods;
        }
    }
}
