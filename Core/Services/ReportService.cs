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
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IFileService _fileService;
        private readonly IDeadlineService _deadlineService;

        public ReportService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IFileService fileService,
            IDeadlineService deadlineService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _fileService = fileService;
            _deadlineService = deadlineService;
        }

        public async Task<IEnumerable<Report>> GetAllReportsAsync()
        {
            return await _unitOfWork.Reports.GetAllAsync();
        }

        public async Task<IEnumerable<Report>> GetReportsByBranchAsync(int branchId)
        {
            return await _unitOfWork.Reports.FindAllAsync(r => r.BranchId == branchId);
        }

        public async Task<ReportDto?> GetReportByIdAsync(int id)
        {
            var report = await _unitOfWork.Reports.FindAsync(r => r.Id == id);
            return report == null ? null : MapToDto(report);
        }

        public async Task<ReportDto> CreateReportAsync(ReportDto reportDto)
        {
            var report = MapToEntity(reportDto);
            var created = await _unitOfWork.Reports.AddAsync(report);
            return MapToDto(created);
        }

        public async Task<Report?> FindByTemplateBranchPeriodAsync(int templateId, int branchId, int year, int month)
        {
            return await _unitOfWork.Reports.FindAsync(r =>
                r.TemplateId == templateId &&
                r.BranchId == branchId &&
                r.Period.Year == year &&
                r.Period.Month == month);
        }

        public async Task<ReportDto> UpdateReportAsync(int id, ReportDto reportDto)
        {
            var report = await _unitOfWork.Reports.FindAsync(r => r.Id == id);
            if (report == null) return null;

            report.Name = reportDto.Name;
            report.UploadDate = reportDto.SubmissionDate;
            report.FilePath = reportDto.FilePath;

            await _unitOfWork.Reports.UpdateAsync(report);

            return MapToDto(report);
        }

        public async Task<bool> DeleteReportAsync(int id)
        {
            var report = await _unitOfWork.Reports.FindAsync(r => r.Id == id);
            if (report == null) return false;

            if (!string.IsNullOrEmpty(report.FilePath))
                await _fileService.DeleteFileAsync(report.FilePath);

            await _unitOfWork.Reports.DeleteAsync(report);

            return true;
        }

        public async Task<ReportDto> UploadReportAsync(int templateId, int branchId, int uploadedById, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл отсутствует или пуст");

            var branch = await _unitOfWork.Branches.FindAsync(b => b.Id == branchId)
                         ?? throw new ArgumentException("Филиал не найден");

            var template = await _unitOfWork.ReportTemplates.FindAsync(t => t.Id == templateId)
                           ?? throw new ArgumentException("Шаблон не найден");

            var deadline = await _unitOfWork.SubmissionDeadlines.FindAsync(d =>
                d.ReportTemplateId == templateId && d.BranchId == branchId)
                ?? throw new ArgumentException("Срок сдачи не найден");

            var existingReport = await _unitOfWork.Reports.FindAsync(r =>
                r.TemplateId == templateId && r.BranchId == branchId && r.Period == deadline.Period);

            var filePath = await _fileService.SaveFileAsync(file, "Reports", branch.Name, DateTime.Now.Year, template.Name);

            if (existingReport != null)
            {
                if (!string.IsNullOrEmpty(existingReport.FilePath))
                    await _fileService.DeleteFileAsync(existingReport.FilePath);

                existingReport.Name = file.FileName;
                existingReport.FilePath = filePath;
                existingReport.UploadedById = uploadedById;
                existingReport.UploadDate = DateTime.UtcNow;
                existingReport.Period = deadline.Period;

                await _unitOfWork.Reports.UpdateAsync(existingReport);

                deadline.Status = ReportStatus.Draft;
                deadline.ReportId = existingReport.Id;
                await _unitOfWork.SubmissionDeadlines.UpdateAsync(deadline);

                

                return MapToDto(existingReport);
            }

            var newReport = new Report
            {
                Name = file.FileName,
                TemplateId = templateId,
                BranchId = branchId,
                UploadedById = uploadedById,
                FilePath = filePath,
                UploadDate = DateTime.UtcNow,
                Period = deadline.Period,
                Branch = branch,
                Template = template
            };

            var createdReport = await _unitOfWork.Reports.AddAsync(newReport);

            deadline.Status = ReportStatus.Draft;
            deadline.ReportId = createdReport.Id;
            await _unitOfWork.SubmissionDeadlines.UpdateAsync(deadline);

            var user = await _unitOfWork.Users.GetAll(u => u.Include(r => r.Role))
                .Where(u => u.Id == uploadedById).FirstOrDefaultAsync();

            if (user?.Role?.RoleName != "User")
                await UpdateReportStatusAsync(deadline.Id,createdReport.Id, ReportStatus.Reviewed);

            

            return MapToDto(createdReport);
        }
        
        public async Task<bool> UpdateReportStatusAsync(int deadlineId, int reportId, ReportStatus newStatus)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var report = await _unitOfWork.Reports.FindAsync(r => r.Id == reportId);
                if (report == null) return false;

                // Находим активный (не закрытый) дедлайн для этого отчета
                var deadline = await _unitOfWork.SubmissionDeadlines.FindAsync(
                    d => d.Id==deadlineId,
                    includes: q => q.Include(d => d.Branch));

                if (deadline == null) return false;

                deadline.Status = newStatus;
                //коммент

                if (newStatus == ReportStatus.Reviewed)
                {
                    // Помечаем текущий дедлайн как закрытый
                    deadline.IsClosed = true;
                    deadline.ReportId = reportId; // Связываем с отчетом

                    // Создаем новый дедлайн вместо обновления
                    await _deadlineService.CheckAndUpdateDeadlineAsync(
                        report.TemplateId,
                        (int)report.BranchId);

                    var users = await _unitOfWork.Users.FindAllAsync(
                        u => u.BranchId == report.BranchId);

                    await _notificationService.AddNotificationAsync(
                        (int)report.UploadedById,
                        $"Отчет '{report.Name}' за период '{report.Period:yyyy-MM-dd}' был принят.");
                }

                await _unitOfWork.SubmissionDeadlines.UpdateAsync(deadline);
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> AddReportCommentAsync(int deadlinId, int reportId, string comment, int? authorId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var report = await _unitOfWork.Reports.FindAsync(r => r.Id == reportId);
                if (report == null) return false;

                var deadline = await _unitOfWork.SubmissionDeadlines.FindAsync(
                    d => d.Id==deadlinId);

                if (deadline == null) return false;

                deadline.Status = ReportStatus.NeedsCorrection;

                var commentHistory = new CommentHistory
                {
                    Comment = comment,
                    CreatedAt = DateTime.Now,
                    DeadlineId = deadline.Id,
                    ReportId = deadline.ReportId,
                    AuthorId = authorId
                };

                await _unitOfWork.CommentHistory.AddAsync(commentHistory);

                await _notificationService.AddNotificationAsync(
                    (int)report.UploadedById,
                    $"{report.Name}: {comment}");

                await _unitOfWork.SubmissionDeadlines.UpdateAsync(deadline);
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<byte[]> GetReportFileAsync(int reportId)
        {
            var report = await _unitOfWork.Reports.FindAsync(r => r.Id == reportId);
            if (report == null || string.IsNullOrEmpty(report.FilePath))
                throw new FileNotFoundException("Файл отчета не найден");

            return await _fileService.GetFileAsync(report.FilePath);
        }

        public async Task<List<PendingTemplateDto>> GetPendingTemplatesAsync(int? branchId)
        {
            var query = _unitOfWork.SubmissionDeadlines
                .GetAll(q => q
                    .Include(t => t.Template)
                    .Include(d => d.CommentHistory)); // Подгружаем историю комментариев

            if (branchId.HasValue)
                query = query.Where(d => d.BranchId == branchId.Value && !d.IsClosed);

            var deadlines = await query.ToListAsync();

            return deadlines.Select(d => new PendingTemplateDto
            {
                Id = d.Id,
                TemplateId = d.ReportTemplateId,
                TemplateName = d.Template?.Name ?? "Неизвестный шаблон",
                Deadline = d.DeadlineDate,
                ReportId = d.ReportId,
                Status = (ReportStatus)d.Status,
                Comment = d.Comment,
                ReportType = d.Template?.Type.ToString(),
                BranchId = d.BranchId,
                CommentHistory = d.CommentHistory?
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CommentHistoryDto
                    {
                        CreatedAt = c.CreatedAt,
                        Comment = c.Comment
                    })
                    .ToList() ?? new List<CommentHistoryDto>() // защита от null
            }).ToList();
        }


        public async Task<Dictionary<string, Dictionary<string, List<List<string>>>>> ReadExcelFileAsync(int reportId)
        {
            var report = await _unitOfWork.Reports.FindAsync(r => r.Id == reportId);
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

        public async Task<IEnumerable<ReportDto>> GetFilteredReportsAsync(
            string? name, int? templateId, int? branchId, DateTime? startDate, DateTime? endDate, ReportType? reportType)
        {
            var query = _unitOfWork.Reports.GetAll(includes:r=>r.Include(d=>d.Branch));
            query = query.Where(rp => rp.IsClosed);
            if (!string.IsNullOrEmpty(name))
                query = query.Where(r => r.Name.Contains(name));

            if (templateId.HasValue)
                query = query.Where(r => r.TemplateId == templateId.Value);

            if (branchId.HasValue)
                query = query.Where(r => r.BranchId == branchId.Value);

            if (startDate.HasValue)
                query = query.Where(r => r.Period >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.Period <= endDate.Value);

            if (reportType.HasValue)
                query = query.Where(r => r.Type == reportType.Value);

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
                Period = r.Period,
                Type = r.Type
            }).ToList();
        }

        public async Task<ReportDto> ReopenReportAsync(int reportid)
        {
            var report = await _unitOfWork.Reports.FindAsync(r => r.Id == reportid);
            var deadline = await _unitOfWork.SubmissionDeadlines.FindAsync(d => d.ReportId == reportid);
            if (report == null|| deadline==null) return null;
            report.IsClosed = false;
            deadline.IsClosed = false;
            await _unitOfWork.Reports.UpdateAsync(report);
            return MapToDto(report);
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
                Period = report.Period,
                UploadDate = report.UploadDate,
                Type =report.Type
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
                Period = reportDto.Period,
                Type = reportDto.Type
            };
        }
    }
}
