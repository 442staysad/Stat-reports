using Core.Entities;
using Core.Interfaces;
using Core.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stat_reports.Models;

namespace Stat_reports.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminReportTemplateController : Controller
    {
        private readonly IReportTemplateService _reportTemplateService;
        private readonly IDeadlineService _deadlineService;
        private readonly IFileService _fileService;
        private readonly ILogger<AdminReportTemplateController> _logger;

        public AdminReportTemplateController(IReportTemplateService reportTemplateService, IFileService fileService, IDeadlineService deadlineService, ILogger<AdminReportTemplateController> logger)
        {
            _reportTemplateService = reportTemplateService;
            _fileService = fileService;
            _deadlineService = deadlineService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var reportTemplates = await _reportTemplateService.GetAllReportTemplatesAsync();
            return View(reportTemplates);
        }

        public IActionResult Create() => View();

       [HttpPost]
        public async Task<IActionResult> Create(ReportTemplateModel model, IFormFile file)
        {
            // Сохранение файла
            string filePath = null;
            if (file != null && file.Length > 0)
            {
                filePath = await _fileService.SaveFileAsync(file, "Templates");
            }

            // Создание шаблона отчета
            var reportTemplate = new ReportTemplate
            {
                Name = model.Name,
                Description = model.Description,
                FilePath = filePath,
                
            };

            // Создание шаблона в базе данных
            var createdTemplate = await _reportTemplateService.
                CreateReportTemplateAsync(reportTemplate,model.DeadlineType,(int)model.FixedDay,(DateTime)model.ReportDate);



            // Перенаправление на страницу списка шаблонов
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ReportTemplate = await _reportTemplateService.GetReportTemplateByIdAsync(id);
            ReportTemplateModel model = new ReportTemplateModel
            {
                Id = ReportTemplate.Id,
                Name = ReportTemplate.Name,
                Description = ReportTemplate.Description,
                //SubmissionDeadline = ReportTemplate.SubmissionDeadline,
                //File = (IFormFile)_fileService.GetFileAsync(ReportTemplate.FilePath)
            };
            if (ReportTemplate == null)
                return NotFound();
            return View(ReportTemplate);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ReportTemplateModel model, IFormFile FilePath)
        {
            if (id != model.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                var existingTemplate = await _reportTemplateService.GetReportTemplateByIdAsync(id);
                if (existingTemplate == null)
                    return NotFound();

                if (FilePath != null && FilePath.Length > 0)
                {
                    // Удаляем старый файл
                    if (!string.IsNullOrEmpty(existingTemplate.FilePath))
                    {
                        await _fileService.DeleteFileAsync(existingTemplate.FilePath);
                    }

                    // Сохраняем новый файл
                    existingTemplate.FilePath = await _fileService.SaveFileAsync(FilePath, "Templates");
                }

                existingTemplate.Name = model.Name;
                existingTemplate.Description = model.Description;


                await _reportTemplateService.UpdateReportTemplateAsync(existingTemplate);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var template = await _reportTemplateService.GetReportTemplateByIdAsync(id);
            if (template == null)
                return NotFound();
            return View(template);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _reportTemplateService.DeleteReportTemplateAsync(id);                       
            if (success)
                return RedirectToAction(nameof(Index));
            return NotFound();
        }

    }
}
