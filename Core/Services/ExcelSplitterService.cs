using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Core.Services
{
    public class ExcelSplitterService : IExcelSplitterService
    {
        public byte[] ProcessReports(List<string> filePaths, string templatePath)
        {
            // Создаем результатирующий файл Excel из шаблона
            using (var resultWorkbook = new XLWorkbook(templatePath))
            {
                // Загружаем все файлы с отчетами
                foreach (var filePath in filePaths)
                {
                    using (var workbook = new XLWorkbook(filePath))
                    {
                        // Проходим по всем листам в файле отчета
                        foreach (var worksheet in workbook.Worksheets)
                        {
                            var resultWorksheet = resultWorkbook.Worksheet(worksheet.Name);

                            // Суммируем данные с этого листа и записываем в итоговый отчет
                            SumAndWriteData(worksheet, resultWorksheet);
                        }
                    }
                }

                // Создаем поток в памяти для сохранения итогового Excel файла
                using (var memoryStream = new MemoryStream())
                {
                    resultWorkbook.SaveAs(memoryStream);
                    return memoryStream.ToArray(); // Возвращаем файл как byte[]
                }
            }
        }

        private void SumAndWriteData(IXLWorksheet sourceWorksheet, IXLWorksheet targetWorksheet)
        {
            int firstNumericRow = -1;
            int firstNumericColumn = -1;

            // Находим первое числовое значение на листе и его строку и столбец
            foreach (var row in sourceWorksheet.RowsUsed())
            {
                foreach (var cell in row.CellsUsed())
                {
                    if (double.TryParse(cell.Value.ToString(), out _))
                    {
                        firstNumericRow = row.RowNumber();
                        firstNumericColumn = cell.Address.ColumnNumber;
                        break;
                    }
                }
                if (firstNumericRow != -1) break; // Прерываем, как только нашли первое число
            }

            // Если мы нашли числовое значение, начинаем с пропуска первой строки
            if (firstNumericRow != -1 && firstNumericColumn != -1)
            {

                foreach (var row in sourceWorksheet.RowsUsed())
                {
                    if (row.RowNumber() <= firstNumericRow) continue; // Пропускаем все строки ДО первой числовой строки

                    foreach (var cell in row.CellsUsed().Skip(2))
                    {
                        if (double.TryParse(cell.Value.ToString(), out double sourceValue))
                        {
                            var targetCell = targetWorksheet.Cell(cell.Address);

                            if (double.TryParse(targetCell.Value.ToString(), out double targetValue))
                            {
                                targetCell.Value = targetValue + sourceValue;
                            }
                            else
                            {
                                targetCell.Value = sourceValue;
                            }
                        }
                    }
                }
            }
        }
    }
}