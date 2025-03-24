using Core.DTO;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.Services
{
    public class SummaryReportService : ISummaryReportService
    {
        private readonly ISummaryReportRepository _summaryReportRepository;
        private readonly IRepository<Report> _reportRepository;

        public SummaryReportService(ISummaryReportRepository summaryReportRepository, IRepository<Report> reportRepository)
        {
            _summaryReportRepository = summaryReportRepository;
            _reportRepository = reportRepository;
        }

        public async Task<IEnumerable<SummaryReportDto>> GetAllSummaryReportsAsync()
        {
            var reports = await _summaryReportRepository.GetAllAsync();
            return reports.Select(MapToDto);
        }

        public async Task<IEnumerable<SummaryReportDto>> GetSummaryReportsByPeriodAsync(DateTime periodStart, DateTime periodEnd)
        {
            var reports = await _summaryReportRepository.GetByPeriodAsync(periodStart, periodEnd);
            return reports.Select(MapToDto);
        }

        public async Task<SummaryReportDto> GetSummaryReportByIdAsync(int id)
        {
            var report = await _summaryReportRepository.FindAsync(r => r.Id == id);
            return report == null ? null : MapToDto(report);
        }

        public async Task<SummaryReportDto> CreateSummaryReportAsync(SummaryReportDto summaryReportDto)
        {
            var summaryReport = MapToEntity(summaryReportDto);
            var createdReport = await _summaryReportRepository.AddAsync(summaryReport);
            return MapToDto(createdReport);
        }
        public async Task<bool> DeleteSummaryReportAsync(int id)
        {
            var report = await _summaryReportRepository.FindAsync(r => r.Id == id);
            if (report == null) return false;

            await _summaryReportRepository.DeleteAsync(report);
            return true;
        }

        public async Task<SummaryReportDto> GenerateSummaryReportAsync(List<int> reportIds)
        {
            var reports = await _reportRepository
                .GetAll()
                .Where(r => reportIds.Contains(r.Id))
                .ToListAsync();

            // Инициализируем итоговые данные
            var summaryData = new Dictionary<string, Dictionary<string, decimal>>();

            foreach (var report in reports)
            {
                var reportData = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, object>>>>(report.Fields);

                foreach (var sheet in reportData)
                {
                    // Обрабатываем каждый лист (например, "Лист1")
                    foreach (var row in sheet.Value)
                    {
                        var rowKey = row["Показатель"].ToString();
                        foreach (var key in row.Keys.Where(k => k != "Показатель"))
                        {
                            if (!summaryData.ContainsKey(rowKey))
                            {
                                summaryData[rowKey] = new Dictionary<string, decimal>();
                            }

                            if (!summaryData[rowKey].ContainsKey(key))
                            {
                                summaryData[rowKey][key] = 0;
                            }

                            // Суммируем данные по каждому ключу
                            if (decimal.TryParse(row[key]?.ToString(), out var value))
                            {
                                summaryData[rowKey][key] += value;
                            }
                        }
                    }
                }
            }

            // Формируем DTO для сводного отчета
            var summaryReport = new SummaryReportDto
            {
              /*  GeneratedDate = DateTime.UtcNow,
                Data = summaryData*/
            };

            return summaryReport;
        }

        private SummaryReportDto MapToDto(SummaryReport report)
        {
            return new SummaryReportDto
            {
                Id = report.Id,
                Name = report.Name,
                FilePath = report.FilePath,
                CreatedDate = report.CreatedDate,
                ReportTemplateId = report.ReportTemplateId,
                PeriodStart = report.PeriodStart,
                PeriodEnd = report.PeriodEnd
            };
        }

        private SummaryReport MapToEntity(SummaryReportDto reportDto)
        {
            return new SummaryReport
            {
                Id = reportDto.Id,
                Name = reportDto.Name,
                FilePath = reportDto.FilePath,
                CreatedDate = reportDto.CreatedDate,
                ReportTemplateId = reportDto.ReportTemplateId,
                PeriodStart = reportDto.PeriodStart,
                PeriodEnd = reportDto.PeriodEnd
            };
        }
    }
}