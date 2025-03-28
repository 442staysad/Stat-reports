using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stat_reports.Models;

namespace Stat_reports.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IBranchService _branchService;
        public UserController(IUserService userService,IBranchService branchService)
        {
            _branchService = branchService;
            _userService = userService;
        }


        public async Task<IActionResult> Index()
        {
            var branches = await _userService.GetAllUsersAsync();
            return View(branches);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Branches= await _branchService.GetAllBranchesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserModel userm)
        {
            User user = new User
            {
                UserName = userm.UserName,
                FullName = userm.FullName,
                Role = userm.Role,
                BranchId = userm.BranchId,
                Branch = userm.Branch,
                Email = userm.Email,
                Number = userm.Number,
                Position = userm.Position
            };
            if (ModelState.IsValid)
            {
                await _userService.CreateUserAsync(user);
                return RedirectToAction(nameof(Index));
            }
            return View(userm);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Branches = await _branchService.GetAllBranchesAsync();
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _userService.UpdateUserAsync(user);
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var branch = await _userService.GetUserByIdAsync(id);
            if (branch == null)
                return NotFound();
            return View(branch);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (success)
                return RedirectToAction(nameof(Index));
            return NotFound();
        }
    }
}