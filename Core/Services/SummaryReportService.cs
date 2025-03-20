using Core.DTO;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services
{
    public class SummaryReportService : ISummaryReportService
    {
        private readonly ISummaryReportRepository _summaryReportRepository;

        public SummaryReportService(ISummaryReportRepository summaryReportRepository)
        {
            _summaryReportRepository = summaryReportRepository;
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