using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
namespace Core.Services
{
    public class ExcelReportMerger : IExcelReportMerger
    {
        public async Task<byte[]> MergeReportsAsync(List<string> filePaths)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            var summaryData = new Dictionary<string, decimal[]>();
            var headers = new List<string>();
            int valueColumnStart = int.MaxValue; // Найдем минимальный индекс числовых колонок

            foreach (var filePath in filePaths)
            {
                using var package = new ExcelPackage(new FileInfo(filePath));
                var sheet = package.Workbook.Worksheets.First(); // Берем первый лист (кроме "Главная")

                int colCount = sheet.Dimension.End.Column;
                int rowCount = sheet.Dimension.End.Row;

                if (headers.Count == 0)
                {
                    headers = Enumerable.Range(1, colCount)
                                        .Select(i => sheet.Cells[1, i].Text)
                                        .ToList();
                }

                for (int col = 3; col <= colCount; col++) // Ищем первую колонку с числами
                {
                    if (decimal.TryParse(sheet.Cells[2, col].Text, out _))
                    {
                        valueColumnStart = Math.Min(valueColumnStart, col);
                        break;
                    }
                }

                for (int row = 2; row <= rowCount; row++)
                {
                    string key = sheet.Cells[row, 1].Text.Trim();
                    if (string.IsNullOrWhiteSpace(key)) continue;

                    if (!summaryData.ContainsKey(key))
                        summaryData[key] = new decimal[colCount - valueColumnStart + 1];

                    for (int col = valueColumnStart; col <= colCount; col++)
                    {
                        if (decimal.TryParse(sheet.Cells[row, col].Text, out decimal value))
                        {
                            summaryData[key][col - valueColumnStart] += value;
                        }
                    }
                }
            }

            return GenerateSummaryExcel(headers, summaryData, valueColumnStart);
        }

        private byte[] GenerateSummaryExcel(List<string> headers, Dictionary<string, decimal[]> summaryData, int valueColumnStart)
        {
            using var outputPackage = new ExcelPackage();
            var summarySheet = outputPackage.Workbook.Worksheets.Add("Summary");

            for (int col = 0; col < headers.Count; col++)
            {
                summarySheet.Cells[1, col + 1].Value = headers[col];
            }

            int rowIdx = 2;
            foreach (var (key, values) in summaryData)
            {
                summarySheet.Cells[rowIdx, 1].Value = key;
                for (int col = 0; col < values.Length; col++)
                {
                    summarySheet.Cells[rowIdx, col + valueColumnStart].Value = values[col];
                }
                rowIdx++;
            }

            return outputPackage.GetAsByteArray();
        }
    }
}