using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Stat_reports.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IBranchService _branchService;
        private readonly IReportTemplateService _reportTemplateService;
        private readonly IFileService _fileService;

        public AdminReportController(
            IReportService reportService,
            IBranchService branchService,
            IReportTemplateService reportTemplateService,
            IFileService fileService)
        {
            _branchService = branchService;
            _reportService = reportService;
            _reportTemplateService = reportTemplateService;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var reports = await _reportService.GetAllReportsAsync();
            return View(reports);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Branches = await _branchService.GetAllBranchesAsync();
            ViewBag.ReportTemplates = await _reportTemplateService.GetAllReportTemplatesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Report report, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = await _branchService.GetAllBranchesAsync();
                ViewBag.ReportTemplates = await _reportTemplateService.GetAllReportTemplatesAsync();
                return View(report);
            }

            // Получаем данные филиала и шаблона
            var branch = await _branchService.GetBranchByIdAsync(report.BranchId);
            var template = await _reportTemplateService.GetReportTemplateByIdAsync(report.TemplateId);

            if (branch == null || template == null)
            {
                ModelState.AddModelError("", "Ошибка: филиал или шаблон не найдены.");
                return View(report);
            }

            // Определяем путь сохранения
            int currentYear = DateTime.Now.Year;
            string filePath = null;

            if (file != null && file.Length > 0)
            {
                string? name = branch.Name;
                filePath = await _fileService.SaveFileAsync(file, "Reports", name, currentYear, template.Name);
            }

            report.FilePath = filePath;
            report.UploadDate = DateTime.UtcNow;

            await _reportService.CreateReportAsync(report);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var report = await _reportService.GetReportByIdAsync(id);
            if (report == null)
                return NotFound();

            ViewBag.Branches = await _branchService.GetAllBranchesAsync();
            ViewBag.ReportTemplates = await _reportTemplateService.GetAllReportTemplatesAsync();
            return View(report);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Report report, IFormFile file)
        {
            if (id != report.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Branches = await _branchService.GetAllBranchesAsync();
                ViewBag.ReportTemplates = await _reportTemplateService.GetAllReportTemplatesAsync();
                return View(report);
            }

            var branch = await _branchService.GetBranchByIdAsync(report.BranchId);
            var template = await _reportTemplateService.GetReportTemplateByIdAsync(report.TemplateId);

            if (branch == null || template == null)
            {
                ModelState.AddModelError("", "Ошибка: филиал или шаблон не найдены.");
                return View(report);
            }

            // Если загружен новый файл — сохраняем, старый можно удалить при необходимости
            if (file != null && file.Length > 0)
            {
                var newFilePath = await _fileService.SaveFileAsync(file, "Reports", branch.Name, DateTime.Now.Year, template.Name);
                if (!string.IsNullOrEmpty(report.FilePath))
                {
                    await _fileService.DeleteFileAsync(report.FilePath);
                }
                report.FilePath = newFilePath;
            }

            await _reportService.UpdateReportAsync(report);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
            {
                var Report = await _reportService.GetReportByIdAsync(id);
                if (Report == null)
                    return NotFound();
                return View(Report);
            }

            [HttpPost, ActionName("Delete")]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var success = await _reportService.DeleteReportAsync(id);
                if (success)
                    return RedirectToAction(nameof(Index));
                return NotFound();
            }
        
    }
}

