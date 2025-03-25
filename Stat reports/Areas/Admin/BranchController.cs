using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StatReports.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BranchController : Controller
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        public async Task<IActionResult> Index()
        {
            var branches = await _branchService.GetAllBranchesAsync();
            return View(branches);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Branch branch)
        {
            if (ModelState.IsValid)
            {
                await _branchService.CreateBranchAsync(branch);
                return RedirectToAction(nameof(Index));
            }
            return View(branch);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            if (branch == null)
                return NotFound();
            return View(branch);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Branch branch)
        {
            if (id != branch.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _branchService.UpdateBranchAsync(branch);
                return RedirectToAction(nameof(Index));
            }
            return View(branch);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            if (branch == null)
                return NotFound();
            return View(branch);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _branchService.DeleteBranchAsync(id);
            if (success)
                return RedirectToAction(nameof(Index));
            return NotFound();
        }
    }
}