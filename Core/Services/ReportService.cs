using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.DTO;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<Report> _reportRepository;
        private readonly IRepository<ReportTemplate> _templateRepository;
        private readonly IRepository<SubmissionDeadline> _deadlineRepository;
        private readonly IRepository<Branch> _branchRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IFileService _fileService;
        private readonly IDeadlineService _deadlineService;

        public ReportService(
            IRepository<Report> _reportRepository,
            IRepository<ReportTemplate> templateRepository,
            IRepository<SubmissionDeadline> deadlineRepository,
            IFileService fileService,
            IDeadlineService deadlineService, 
            IRepository<Branch> branchrepository,
            IRepository<User> userRepository)
        {
            this._reportRepository = _reportRepository;
            _templateRepository = templateRepository;
            _deadlineRepository = deadlineRepository;
            _fileService = fileService;
            _deadlineService = deadlineService;
            _userRepository = userRepository;
            _branchRepository = branchrepository;
        }

        public async Task<IEnumerable<Report>> GetAllReportsAsync()
        {
            var reports = await _reportRepository.GetAllAsync();
            return reports;
        }
        public async Task<IEnumerable<Report>> GetReportsByBranchAsync(int branchId)
        {
            return await _reportRepository.FindAllAsync(r => r.BranchId == branchId);
        }

        public async Task<ReportDto> GetReportByIdAsync(int id)
        {
            var report = await _reportRepository.FindAsync(r => r.Id == id);
            return report == null ? null : MapToDto(report);
        }

        public async Task<ReportDto> CreateReportAsync(ReportDto reportDto)
        {
            var report = MapToEntity(reportDto);
            var createdReport = await _reportRepository.AddAsync(report);
            return MapToDto(createdReport);
        }

        public async Task<ReportDto> UpdateReportAsync(int id, ReportDto reportDto)
        {
            var existingReport = await _reportRepository.FindAsync(r => r.Id == id);
            if (existingReport == null) return null;

            existingReport.Name = reportDto.Name;
            existingReport.UploadDate = reportDto.SubmissionDate;

            existingReport.FilePath = reportDto.FilePath;

            var updatedReport = await _reportRepository.UpdateAsync(existingReport);
            return MapToDto(updatedReport);
        }

       public async Task<bool> DeleteReportAsync(int id)
       {
            var report = await _reportRepository.FindAsync(r => r.Id == id);
            if (report == null) return false;
            
            if (!string.IsNullOrEmpty(report.FilePath))
                await _fileService.DeleteFileAsync(report.FilePath);

            await _reportRepository.DeleteAsync(report);
            return true;
        }

        public async Task<IEnumerable<ReportTemplate>> GetAllTemplatesAsync()
        {
            return await _templateRepository.GetAllAsync();
        }


        public ReportService(IRepository<Report> reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<Report> CreateReportAsync(Report report)
        {
            await _reportRepository.AddAsync(report);
            return report;
        }

        public async Task<Report> UpdateReportAsync(Report report)
        {
            await _reportRepository.DeleteAsync(report);
            return report;
        }

        /// <summary>
        /// Проверяет срок сдачи перед загрузкой отчета.
        /// </summary>
        public async Task<ReportDto> UploadReportAsync(int templateId, int branchId, int uploadedById, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл отсутствует или пуст");
            var branchname = await _branchRepository.FindAsync(b => b.Id == branchId);
           

            var templateName = await _templateRepository.FindAsync(t => t.Id == templateId);
            // Проверяем срок сдачи
            var deadline = await _deadlineRepository.FindAsync(d => d.ReportTemplateId == templateId);
            deadline.Status = ReportStatus.Draft;
            await _deadlineRepository.UpdateAsync(deadline);
            // Сохраняем файл
            var filePath = await _fileService.SaveFileAsync(file, "Reports",branchname.Name,DateTime.Now.Year,templateName.Name);

            var report = new Report
            {
                Name = file.FileName,
                TemplateId = templateId,
                BranchId = branchId,
                UploadedById = uploadedById,
                FilePath = filePath,
                
                UploadDate = DateTime.UtcNow,
                Branch=branchname,
                Template = templateName
            };

            var createdReport = await _reportRepository.AddAsync(report);
            return MapToDto(createdReport);
        }

        /// <summary>
        /// Изменение статуса отчета (например, после проверки).
        /// </summary> 
        
        public async Task<bool> UpdateReportStatusAsync(int reportId, ReportStatus newStatus, string? remarks = null)
        {/*
            var report = await _reportRepository.FindAsync(r=>r.Id==reportId);
            if (report == null) return false;

            report.Status = newStatus;
            if (!string.IsNullOrEmpty(remarks))
                report.Comment = remarks;

            _reportRepository.UpdateAsync(report);

            if (newStatus == ReportStatus.Reviewed)
            {
                await _deadlineService.CheckAndUpdateDeadlineAsync(report.TemplateId);
            }
            */
            return true;
        }

        /// <summary>
        /// Добавление замечания к отчету (например, при проверке).
        /// </summary>
        public async Task<bool> AddReportCommentAsync(int reportId, string comment)
        {
            var report = await _reportRepository.FindAsync(r => r.Id == reportId);
            if (report == null) return false;

            report.Comment = comment;
            await _reportRepository.UpdateAsync(report);
            return true;
        }

        public async Task<byte[]> GetReportFileAsync(int reportId)
        {
            var report = await _reportRepository.FindAsync(r => r.Id == reportId);
            if (report == null || string.IsNullOrEmpty(report.FilePath))
                throw new FileNotFoundException("Файл отчета не найден");

            return await _fileService.GetFileAsync(report.FilePath);
        }

        public async Task<List<PendingTemplateDto>> GetPendingTemplatesAsync(int? branchId)
        {
            var today = DateTime.UtcNow;

            var templates = await _deadlineRepository
                .GetAll(q => q.Include(t => t.Template))
                .Where(d => d.BranchId == branchId) // Фильтрация по филиалу
                .ToListAsync();

            var pendingTemplates = new List<PendingTemplateDto>();

            foreach (var template in templates)
            {
                if (template == null)
                {
                    Console.WriteLine("Ошибка: template == null");
                    continue;
                }

                if (template.Template == null)
                {
                    Console.WriteLine($"Ошибка: template.Template == null (TemplateId: {template.ReportTemplateId})");
                }

                var existingReport = await _reportRepository.FindAsync(r =>
                    r.BranchId == branchId); // Фильтрация по филиалу

                pendingTemplates.Add(new PendingTemplateDto
                {
                    TemplateId = template.ReportTemplateId,
                    TemplateName = template.Template?.Name ?? "Неизвестный шаблон",
                    Deadline = template.DeadlineDate,
                    ReportId = existingReport?.Id,
                    Status = template.Status.ToString(),
                    
                });
            }

            return pendingTemplates;
        }

        public async Task<Dictionary<string, Dictionary<string, List<List<string>>>>> ReadExcelFileAsync(int reportId)
        {
            var report = await _reportRepository.FindAsync(r => r.Id == reportId);
            if (report == null || string.IsNullOrEmpty(report.FilePath))
                throw new FileNotFoundException("Report file not found");

            var fileBytes = await _fileService.GetFileAsync(report.FilePath);
            using var stream = new MemoryStream(fileBytes);
            using var package = new ExcelPackage(stream);

            // Предположим, что филиал берется из самого отчета
            string branchKey = report.BranchId.ToString();

            var result = new Dictionary<string, Dictionary<string, List<List<string>>>>();

            if (!result.ContainsKey(branchKey))
                result[branchKey] = new Dictionary<string, List<List<string>>>();

            foreach (var worksheet in package.Workbook.Worksheets)
            {
                var sheetData = new List<List<string>>();
                for (int row = 1; row <= worksheet.Dimension.Rows; row++)
                {
                    var rowData = new List<string>();
                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                    {
                        rowData.Add(worksheet.Cells[row, col].Text);
                    }
                    sheetData.Add(rowData);
                }
                result[branchKey][worksheet.Name] = sheetData;
            }

            return result;
        }
        // Другие методы...

        public async Task<IEnumerable<ReportDto>> GetFilteredReportsAsync(string? name, int? templateId, int? branchId, DateTime? startDate, DateTime? endDate)
        {
            var query = _reportRepository.GetAll();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(r => r.Name.Contains(name));
            }

            if (templateId.HasValue)
            {
                query = query.Where(r => r.TemplateId == templateId.Value);
            }

            if (branchId.HasValue)
            {
                query = query.Where(r => r.BranchId == branchId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(r => r.UploadDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.UploadDate <= endDate.Value);
            }

            var reports = await query.ToListAsync();
            return reports.Select(r => new ReportDto
            {
                Id = r.Id,
                Name = r.Name,
                SubmissionDate = r.UploadDate,
                UploadedById = r.UploadedById ?? 0,
                BranchId = r.BranchId,
                TemplateId = r.TemplateId,
                FilePath = r.FilePath,
                Comment = r.Comment
            }).ToList();
        }

        private ReportDto MapToDto(Report report)
        {
            return new ReportDto
            {
                Id = report.Id,
                Name = report.Name,
                SubmissionDate = report.UploadDate,
                FilePath = report.FilePath,
                UploadedById = report.UploadedById ?? 0, // Fix for nullable type
                BranchId = report.BranchId,
                TemplateId = report.TemplateId,
                Comment = report.Comment
            };
        }

        private Report MapToEntity(ReportDto reportDto)
        {
            return new Report
            {
                Id = reportDto.Id,
                Name = reportDto.Name,
                UploadDate = reportDto.SubmissionDate,
                FilePath = reportDto.FilePath,
                UploadedById = reportDto.UploadedById,
                BranchId = reportDto.BranchId ?? 0,
                TemplateId = reportDto.TemplateId ?? 0,
                Comment = reportDto.Comment
            };
        }
    }
}
