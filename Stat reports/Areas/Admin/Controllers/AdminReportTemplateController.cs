using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Stat_reports.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminReportTemplateController:Controller
    {
        private readonly IReportTemplateService _ReportTemplateTemplateService;

        public AdminReportTemplateController(IReportTemplateService ReportTemplateService)
        {
            _ReportTemplateTemplateService = ReportTemplateService;
        }

        public async Task<IActionResult> Index()
        {
            var ReportTemplatees = await _ReportTemplateTemplateService.GetAllReportTemplatesAsync();
            return View(ReportTemplatees);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(ReportTemplate ReportTemplate)
        {
            if (ModelState.IsValid)
            {
                await _ReportTemplateTemplateService.CreateReportTemplateAsync(ReportTemplate);
                return RedirectToAction(nameof(Index));
            }
            return View(ReportTemplate);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ReportTemplate = await _ReportTemplateTemplateService.GetReportTemplateByIdAsync(id);
            if (ReportTemplate == null)
                return NotFound();
            return View(ReportTemplate);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ReportTemplate ReportTemplate)
        {
            if (id != ReportTemplate.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _ReportTemplateTemplateService.UpdateReportTemplateAsync(ReportTemplate);
                return RedirectToAction(nameof(Index));
            }
            return View(ReportTemplate);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var ReportTemplate = await _ReportTemplateTemplateService.GetReportTemplateByIdAsync(id);
            if (ReportTemplate == null)
                return NotFound();
            return View(ReportTemplate);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _ReportTemplateTemplateService.DeleteReportTemplateAsync(id);                       
            if (success)
                return RedirectToAction(nameof(Index));
            return NotFound();
        }

    }
}
