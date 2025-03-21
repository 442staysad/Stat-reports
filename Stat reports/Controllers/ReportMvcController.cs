using System.Threading.Tasks;
using Core.DTO;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stat_reports.ViewModels;
namespace Stat_reports.Controllers
{
    public class ReportMvcController : Controller
    {
        private readonly IReportService _reportService;

        public ReportMvcController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            var reports = await _reportService.GetAllReportsAsync();
            return View(reports);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int templateId, int branchId, int uploadedById, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Файл не выбран");
                return View();
            }

            await _reportService.UploadReportAsync(templateId, branchId, uploadedById, file);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var report = await _reportService.GetReportByIdAsync(id);
            return report == null ? NotFound() : View(report);
        }

        public async Task<IActionResult> Preview(int id)
        {
            var report = await _reportService.GetReportByIdAsync(id);
            if (report == null || string.IsNullOrEmpty(report.FilePath))
                return NotFound("Файл отчета не найден");

            var fileBytes = await _reportService.DownloadReportAsync(id);
            if (fileBytes == null)
                return NotFound("Не удалось загрузить файл");

            var model = new ReportPreviewViewModel
            {
                ReportId = id,
                ReportName = report.Name,
                FileContent = fileBytes
            };

            return View(model);
        }
        public async Task<IActionResult> WorkingReports()
        {
            var templates = await _reportService.GetPendingTemplatesAsync();
            var viewModel = templates.Select(t => new PendingTemplateViewModel
            {
                TemplateId = t.TemplateId,
                TemplateName = t.TemplateName,
                Deadline = t.Deadline,
                Status = t.Status.ToString()
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
