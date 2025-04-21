using Core.DTO;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stat_reports.Models;

namespace Stat_reports.Areas.Admin.Controllers
{
    [Area("Admin")]
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
        public async Task<IActionResult> Create(BranchModel branchmodel)
        {

                BranchDto branch = new()
                {
                    Name = branchmodel.Name,
                    Address = branchmodel.Address,
                    HeadName = branchmodel.HeadName,
                    ChiefAccountant = branchmodel.ChiefAccountant,
                    Email = branchmodel.Email,
                    GoverningName = branchmodel.GoverningName,
                    OKPO = branchmodel.OKPO,
                    OKYLP = branchmodel.OKYLP,
                    Region = branchmodel.Region,
                    Shortname = branchmodel.Shortname,
                    Supervisor = branchmodel.Supervisor,
                    UNP = branchmodel.UNP,
                    Password = branchmodel.PasswordHash
                };
                await _branchService.CreateBranchAsync(branch);
                return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Edit(int id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            if (branch == null)
                return NotFound();
            return View(branch);
        }
        /*
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
        }*/

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