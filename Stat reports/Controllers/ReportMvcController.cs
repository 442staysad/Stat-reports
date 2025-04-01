using System.Threading.Tasks;
using Core.DTO;
using Core.Enums;
using Core.Interfaces;
using Core.Services;
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

        public ReportMvcController(IReportService reportService, IDeadlineService deadlineService)
        {
            _reportService = reportService;
            _deadlineService = deadlineService;
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

        public async Task<IActionResult> PreviewExcel(int id)
        {
            var report = await _reportService.GetReportByIdAsync(id);
            if (report == null || string.IsNullOrEmpty(report.FilePath))
                return NotFound("Файл отчета не найден");

            var excelData = await _reportService.ReadExcelFileAsync(id);
            var model = new ExcelPreviewViewModel
            {
                ReportId = id,
                ReportName = report.Name,
                ExcelData = excelData,
                Comment = report.Comment, // Load existing comment
                Status = report.Status // Load existing status
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int reportId, string comment)
        {
            await _reportService.AddReportCommentAsync(reportId, comment);
            await _reportService.UpdateReportStatusAsync(reportId, ReportStatus.NeedsCorrection,comment);
            return RedirectToAction(nameof(PreviewExcel), new { id = reportId });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptReport(int reportId)
        {
            await _reportService.UpdateReportStatusAsync(reportId, ReportStatus.Reviewed);
            var report = await _reportService.GetReportByIdAsync(reportId);
            if (report != null)
            {
                await _deadlineService.CheckAndUpdateDeadlineAsync((int)report.TemplateId);
            }
            return RedirectToAction(nameof(PreviewExcel), new { id = reportId });
        }

        public async Task<IActionResult> WorkingReports()
        {
            int? branchId = HttpContext.Session.GetInt32("BranchId");
            Console.WriteLine(branchId);
            if (branchId == null)
            {
                return Unauthorized(); // Если филиал не найден, блокируем доступ
            }

            var templates = await _reportService.GetPendingTemplatesAsync(branchId.Value);
            Console.WriteLine(branchId+ "   "+templates);
            var viewModel = templates.Select(t => new PendingTemplateViewModel
            {
                TemplateId = t.TemplateId,
                TemplateName = t.TemplateName,
                Deadline = t.Deadline,
                Status = t.Status,
                Comment = t.Comment,
                ReportId = t.ReportId
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


    }
}
