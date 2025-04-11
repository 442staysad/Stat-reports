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
        private readonly INotificationService _notificationService;
        private readonly IFileService _fileService;
        private readonly IDeadlineService _deadlineService;

        public ReportService(
            IRepository<Report> _reportRepository,
            IRepository<ReportTemplate> templateRepository,
            IRepository<SubmissionDeadline> deadlineRepository,
            IFileService fileService,
            IDeadlineService deadlineService, 
            IRepository<Branch> branchrepository,
            IRepository<User> userRepository,
            INotificationService notificationService)
        {
            this._reportRepository = _reportRepository;
            _templateRepository = templateRepository;
            _deadlineRepository = deadlineRepository;
            _fileService = fileService;
            _deadlineService = deadlineService;
            _userRepository = userRepository;
            _branchRepository = branchrepository;
            _notificationService = notificationService;
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

        public async Task<ReportDto?> GetReportByIdAsync(int id)
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

        /// <summary>
        /// Проверяет срок сдачи перед загрузкой отчета.
        /// </summary>
        public async Task<ReportDto> UploadReportAsync(int templateId, int branchId, int uploadedById, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл отсутствует или пуст");

            // Получаем информацию о филиале и шаблоне
            var branch = await _branchRepository.FindAsync(b => b.Id == branchId);
            if (branch == null) throw new ArgumentException("Филиал не найден");

            var template = await _templateRepository.FindAsync(t => t.Id == templateId);
            if (template == null) throw new ArgumentException("Шаблон не найден");

            var deadline = await _deadlineRepository.FindAsync(d => d.ReportTemplateId == templateId 
            && d.BranchId == branchId);


            // Проверяем, существует ли отчет для указанного периода
            var existingReport = await _reportRepository.FindAsync(r => r.TemplateId == templateId 
            && r.BranchId == branchId && r.Period == deadline.Period);

            // Сохраняем новый файл
            var filePath = await _fileService.SaveFileAsync(file, "Reports", branch.Name, DateTime.Now.Year, template.Name);
            
            if (existingReport != null)
            {
                // Если отчет существует, обновляем его
                if (!string.IsNullOrEmpty(existingReport.FilePath))
                {
                    // Удаляем старый файл
                    await _fileService.DeleteFileAsync(existingReport.FilePath);
                }

                existingReport.Name = file.FileName;
                existingReport.FilePath = filePath;
                existingReport.UploadedById = uploadedById;
                existingReport.UploadDate = DateTime.UtcNow;

                var updatedReport = await _reportRepository.UpdateAsync(existingReport);

               
                if (deadline != null)
                {
                    deadline.Status = ReportStatus.Draft;
                    deadline.ReportId = updatedReport.Id;
                    await _deadlineRepository.UpdateAsync(deadline);
                }

                return MapToDto(updatedReport);
            }
            else
            {
                // Если отчет не существует, создаем новый
                var newReport = new Report
                {
                    Name = file.FileName,
                    TemplateId = templateId,
                    BranchId = branchId,
                    UploadedById = uploadedById,
                    FilePath = filePath,
                    UploadDate = DateTime.UtcNow,
                    Branch = branch,
                    Template = template,
                    Period = deadline.Period,
                };

                var createdReport = await _reportRepository.AddAsync(newReport);

                // Обновляем дедлайн
                
                if (deadline != null)
                {
                    deadline.Status = ReportStatus.Draft;
                    deadline.ReportId = createdReport.Id;
                    await _deadlineRepository.UpdateAsync(deadline);
                }

                return MapToDto(createdReport);
            }
        }


        /// <summary>
        /// Изменение статуса отчета (например, после проверки).
        /// </summary> 

        public async Task<bool> UpdateReportStatusAsync(int reportId, ReportStatus newStatus, string? remarks = null)
        {
            var report = await _reportRepository.FindAsync(r => r.Id == reportId);
            if (report == null) return false;
            var deadline = await _deadlineRepository.FindAsync(r=>r.ReportTemplateId==report.TemplateId);
            if (deadline == null) return false;

            
            deadline.Status= newStatus;
            if (!string.IsNullOrEmpty(remarks))
                deadline.Comment = remarks;



            if (newStatus == ReportStatus.Reviewed)
            {
                await _notificationService.AddNotificationAsync((int)report.UploadedById, $"{report.Name}: Отчет принят");
                await _deadlineService.CheckAndUpdateDeadlineAsync(deadline.ReportTemplateId);
                deadline.Status = ReportStatus.InProgress;
                deadline.ReportId = null; // Удаляем связь с отчетом
                deadline.Comment = null; // Удаляем комментарий
            }
            await _deadlineRepository.UpdateAsync(deadline);

            return true;
        }

        /// <summary>
        /// Добавление замечания к отчету (например, при проверке).
        /// </summary>
        public async Task<bool> AddReportCommentAsync(int reportId, string comment)
        {
            var report = await _reportRepository.FindAsync(r => r.Id == reportId);

            await _reportRepository.UpdateAsync(report);
            var deadline = await _deadlineRepository.FindAsync(r => r.ReportTemplateId == report.TemplateId);
            if (deadline == null) return false;
            if (!string.IsNullOrEmpty(comment))
                deadline.Comment = comment;
            deadline.Status = ReportStatus.NeedsCorrection;

            await _notificationService.AddNotificationAsync((int)report.UploadedById, $"{report.Name}: {comment}");

            await _deadlineRepository.UpdateAsync(deadline);
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

            var deadlines = await _deadlineRepository
                .GetAll(q => q.Include(t => t.Template))
                .Where(d => d.BranchId == branchId) // Фильтрация по филиалу
                .ToListAsync();

            var pendingTemplates = new List<PendingTemplateDto>();

            foreach (var deadline in deadlines)
            {
                if (deadline == null)
                {
                    Console.WriteLine("Ошибка: template == null");
                    continue;
                }

                if (deadline.Template == null)
                {
                    Console.WriteLine($"Ошибка: template.Template == null (TemplateId: {deadline.ReportTemplateId})");
                }


                pendingTemplates.Add(new PendingTemplateDto
                {
                    Id=deadline.Id,
                    TemplateId = deadline.ReportTemplateId,
                    TemplateName = deadline.Template?.Name ?? "Неизвестный шаблон",
                    Deadline = deadline.DeadlineDate,
                    ReportId = deadline.ReportId,
                    Status = deadline.Status.ToString(),
                    Comment=deadline.Comment
                    
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

            // Установка контекста лицензии
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(stream);

            // Предположим, что филиал берется из самого отчета
            string branchKey = report.BranchId?.ToString() ?? throw new InvalidOperationException("BranchId is null");

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
                Comment = report.Comment,
                Period = report.Period,
                UploadDate = report.UploadDate
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
                Comment = reportDto.Comment,
                Period = reportDto.Period
                
            };
        }
    }
}
