using System.Threading.Tasks;
using Core.DTO;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stat_reports.ViewModels;
using Stat_reportsnt.Filters;
namespace Stat_reports.Controllers
{
    [AuthorizeBranchAndUser]
    public class ReportMvcController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IDeadlineService _deadlineService;
        private readonly IBranchService _branchService;
        private readonly IReportTemplateService _reportTemplateService;
        private readonly IFileService _fileService;

        public ReportMvcController(IReportService reportService, 
            IDeadlineService deadlineService,
            IBranchService branchService,
            IReportTemplateService reportTemplateService,
            IFileService fileService)
        {
            _reportService = reportService;
            _deadlineService = deadlineService;
            _branchService = branchService;
            _reportTemplateService = reportTemplateService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            if (!HttpContext.Session.TryGetValue("BranchId", out var branchIdBytes))
            {
                return RedirectToAction("BranchLogin", "Auth"); // Перенаправляем на вход филиала
            }

            int branchId = BitConverter.ToInt32(branchIdBytes, 0);
            var reports = await _reportService.GetReportsByBranchAsync(branchId);
            return View(reports);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadReport(int templateId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Файл не выбран";
                return RedirectToAction(nameof(WorkingReports));
            }

            // Получаем текущие UserId и BranchId из сессии
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? branchId = HttpContext.Session.GetInt32("BranchId");

            if (userId == null || branchId == null)
            {
                TempData["Error"] = "Ошибка авторизации";
                return RedirectToAction(nameof(WorkingReports));
            }

            await _reportService.UploadReportAsync(templateId, branchId.Value, userId.Value, file);
            TempData["Success"] = "Отчет успешно загружен";

            return RedirectToAction(nameof(WorkingReports));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var report = await _reportService.GetReportByIdAsync(id);
            return report == null ? NotFound() : View(report);
        }


