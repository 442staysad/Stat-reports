using Core.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISummaryReportService
    {
        Task<IEnumerable<SummaryReportDto>> GetAllSummaryReportsAsync();
        Task<IEnumerable<SummaryReportDto>> GetSummaryReportsByPeriodAsync(DateTime periodStart, DateTime periodEnd);
        Task<SummaryReportDto> GetSummaryReportByIdAsync(int id);
        Task<SummaryReportDto> CreateSummaryReportAsync(SummaryReportDto summaryReportDto);
        Task<bool> DeleteSummaryReportAsync(int id); // Новый метод
    }
}