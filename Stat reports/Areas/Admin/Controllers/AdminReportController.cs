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
            public AdminReportController(IReportService reportService, IBranchService branchService)
            {
                _branchService = branchService;
                _reportService = reportService;
            }

            public async Task<IActionResult> Index()
            {
                var Reportes = await _reportService.GetAllReportsAsync();
                return View(Reportes);
            }

            public async Task<IActionResult> Create() 
            {

                ViewBag.Branches = await _branchService.GetAllBranchesAsync();
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> Create(Report Report)
            {
                if (ModelState.IsValid)
                {
                    await _reportService.CreateReportAsync(Report);
                    return RedirectToAction(nameof(Index));
                }
                return View(Report);
            }

            public async Task<IActionResult> Edit(int id)
            {
             ViewBag.Branches = await _branchService.GetAllBranchesAsync();
                var Report = await _reportService.GetReportByIdAsync(id);
                if (Report == null)
                    return NotFound();
                return View(Report);
            }

            [HttpPost]
            public async Task<IActionResult> Edit(int id, Report Report)
            {
                if (id != Report.Id)
                    return BadRequest();

                if (ModelState.IsValid)
                {
                    await _reportService.UpdateReportAsync(Report);
                    return RedirectToAction(nameof(Index));
                }
                return View(Report);
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