        public async Task<IActionResult> PreviewExcel(int reportId, int? deadlineId , bool isArchive = false)
        {
            var report = await _reportService.GetReportByIdAsync(reportId);
            if (report == null || string.IsNullOrEmpty(report.FilePath))
                return NotFound("Файл отчета не найден");

            var branch = await _branchService.GetBranchByIdAsync(report.BranchId);
            var excelData = await _reportService.ReadExcelFileAsync(reportId);

            var model = new ExcelPreviewViewModel
            {
                ReportType = report.Type == ReportType.Accountant ? "OBUnF" : "PEB",
                BranchName = branch.Name,
                DeadlineId = deadlineId,
                ReportId = reportId,
                ReportName = report.Name,
                ExcelData = excelData,
                Comment = report.Comment,
                Status = report.Status,
                CommentHistory = report.CommentHistory,
                IsArchive = isArchive 
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,PEB,OBUnF")]
        public async Task<IActionResult> AddComment(int deadlineId, int reportId, string comment)
        {
            await _reportService.AddReportCommentAsync(deadlineId,reportId, comment, HttpContext.Session.GetInt32("UserId"));
            //await _reportService.UpdateReportStatusAsync(reportId, ReportStatus.NeedsCorrection,comment);
            return RedirectToAction(nameof(WorkingReports));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,PEB,OBUnF")]
        public IActionResult CreateTemplate()
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("PEB") && !User.IsInRole("OBUnF"))
                return Forbid();

            ViewBag.AllowedTypes = GetAllowedReportTypes();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,PEB,OBUnF")]
        public async Task<IActionResult> CreateTemplate(CreateTemplateViewModel model)
        {/*
            if (!ModelState.IsValid)
            {
                ViewBag.AllowedTypes = GetAllowedReportTypes();
                return View(model);
            }*/

            
            string filePath = null;
            if (model.File != null)
            {
                filePath = await _fileService.SaveFileAsync(model.File, "Templates");
            }

            var template = new ReportTemplate
            {
                Name = model.Name,
                Description = model.Description,
                FilePath = filePath, // сюда кладем путь к загруженному файлу
                Type = model.Type,
                DeadlineType = model.DeadlineType
            };

            await _reportTemplateService.CreateReportTemplateAsync(template, model.DeadlineType, model.FixedDay, model.ReportDate);

            return RedirectToAction("WorkingReports");
        }

        private List<ReportType> GetAllowedReportTypes()
        {
            var types = new List<ReportType>();

            if (User.IsInRole("Admin"))
            {
                types.Add(ReportType.Plan);
                types.Add(ReportType.Accountant);
            }
            else if (User.IsInRole("PEB"))
            {
                types.Add(ReportType.Plan);
            }
            else if (User.IsInRole("OBUnF"))
            {
                types.Add(ReportType.Accountant);
            }

            return types;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,PEB,OBUnF")]
        public async Task<IActionResult> AcceptReport(int deadlineId, int reportId)
        {
            await _reportService.UpdateReportStatusAsync(deadlineId,reportId, ReportStatus.Reviewed);
            return RedirectToAction(nameof(WorkingReports));
        }

        public async Task<IActionResult> WorkingReports()
        {
            int? sessionBranchId = HttpContext.Session.GetInt32("BranchId");

            bool isGlobalUser = User.IsInRole("Admin") || User.IsInRole("PEB") || User.IsInRole("OBUnF");

            // Только если не глобальный пользователь — ограничиваем по филиалу
            int? branchId = isGlobalUser ? null : sessionBranchId;

            var templates = await _reportService.GetPendingTemplatesAsync(branchId);

            var branches = await _branchService.GetAllBranchesAsync();

            var viewModel = templates.Select(t => new PendingTemplateViewModel
            {
                DeadlineId = t.Id,
                TemplateId = t.TemplateId,
                TemplateName = t.TemplateName,
                Deadline = t.Deadline,
                Status = t.Status,
                Comment = t.Comment,
                ReportId = t.ReportId,
                ReportType = t.ReportType,
                BranchId = (int)t.BranchId,
                BranchName = branches.FirstOrDefault(b => b.Id == t.BranchId)?.Name ?? "Неизвестный филиал"
            }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ReportDto reportDto)
        {
            if (!ModelState.IsValid)
                return View(reportDto);

            await _reportService.UpdateReportAsync(id, reportDto);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            await _reportService.DeleteReportAsync(id);
            return RedirectToAction( "ReportArchive","ReportMvc");
        }

        [HttpPost]
        [Authorize(Roles = "Admin,PEB,OBUnF")]
        public async Task<IActionResult> ReopenReport(int reportId)
        {
            await _reportService.ReopenReportAsync(reportId);
            return View();
        }

        [HttpGet("download/{reportId}")]
        public async Task<IActionResult> DownloadReport(int reportId, string reportname)
        {
            var fileBytes = await _reportService.GetReportFileAsync(reportId);
            return fileBytes == null ? NotFound() : File(fileBytes, "application/octet-stream", $"{reportname}.xls");
        }

        public async Task<IActionResult> ReportArchive(string? name, int? templateId, 
            int? branchId,
            DateTime? startDate, DateTime? endDate, ReportType? reportType)
        {
            int? sessionBranchId = HttpContext.Session.GetInt32("BranchId");
            if(User.IsInRole("User")) 
                branchId = sessionBranchId;

            var reports = await _reportService.GetFilteredReportsAsync(name, templateId, branchId, startDate, endDate, reportType);
            var branches = await _branchService.GetAllBranchesAsync();
            var templates = await _reportTemplateService.GetAllReportTemplatesAsync();

            var model = new ReportArchiveViewModel
            {
                Reports = reports,
                Branches = branches,
                Templates = templates,
                Filter = new ReportFilterViewModel
                {
                    Name = name,
                    TemplateId = templateId,
                    BranchId = branchId,
                    StartDate = startDate,
                    EndDate = endDate,
                    Type = reportType ?? null // или null, если Type тоже Nullable
                }
            };

            return View(model);
        }

    }
}
