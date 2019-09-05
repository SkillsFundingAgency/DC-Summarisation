﻿using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Configuration;
using ESFA.DC.Summarisation.Data.External.FCS.Model;
using ESFA.DC.Summarisation.Data.Input.Model;
using ESFA.DC.Summarisation.Interfaces;
using ESFA.DC.Summarisation.Service;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.Summarisation.Apps1920.Service.Tests
{
    public class SummarisationServiceTests
    {
        private const int periodsToGenerate = 5;
        private const decimal amount = 10m;
        private const int ukprn = 10000001;
        private const int learningDeliveryRecords = 2;

        [Fact]
        public void SummariseByPeriods()
        {
            var task = new SummarisationPaymentsProcess();

            var collectionPeriods = GetCollectionPeriods().Where(w => w.CollectionYear == 1920);

            var result = task.SummarisePeriods(GetPeriodsData(), collectionPeriods);

            result.Count().Should().Be(12);

            foreach (var item in result)
            {
                item.ActualValue.Should().Be(5 * amount);
            }
        }

        [Theory]
        [InlineData(1, "LEVY1799", 2, "1,5", "1,2,3")]
        [InlineData(1, "LEVY1799", 3, "2", "1,2,3")]
        [InlineData(1, "LEVY1799", 4, "4", "15")]
        [InlineData(1, "LEVY1799", 5, "4", "5,7,8,9,10,11,12,13,14")]
        [InlineData(1, "LEVY1799", 6, "4", "4,6")]
        [InlineData(1, "LEVY1799", 13, "4", "16")]
        [InlineData(1, "LEVY1799", 8, "1,5", "1,2,3")]
        [InlineData(1, "LEVY1799", 9, "2", "1,2,3")]
        [InlineData(1, "LEVY1799", 10, "4", "15")]
        [InlineData(1, "LEVY1799", 11, "4", "5,7,8,9,10,11,12,13,14")]
        [InlineData(1, "LEVY1799", 12, "4", "4,6")]
        [InlineData(1, "LEVY1799", 14, "4", "16")]

        [InlineData(1, "NONLEVY2019", 2, "5", "1,2,3")]
        [InlineData(1, "NONLEVY2019", 3, "2", "1,2,3")]
        [InlineData(1, "NONLEVY2019", 4, "4", "15")]
        [InlineData(1, "NONLEVY2019", 5, "4", "5,7,8,9,10,11,12,13,14")]
        [InlineData(1, "NONLEVY2019", 6, "4", "4,6")]
        [InlineData(1, "NONLEVY2019", 7, "4", "16")]
        [InlineData(1, "NONLEVY2019", 9, "5", "1,2,3")]
        [InlineData(1, "NONLEVY2019", 10, "2", "1,2,3")]
        [InlineData(1, "NONLEVY2019", 11, "4", "15")]
        [InlineData(1, "NONLEVY2019", 12, "4", "5,7,8,9,10,11,12,13,14")]
        [InlineData(1, "NONLEVY2019", 13, "4", "4,6")]
        [InlineData(1, "NONLEVY2019", 14, "4", "16")]
        public void SummariseByFundingStream(int apprenticeshipContractType, string fspCode, int dlc, string fundingStreamsCSV, string transactionTypesCSV)
        {
            var fundingTypes = GetFundingTypes();

            FundingStream fundingStream = fundingTypes.SelectMany(ft => ft.FundingStreams).Where(fs => fs.PeriodCode.Equals(fspCode, StringComparison.OrdinalIgnoreCase) && fs.DeliverableLineCode == dlc).First();

            int ilrFundlineCount = fundingStream.FundLines.Count(fl => fl.LineType.Equals("ILR", StringComparison.OrdinalIgnoreCase));

            int easFundlineCount = fundingStream.FundLines.Count(fl => fl.LineType.Equals("EAS", StringComparison.OrdinalIgnoreCase));

            List<int> fundingStreams = fundingStreamsCSV.Split(',').Select(int.Parse).ToList();

            List<int> transactionTypes = transactionTypesCSV.Split(',').Select(int.Parse).ToList();

            var summarisationMessageMock = new Mock<ISummarisationMessage>();

            summarisationMessageMock.SetupGet(s => s.CollectionYear).Returns(1920);
            summarisationMessageMock.SetupGet(s => s.CollectionMonth).Returns(1);

            var task = new SummarisationPaymentsProcess();

            var results = task.Summarise(fundingStream, GetTestProvider(apprenticeshipContractType, fundingStreams, transactionTypes), GetContractAllocation(), GetCollectionPeriods(), summarisationMessageMock.Object).OrderBy(x => x.Period).ToList();

            results.Count().Should().Be(1);

            foreach (var item in results)
            {
                decimal ilrActualValue = learningDeliveryRecords * fundingStreams.Count() * ilrFundlineCount * transactionTypes.Count() * periodsToGenerate * amount;

                decimal easActualValue = learningDeliveryRecords * easFundlineCount * periodsToGenerate * amount;

                decimal actualValue = ilrActualValue + easActualValue;

                item.ActualValue.Should().Be(actualValue);
            }
        }

        [Theory]
        [InlineData(2, "APPS1920", 8, "2,4", "1,2,3,5,7,8,9,10,11,12,13,14")]
        [InlineData(2, "APPS1920", 9, "4", "15")]
        [InlineData(2, "APPS1920", 10, "4", "4,6")]
        [InlineData(2, "APPS1920", 22, "2,4", "1,2,3,5,7,8,9,10,11,12,13,14")]
        [InlineData(2, "APPS1920", 23, "4", "15")]
        [InlineData(2, "APPS1920", 24, "4", "4,6")]

        [InlineData(2, "16-18NLAP2018", 2, "2,4", "1,2,3,5,7,8,9,10,11,12,13,14")]
        [InlineData(2, "16-18NLAP2018", 3, "4", "15")]
        [InlineData(2, "16-18NLAP2018", 4, "4", "4,6")]
        [InlineData(2, "16-18NLAP2018", 5, "4", "16")]
        [InlineData(2, "ANLAP2018", 2, "2,4", "1,2,3,5,7,8,9,10,11,12,13,14")]
        [InlineData(2, "ANLAP2018", 3, "4", "15")]
        [InlineData(2, "ANLAP2018", 4, "4", "4,6")]
        [InlineData(2, "ANLAP2018", 5, "4", "16")]
        public void SummariseByFundingStream_NonLevy(int apprenticeshipContractType, string fspCode, int dlc, string fundingSourcesCSV, string transactionTypesCSV)
        {
            var fundingTypes = GetFundingTypes();

            FundingStream fundingStream = fundingTypes.SelectMany(ft => ft.FundingStreams).Where(fs => fs.PeriodCode.Equals(fspCode, StringComparison.OrdinalIgnoreCase) && fs.DeliverableLineCode == dlc).First();

            int ilrFundlineCount = fundingStream.FundLines.Count(fl => fl.LineType.Equals("ILR", StringComparison.OrdinalIgnoreCase));

            int easFundlineCount = fundingStream.FundLines.Count(fl => fl.LineType.Equals("EAS", StringComparison.OrdinalIgnoreCase));

            List<int> fundingSources = fundingSourcesCSV.Split(',').Select(int.Parse).ToList();

            List<int> transactionTypes = transactionTypesCSV.Split(',').Select(int.Parse).ToList();

            var summarisationMessageMock = new Mock<ISummarisationMessage>();

            summarisationMessageMock.SetupGet(s => s.CollectionYear).Returns(1920);
            summarisationMessageMock.SetupGet(s => s.CollectionMonth).Returns(1);

            var task = new SummarisationPaymentsProcess();

            var results = task.Summarise(fundingStream, GetTestProvider(apprenticeshipContractType, fundingSources, transactionTypes), GetContractAllocation(), GetCollectionPeriods(), summarisationMessageMock.Object).OrderBy(x => x.Period).ToList();

            results.Count().Should().Be(12);

            foreach (var item in results)
            {
                decimal ilrActualValue = learningDeliveryRecords * fundingSources.Count() * ilrFundlineCount * transactionTypes.Count() * periodsToGenerate * amount;

                decimal easActualValue = learningDeliveryRecords * easFundlineCount * periodsToGenerate * amount;

                decimal actualValue = ilrActualValue + easActualValue;

                item.ActualValue.Should().Be(actualValue);
            }
        }

        private Provider GetTestProvider(int apprenticeshipContratType, List<int> fundingSources, List<int> transactionTypes)
        {
            return new Provider()
            {
                UKPRN = ukprn,
                LearningDeliveries = GetLearningDeliveries(apprenticeshipContratType, fundingSources, transactionTypes),
            };
        }

        private List<LearningDelivery> GetLearningDeliveries(int apprenticeshipContratType, List<int> fundingSources, List<int> transactionTypes)
        {
            List<LearningDelivery> learningDeliveries = new List<LearningDelivery>();

            foreach (var item in GetFundLines())
            {
                for (int i = 1; i <= learningDeliveryRecords; i++)
                {
                    LearningDelivery learningDelivery = new LearningDelivery()
                    {
                        Fundline = item.Fundline,
                        PeriodisedData = item.LineType == "ILR" ? GetPeriodisedData(apprenticeshipContratType, fundingSources, transactionTypes) : GetPeriodisedData_EAS(),
                    };

                    learningDeliveries.Add(learningDelivery);
                }
            }

            return learningDeliveries;
        }

        private List<PeriodisedData> GetPeriodisedData(int apprenticeshipContratType, List<int> fundingSources, List<int> transactionTypes)
        {
            List<PeriodisedData> periodisedDatas = new List<PeriodisedData>();

            foreach (var fundingSource in fundingSources)
            {
                foreach (var transactionType in transactionTypes)
                {
                    PeriodisedData periodisedData = new PeriodisedData()
                    {
                        ApprenticeshipContractType = apprenticeshipContratType,
                        FundingSource = fundingSource,
                        TransactionType = transactionType,
                        Periods = GetPeriodsData()
                    };

                    periodisedDatas.Add(periodisedData);
                }
            }

            return periodisedDatas;
        }

        private List<PeriodisedData> GetPeriodisedData_EAS()
        {
            List<PeriodisedData> periodisedDatas = new List<PeriodisedData>();

            PeriodisedData periodisedData = new PeriodisedData()
            {
                Periods = GetPeriodsData(),
            };

            periodisedDatas.Add(periodisedData);

            return periodisedDatas;
        }

        private List<Period> GetPeriodsData()
        {
            List<Period> periods = new List<Period>();

            for (int j = 1; j <= periodsToGenerate; j++)
            {
                foreach (var item in GetCollectionPeriods())
                {
                    Period period = new Period() { PeriodId = item.Period, CollectionYear = item.CollectionYear, CollectionMonth = item.CollectionMonth, Value = amount };
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

        private IEnumerable<int> GetApprenticeshipContratTypes()
        {
            return Enumerable.Range(1, 2);
        }

        private IEnumerable<int> GetFundingSources()
        {
            List<int> fundingSources = new List<int> { 1, 2, 4, 5 };

            return fundingSources;
        }

        private IEnumerable<int> GetTransationTypes()
        {
            return Enumerable.Range(1, 16);
        }

        private List<FundLine> GetFundLines()
        {
            List<FundLine> fundLines = new List<FundLine>
            {
                new FundLine { Fundline = "16-18 Apprenticeship (Employer on App Service) Levy funding", LineType = "ILR" },
                new FundLine { Fundline = "19+ Apprenticeship (Employer on App Service) Levy funding", LineType = "ILR" },
                new FundLine { Fundline = "16-18 Apprenticeship (Employer on App Service) Non-Levy funding", LineType = "ILR" },
                new FundLine { Fundline = "19+ Apprenticeship (Employer on App Service) Non-Levy funding", LineType = "ILR" },
                new FundLine { Fundline = "16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured)", LineType = "ILR" },
                new FundLine { Fundline = "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)", LineType = "ILR" },
                new FundLine { Fundline = "16-18 Apprenticeship Non-Levy Contract (procured)", LineType = "ILR" },
                new FundLine { Fundline = "19+ Apprenticeship Non-Levy Contract (procured)", LineType = "ILR" },

                new FundLine { Fundline = "Authorised Claims: 16-18 Levy Apprenticeships - Training", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 16-18 Levy Apprenticeships - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Levy Apprenticeships - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Levy Apprenticeships - Employer", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Levy Apprenticeships - Apprentice", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: Adult Levy Apprenticeships - Training", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: Adult Levy Apprenticeships - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: Adult Levy Apprenticeships - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: Adult Levy Apprenticeships - Employer", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: Adult Levy Apprenticeships - Apprentice", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Apprenticeship (Employer on App Service) Non-Levy - Training", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 16-18 Apprenticeship (Employer on App Service) Non-Levy", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Apprenticeship (Employer on App Service) Non-Levy - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Apprenticeship (Employer on App Service) Non-Levy - Employer", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Apprenticeship (Employer on App Service) Non-Levy - Apprentice", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 19+ Apprenticeship (Employer on App Service) Non-Levy - Training", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 19+ Apprenticeship (Employer on App Service) Non-Levy", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 19+ Apprenticeship (Employer on App Service) Non-Levy - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 19+ Apprenticeship (Employer on App Service) Non-Levy - Employer", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 19+ Apprenticeship (Employer on App Service) Non-Levy - Apprentice", LineType = "EAS" },

                new FundLine { Fundline = "Authorised Claims: 16-18 Non-Levy Apprenticeships - Training", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Non-Levy Apprenticeships - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 16-18 Non-Levy Apprenticeships - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Non-Levy Apprenticeships - Employer", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: Adult Non-Levy Apprenticeships - Training", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: Adult Non-Levy Apprenticeships - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: Adult Non-Levy Apprenticeships - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: Adult Non-Levy Apprenticeships - Employer", LineType = "EAS" },

                new FundLine { Fundline = "Audit Adjustments: 16-18 Non-Levy Apprenticeships (procured) - Training", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Non-Levy Apprenticeships (procured) - Training", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: 16-18 Non-Levy Apprenticeships (procured) - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Non-Levy Apprenticeships (procured) - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: 16-18 Non-Levy Apprenticeships (procured) - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: 16-18 Non-Levy Apprenticeships (procured) - Employer", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Non-Levy Apprenticeships (procured) - Employer", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: 16-18 Non-Levy Apprenticeships (procured) - Apprentice", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: Adult Non-Levy Apprenticeships (procured) - Training", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: Adult Non-Levy Apprenticeships (procured) - Training", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: Adult Non-Levy Apprenticeships (procured) - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: Adult Non-Levy Apprenticeships (procured) - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Excess Learning Support: Adult Non-Levy Apprenticeships (procured) - Provider", LineType = "EAS" },
                new FundLine { Fundline = "Audit Adjustments: Adult Non-Levy Apprenticeships (procured) - Employer", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: Adult Non-Levy Apprenticeships (procured) - Employer", LineType = "EAS" },
                new FundLine { Fundline = "Authorised Claims: Adult Non-Levy Apprenticeships (procured) - Apprentice", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
                //new FundLine { Fundline = "", LineType = "EAS" },
            };
            return fundLines;
        }

        private IList<FcsContractAllocation> GetContractAllocation()
        {
            IList<FcsContractAllocation> fcsContractAllocations = new List<FcsContractAllocation>();
            var fundingStreams = GetFundingTypes().SelectMany(ft => ft.FundingStreams);

            foreach (var item in fundingStreams)
            {
                if (!fcsContractAllocations.Any(f => f.FundingStreamPeriodCode == item.PeriodCode))
                {
                    FcsContractAllocation allocation = new FcsContractAllocation()
                    {
                        ContractAllocationNumber = $"Alloc{item.PeriodCode}",
                        FundingStreamPeriodCode = item.PeriodCode,
                        DeliveryUkprn = ukprn,
                        DeliveryOrganisation = $"Org{ukprn}",
                    };
                    fcsContractAllocations.Add(allocation);
                }
            }

            return fcsContractAllocations;
        }

        private List<CollectionPeriod> GetCollectionPeriods()
        {
            var collectionPeriodsProvider = new CollectionPeriodsProvider(new JsonSerializationService());

            return collectionPeriodsProvider.Provide().ToList();
        }
    }
}