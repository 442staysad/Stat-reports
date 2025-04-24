using Core.DTO;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stat_reports.ViewModels;

namespace Stat_reports.Controllers
{
    [Authorize(Roles = "Admin,PEB,OBUnF,User")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IBranchService _branchService;
        private readonly IRoleService _roleService; // сервис, который отдаёт SystemRole

        public UserController(IUserService u, IBranchService b, IRoleService r)
        {
            _userService = u;
            _branchService = b;
            _roleService = r;
        }
        
        public async Task<IActionResult> Index()
        {
            int? sessionBranchId = HttpContext.Session.GetInt32("BranchId");
            bool isGlobal = User.IsInRole("Admin") || User.IsInRole("PEB") || User.IsInRole("OBUnF");
            var dtos = await _userService.GetAllUsersAsync(isGlobal ? null : sessionBranchId);
            return View(dtos);
        }

        [Authorize(Roles = "Admin,PEB,OBUnF,User")]
        public async Task<IActionResult> Create()
        {
            int? sessionBranchId = HttpContext.Session.GetInt32("BranchId");
            bool isGlobal = User.IsInRole("Admin") || User.IsInRole("PEB") || User.IsInRole("OBUnF");

            var vm = new UserCreateViewModel
            {
                RoleOptions = (await _roleService.GetAllRolesAsync())
                                 .Where(r => isGlobal ? true : r.Id == /*User*/1)
                                 .Select(r => new SelectListItem(r.RoleName, r.Id.ToString())),
                BranchOptions = (await _branchService.GetAllBranchesAsync())
                                 .Where(b => isGlobal ? true : b.UNP == /*session*/"")
                                 .Select(b => new SelectListItem(b.Name, b.Id.ToString())),
            };
            return View(vm);
        }

        [HttpPost, Authorize(Roles = "Admin,PEB,OBUnF,User")]
        public async Task<IActionResult> Create(UserCreateViewModel vm)
        {
            int? sessionBranchId = HttpContext.Session.GetInt32("BranchId");
            bool isGlobal = User.IsInRole("Admin") || User.IsInRole("PEB") || User.IsInRole("OBUnF");

            if (!ModelState.IsValid) return View(vm);

            // force branch/role for non-global
            if (!isGlobal)
            {
                vm.BranchId = sessionBranchId!.Value;
                vm.RoleId = (await _roleService.GetRoleByNameAsync("User")).Id;
            }

            var dto = new UserDto
            {
                UserName = vm.UserName,
                FullName = vm.FullName,
                Number = vm.Number,
                Email = vm.Email,
                Position = vm.Position,
                Password = vm.Password,
                RoleId = vm.RoleId,
                BranchId = vm.BranchId
            };
            await _userService.CreateUserAsync(dto);
            TempData["Success"] = "Пользователь создан";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize(Roles = "Admin,PEB,OBUnF")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
