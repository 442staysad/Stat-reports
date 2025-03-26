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

namespace Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<Report> _reportRepository;
        private readonly IRepository<ReportTemplate> _templateRepository;
        private readonly IRepository<SubmissionDeadline> _deadlineRepository;
        private readonly IFileService _fileService;
        private readonly IExcelSplitterService _excelToJsonService;

        public ReportService(
            IRepository<Report> _reportRepository,
            IRepository<ReportTemplate> templateRepository,
            IRepository<SubmissionDeadline> deadlineRepository,
            IFileService fileService,
            IExcelSplitterService excelToJsonService)
        {
            this._reportRepository = _reportRepository;
            _templateRepository = templateRepository;
            _deadlineRepository = deadlineRepository;
            _fileService = fileService;
            _excelToJsonService = excelToJsonService;
        }

       public async Task<IEnumerable<Report>> GetAllReportsAsync()
        {
            var reports = await _reportRepository.GetAllAsync();
            return reports;
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
            existingReport.Status = (ReportStatus)reportDto.Status;
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

        /*public async Task<IEnumerable<Report>> GetAllReportsAsync()
        {
            return await _reportRepository.GetAllAsync();
        }

        public async Task<Report> GetReportByIdAsync(int id)
        {
            return await _reportRepository.FindAsync(r => r.Id == id);
        }*/

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

      /*  public async Task<bool> DeleteReportAsync(int id)
        {
            var report = await _reportRepository.FindAsync(r => r.Id == id);
            if (report == null) return false;
            await _reportRepository.DeleteAsync(report);
            return true;
        }*/

        /// <summary>
        /// Проверяет срок сдачи перед загрузкой отчета.
        /// </summary>
        public async Task<ReportDto> UploadReportAsync(int templateId, int branchId, int uploadedById, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл отсутствует или пуст");

            // Проверяем срок сдачи
            var deadline = await _deadlineRepository.FindAsync(d => d.ReportTemplateId == templateId);

            // Сохраняем файл
            var filePath = await _fileService.SaveFileAsync(file, $"Reports/{templateId}");

            // Конвертируем Excel в JSON
            //var jsonData = await _excelToJsonService.ConvertToJSONAsync(filePath);
         //   var jsonString = JsonSerializer.Serialize(jsonData);

            var report = new Report
            {
                Name = file.FileName,
                TemplateId = templateId,
                BranchId = branchId,
                UploadedById = uploadedById,
               // Fields = jsonString,  // Сохраняем JSON в БД
                FilePath = filePath,
                Status = ReportStatus.Черновик,
                UploadDate = DateTime.UtcNow,
            };

            var createdReport = await _reportRepository.AddAsync(report);
            return MapToDto(createdReport);
        }

        /// <summary>
        /// Изменение статуса отчета (например, после проверки).
        /// </summary>
        public async Task<bool> UpdateReportStatusAsync(int reportId, int status, string remarks)
        {
            var report = await _reportRepository.FindAsync(r => r.Id == reportId);
            if (report == null) return false;

            report.Status = (ReportStatus)status;
            report.Comment = remarks;

            await _reportRepository.UpdateAsync(report);
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

        public async Task<byte[]> DownloadReportAsync(int reportId)
        {
            var report = await _reportRepository.FindAsync(r => r.Id == reportId);
            if (report == null || string.IsNullOrEmpty(report.FilePath))
                return null;

            return await _fileService.GetFileAsync(report.FilePath);
        }

        public async Task<List<PendingTemplateDto>> GetPendingTemplatesAsync()
        {
            var today = DateTime.UtcNow;
            var templates = await _templateRepository
                .GetAll()
                .Include(t => t.SubmissionDeadline) // Исправлено
                .ToListAsync();

            var pendingTemplates = new List<PendingTemplateDto>();

            foreach (var template in templates)
            {
                var deadline = CalculateDeadline(template.SubmissionDeadline); // Добавить реализацию метода
                if (deadline > today && deadline <= today.AddMonths(1)) // Дедлайн в ближайший месяц
                {
                    var existingReport = await _reportRepository.FindAsync(r =>
                        r.TemplateId == template.Id &&
                        r.UploadDate.Year == today.Year &&
                        r.UploadDate.Month == today.Month);

                    pendingTemplates.Add(new PendingTemplateDto
                    {
                        TemplateId = template.Id,
                        TemplateName = template.Name,
                        Deadline = deadline,
                        Status = (ReportStatus)(existingReport?.Status)
                    });
                }
            }

            return pendingTemplates;
        }

        private DateTime CalculateDeadline(SubmissionDeadline deadline)
        {
            var now = DateTime.UtcNow;
            if (deadline != null)
            {
                switch (deadline.DeadlineType)
                {
                    case DeadlineType.Quarterly:
                        var quarter = (now.Month - 1) / 3 + 1;
                        var quarterEndMonth = quarter * 3;
                        return new DateTime(now.Year, quarterEndMonth, deadline.FixedDay ?? 30);

                    case DeadlineType.HalfYearly:
                        var halfYearEndMonth = now.Month <= 6 ? 6 : 12;
                        return new DateTime(now.Year, halfYearEndMonth, deadline.FixedDay ?? 30);

                    case DeadlineType.Yearly:
                        return new DateTime(now.Year, 12, deadline.FixedDay ?? 30);

                    default:
                        throw new InvalidOperationException("Неизвестный тип дедлайна.");
                }
            }
            else
            {
                return DateTime.Now;
            }
        }


        private ReportDto MapToDto(Report report)
        {
            return new ReportDto
            {
                Id = report.Id,
                Name = report.Name,
                SubmissionDate = report.UploadDate,
                Status = (ReportStatus)report.Status,
                FilePath = report.FilePath,
                UploadedById = (int)report.UploadedById,
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
                Status = (ReportStatus)reportDto.Status,
                FilePath = reportDto.FilePath,
                UploadedById = reportDto.UploadedById,
                BranchId = reportDto.BranchId ?? 0,
                TemplateId = reportDto.TemplateId ?? 0,
                Comment = reportDto.Comment
            };
        }
    }
}
