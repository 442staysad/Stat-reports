using System.Threading.Tasks;
using Core.DTO;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Services;
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

        public ReportMvcController(IReportService reportService, 
            IDeadlineService deadlineService,
            IBranchService branchService,
            IReportTemplateService reportTemplateService)
        {
            _reportService = reportService;
            _deadlineService = deadlineService;
            _branchService = branchService;
            _reportTemplateService = reportTemplateService;
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

        [Authorize(Roles = "Admin,PEB,OBUnF")]
        public async Task<IActionResult> PreviewExcel(int reportid,int deadlineId)
        {
            var report = await _reportService.GetReportByIdAsync(reportid);
            if (report == null || string.IsNullOrEmpty(report.FilePath))
                return NotFound("Файл отчета не найден");

            var excelData = await _reportService.ReadExcelFileAsync(reportid);
            var model = new ExcelPreviewViewModel
            {
                DeadlineId= deadlineId,
                ReportId = reportid,
                ReportName = report.Name,
                ExcelData = excelData,
                Comment = report.Comment, // Load existing comment
                Status = report.Status // Load existing status
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,PEB,OBUnF")]
        public async Task<IActionResult> AddComment(int reportId, string comment)
        {
            await _reportService.AddReportCommentAsync(reportId, comment);
            //await _reportService.UpdateReportStatusAsync(reportId, ReportStatus.NeedsCorrection,comment);
            return RedirectToAction(nameof(WorkingReports));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,PEB,OBUnF")]
        public async Task<IActionResult> AcceptReport(int reportId)
        {
            await _reportService.UpdateReportStatusAsync(reportId, ReportStatus.Reviewed);
            return RedirectToAction(nameof(WorkingReports));
        }

        public async Task<IActionResult> WorkingReports()
        {
            int? branchId = HttpContext.Session.GetInt32("BranchId");

            if (branchId == null)
            {
                return Unauthorized(); // Если филиал не найден, блокируем доступ
            }

            var templates = await _reportService.GetPendingTemplatesAsync(branchId.Value);

            var branches = await _branchService.GetAllBranchesAsync(); // Получаем список всех филиалов

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
                BranchName = branches.FirstOrDefault(b => b.Id == t.BranchId)?.Name 
                ?? "Неизвестный филиал" // Получаем название филиала
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
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("download/{reportId}")]
        public async Task<IActionResult> DownloadReport(int reportId, string reportname)
        {
            var fileBytes = await _reportService.GetReportFileAsync(reportId);
            return fileBytes == null ? NotFound() : File(fileBytes, "application/octet-stream", $"{reportname}.xls");
        }

        public async Task<IActionResult> ReportArchive(string? name, int? templateId, int? branchId, DateTime? startDate, DateTime? endDate)
        {
            var reports = await _reportService.GetFilteredReportsAsync(name, templateId, branchId, startDate, endDate);
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
                    EndDate = endDate
                }
            };

            return View(model);
        }

    }
}
