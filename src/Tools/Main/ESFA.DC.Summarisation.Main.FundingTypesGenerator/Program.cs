using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using ClosedXML.Excel;
using FundingTypesGenerator.Model.Json;
using FundingTypesGenerator.Model.SpreadSheet;

namespace FundingTypesGenerator
{
    class Program
    {
        private static string MainYear = "Main2021_";
        static void Main(string[] args)
        {
            var spreadSheetData = LoadSpreadsheet(@"data\2021 ILR Summarised Actuals for FCS Specification v5.1.xlsx");

            SaveJsonFromSpreadSheet(@"C:\Temp\FundingTypesGenerator\output.json", spreadSheetData);

            Console.ReadLine();
        }

        private static void SaveJsonFromSpreadSheet(string filename, List<SSFundingStream> spreadSheetData)
        {
            var summarisationTypes = new List<SummarisationTypeModel>
            {
                new SummarisationTypeModel { FundingStreams = new List<FundingStream>(), SummarisationType = $"{MainYear}FM35" },
                new SummarisationTypeModel { FundingStreams = new List<FundingStream>(), SummarisationType = $"{MainYear}EAS" },
                new SummarisationTypeModel { FundingStreams = new List<FundingStream>(), SummarisationType = $"{MainYear}FM25" },
                new SummarisationTypeModel { FundingStreams = new List<FundingStream>(), SummarisationType = $"{MainYear}ALB" },
                new SummarisationTypeModel { FundingStreams = new List<FundingStream>(), SummarisationType = $"{MainYear}TBL" },
            };

            foreach (var ssFundingStream in spreadSheetData)
            {
                foreach (var fundingStreamDeliverableLineCode in ssFundingStream.DeliverableLineCodes)
                {
                    var summType = GetSummarisationTypeForDLC(summarisationTypes, fundingStreamDeliverableLineCode);

                    var fundingStream = new FundingStream
                    {
                        PeriodCode = ssFundingStream.PeriodCode,
                        DeliverableLineCode = fundingStreamDeliverableLineCode.LineCode,
                        FundModel = summType.SummarisationType.Replace(MainYear, ""),
                        FundLines = new List<FundLine>()
                    };
                    summType.FundingStreams.Add(fundingStream);

                    foreach (var ssFundLine in fundingStreamDeliverableLineCode.FundLines)
                    {
                        var fundLine = new FundLine
                        {
                            Fundline = ssFundLine.Line,
                            LineType = ssFundLine.LineType,
                            UseAttributes = ssFundLine.ValueCalculation.Contains("AttributeName"),
                            Attributes = GetAttributesFromValueCalculation(ssFundLine.ValueCalculation)
                        };
                        fundingStream.FundLines.Add(fundLine);
                    }
                }
            }

            //Generate Unit Test Theories
            //foreach (var summarisationType in summarisationTypes)
            //{
            //    foreach (var fundingStream in summarisationType.FundingStreams)
            //    {
            //        foreach (var fundLine in fundingStream.FundLines)
            //        {
            //            Console.WriteLine($"[InlineData(\"{summarisationType.SummarisationType}\", \"{fundingStream.PeriodCode}\", {fundingStream.DeliverableLineCode}, \"{fundLine.Fundline}\", \"{fundLine.LineType}\")]");
            //        }
            //    }
            //}

            var jsonString = JsonSerializer.Serialize(summarisationTypes, new JsonSerializerOptions
                {
                    WriteIndented = true, 
                    IgnoreNullValues = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

            File.WriteAllText(filename, jsonString);
        }

        private static List<string> GetAttributesFromValueCalculation(string valueCalculation)
        {
            if (!valueCalculation.Contains("AttributeName"))
            {
                return null;
            }

            valueCalculation = valueCalculation.Substring(valueCalculation.IndexOf(":") + 1).Replace(",","");
            var attribs = valueCalculation
                .Replace("\r", "")
                .Replace(" ","")
                .Split("\n")
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .ToList();

            return attribs;
        }

        private static SummarisationTypeModel GetSummarisationTypeForDLC(List<SummarisationTypeModel> summarisationTypes, SSDeliverableLineCode fundingStreamDeliverableLineCode)
        {
            // Summarisation type is defined from the FundingType column
            var fundingType = fundingStreamDeliverableLineCode.FundingType;
            if (fundingType.IndexOf("- ") > 0)
            {
                fundingType = fundingType.Substring(fundingType.IndexOf("- ") + 2);
            }

            if (fundingType.IndexOf(" & ") > 0)
            {
                fundingType = fundingType.Substring(0, fundingType.IndexOf(" & "));
            }

            if (fundingType.IndexOf(" Only ") > 0)
            {
                fundingType = fundingType.Substring(0, fundingType.IndexOf(" Only "));
            }

            fundingType = fundingType.Replace("calcs", "").Replace("calc", "").Replace("Trailblazer", "TBL").Trim();

            return summarisationTypes.Single(st => st.SummarisationType == $"{MainYear}{fundingType}");
        }

        private static List<SSFundingStream> LoadSpreadsheet(string filename)
        {
            var result = new List<SSFundingStream>();

            using (var workbook = new XLWorkbook(filename))
            {
                foreach (IXLWorksheet worksheet in workbook.Worksheets)
                {
                    if (worksheet.Name != "Version control" && worksheet.Name != " FundStreamCode CCMDLC Mapping ")
                    {
                        var fundingStream = new SSFundingStream {DeliverableLineCodes = new List<SSDeliverableLineCode>()};
                        result.Add(fundingStream);

                        fundingStream.PeriodCode = worksheet.Row(2).Cell(1).GetValue<string>();

                        var rows = worksheet.RowsUsed().Skip(1);

                        SSDeliverableLineCode currentDLC = null;

                        foreach (var row in rows)
                        {
                            if (row.Cell(2).Value != null && !string.IsNullOrWhiteSpace(row.Cell(2).Value.ToString()))
                            {
                                currentDLC = new SSDeliverableLineCode {FundLines = new List<SSFundLine>()};
                                fundingStream.DeliverableLineCodes.Add(currentDLC);

                                currentDLC.LineCode = row.Cell(2).GetValue<int>();
                                currentDLC.DeliverableName = row.Cell(3).GetValue<string>();
                                currentDLC.FundingType = row.Cell(4).GetValue<string>();
                            }

                            var fundLine = new SSFundLine
                            {
                                Line = row.Cell(5).GetValue<string>(),
                                LineType = row.Cell(6).GetValue<string>(),
                                ValueCalculation = row.Cell(7).GetValue<string>(),
                            };
                            currentDLC.FundLines.Add(fundLine);

                        }
                    }
                }
            }

            result = result.OrderByDescending(ssf => ssf.PeriodCode).ToList();

            return result;
        }
    }
}
