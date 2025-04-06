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
        private readonly IRepository<Report> _reportRepo;
        private readonly IRepository<ReportTemplate> _templateRepo;
        private readonly IExcelSplitterService _excelSplitter;

        public SummaryReportService(IRepository<Report> reportRepo,
                                    IRepository<ReportTemplate> templateRepo,
                                    IExcelSplitterService excelSplitter)
        {
            _reportRepo = reportRepo;
            _templateRepo = templateRepo;
            _excelSplitter = excelSplitter;
        }

        public async Task<List<Report>> GetReportsForSummaryAsync(int templateId, int year, int? month, int? quarter, int? halfYear, List<int> branchIds)
        {
            var reports = await _reportRepo.FindAllAsync(r =>
                r.TemplateId == templateId &&
                r.UploadDate.Year == year &&
                branchIds.Contains((int)r.BranchId));

            if (month != null)
                reports = reports.Where(r => r.Period.Month == month).ToList();
            else if (quarter != null)
            {
                var months = GetQuarterMonths(quarter.Value);
                reports = reports.Where(r => months.Contains(r.Period.Month)).ToList();
            }
            else if (halfYear != null)
            {
                var months = halfYear == 1 ? new[] { 1, 2, 3, 4, 5, 6 } : new[] { 7, 8, 9, 10, 11, 12 };
                reports = reports.Where(r => months.Contains(r.Period.Month)).ToList();
            }

            return reports.ToList();
        }

        public Task<string> GetTemplateFilePathAsync(int templateId)
        {
            // Пусть путь хранится в шаблоне
            return _templateRepo.FindAsync(t => t.Id == templateId).ContinueWith(t => t.Result.FilePath);
        }

        public byte[] MergeReportsToExcel(List<Report> reports, string templatePath)
        {
            var paths = reports.Select(r => r.FilePath).ToList();
            return _excelSplitter.ProcessReports(paths, templatePath);
        }

        private List<int> GetQuarterMonths(int quarter)
        {
            return quarter switch
            {
                1 => new List<int> { 1, 2, 3 },
                2 => new List<int> { 4, 5, 6 },
                3 => new List<int> { 7, 8, 9 },
                4 => new List<int> { 10, 11, 12 },
                _ => throw new ArgumentException("Некорректный квартал")
            };
        }
    }
}