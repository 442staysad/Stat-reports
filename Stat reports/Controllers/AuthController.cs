using System.Threading.Tasks;
using Core.Interfaces;
using Stat_reports.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Stat_reports.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult BranchLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BranchLogin(BranchLoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var branch = await _authService.AuthenticateBranchAsync(model.UNP, model.Password);
            if (branch == null)
            {
                ModelState.AddModelError("", "Неверные УНП или пароль.");
                return View(model);
            }

            HttpContext.Session.SetInt32("BranchId", branch.Id);
            return RedirectToAction("UserLogin");
        }

        [HttpGet]
        public IActionResult UserLogin()
        {
            if (HttpContext.Session.GetInt32("BranchId") == null)
                return RedirectToAction("BranchLogin");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var branchId = HttpContext.Session.GetInt32("BranchId");
            if (branchId == null)
                return RedirectToAction("BranchLogin");

            var user = await _authService.AuthenticateUserAsync(branchId.Value, model.Username, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Неверные имя пользователя или пароль.");
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("BranchLogin");
        }
    }
}