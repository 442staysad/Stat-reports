using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stat_reports.Models;
using Core.Interfaces;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
namespace Stat_reports.Controllers
{
    public class AdminController : Controller
    {
        private readonly IRepository<Branch> _branchRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<ReportTemplate> _templateRepository;
        private readonly IRepository<Report> _reportRepository;
        private readonly IPasswordHasher<User> _userPasswordHasher;
        private readonly IPasswordHasher<Branch> _branchPasswordHasher;
        private readonly IAuthService _authService;
     
        public AdminController(IAuthService authService, IRepository<Branch> branchRepository,
            IRepository<User> userRepository,
            IRepository<ReportTemplate> templateRepository,
            IRepository<Report> reportRepository,
            IPasswordHasher<User> userPasswordHasher,
            IPasswordHasher<Branch> branchPasswordHasher) {
        _authService = authService;
            _branchRepository = branchRepository;
            _userRepository = userRepository;
            _templateRepository = templateRepository;
            _reportRepository = reportRepository;
            _userPasswordHasher = userPasswordHasher;
            _branchPasswordHasher = branchPasswordHasher;
        }

        // ==== Управление филиалами ====
        public async Task<IActionResult> Branches()
        {
            var branches = await _branchRepository.GetAllAsync();
            return View(branches);
        }

        public IActionResult CreateBranch() => View();

        [HttpPost]
        public async Task<IActionResult> CreateBranch(Branch model)
        {
            if (!ModelState.IsValid) return View(model);

            model.PasswordHash = _branchPasswordHasher.HashPassword(model, model.PasswordHash);
            await _branchRepository.AddAsync(model);
            return RedirectToAction("Branches");
        }
        /*
        public async Task<IActionResult> EditBranch(int id)
        {
            var branch = await _branchRepository.FindAsync(id);
            return branch == null ? NotFound() : View(branch);
        }*/

     /*   [HttpPost]
        public async Task<IActionResult> EditBranch(Branch model)
        {
            if (!ModelState.IsValid) return View(model);

            var branch = await _branchRepository.GetByIdAsync(model.Id);
            if (branch == null) return NotFound();

            branch.Name = model.Name;
            branch.UNP = model.UNP;
            branch.Email = model.Email;
            if (!string.IsNullOrEmpty(model.PasswordHash))
                branch.PasswordHash = _branchPasswordHasher.HashPassword(branch, model.PasswordHash);

            await _branchRepository.UpdateAsync(branch);
            return RedirectToAction("Branches");
        }*/
     /*
        public async Task<IActionResult> DeleteBranch(int id)
        {
            await _branchRepository.DeleteAsync(id);
            return RedirectToAction("Branches");
        }*/

        // ==== Управление пользователями ====
        public async Task<IActionResult> Users()
        {
            var users = await _userRepository.GetAllAsync();
            return View(users);
        }

        public IActionResult CreateUser() => View();

        [HttpPost]
        public async Task<IActionResult> CreateUser(User model)
        {
            if (!ModelState.IsValid) return View(model);

            model.PasswordHash = _userPasswordHasher.HashPassword(model, model.PasswordHash);
            await _userRepository.AddAsync(model);
            return RedirectToAction("Users");
        }
        /*
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? NotFound() : View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(User model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userRepository.GetByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.UserName = model.UserName;
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Position = model.Position;
            if (!string.IsNullOrEmpty(model.PasswordHash))
                user.PasswordHash = _userPasswordHasher.HashPassword(user, model.PasswordHash);

            await _userRepository.UpdateAsync(user);
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userRepository.DeleteAsync(id);
            return RedirectToAction("Users");
        }*/

        // ==== Управление шаблонами отчетов ====
        public async Task<IActionResult> Templates()
        {
            var templates = await _templateRepository.GetAllAsync();
            return View(templates);
        }

        public IActionResult CreateTemplate() => View();

        [HttpPost]
        public async Task<IActionResult> CreateTemplate(ReportTemplate model)
        {
            if (!ModelState.IsValid) return View(model);

            await _templateRepository.AddAsync(model);
            return RedirectToAction("Templates");
        }
        /*
        public async Task<IActionResult> EditTemplate(int id)
        {
            var template = await _templateRepository.GetByIdAsync(id);
            return template == null ? NotFound() : View(template);
        }

        [HttpPost]
        public async Task<IActionResult> EditTemplate(ReportTemplate model)
        {
            if (!ModelState.IsValid) return View(model);

            var template = await _templateRepository.GetByIdAsync(model.Id);
            if (template == null) return NotFound();

            template.Name = model.Name;
            template.Description = model.Description;
            template.Fields = model.Fields;

            await _templateRepository.UpdateAsync(template);
            return RedirectToAction("Templates");
        }

        public async Task<IActionResult> DeleteTemplate(int id)
        {
            await _templateRepository.DeleteAsync(id);
            return RedirectToAction("Templates");
        }
        
        // ==== Управление отчетами ====
        public async Task<IActionResult> Reports()
        {
            var reports = await _reportRepository.GetAllAsync();
            return View(reports);
        }

        public async Task<IActionResult> DeleteReport(int id)
        {
            await _reportRepository.DeleteAsync(id);
            return RedirectToAction("Reports");
        }
        */
        [HttpPost]
        public async Task<IActionResult> RegisterBranch(RegisterBranchModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var branch = await _authService.RegisterBranchAsync(model.Name, model.UNP, model.Password);
            if (branch == null)
            {
                ModelState.AddModelError("", "Филиал с таким УНП уже существует.");
                return View(model);
            }

            return RedirectToAction("BranchLogin");
        }

        [HttpGet]
        public IActionResult RegisterUser()
        {
            if (HttpContext.Session.GetInt32("BranchId") == null)
                return RedirectToAction("BranchLogin");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var branchId = HttpContext.Session.GetInt32("BranchId");
            if (branchId == null)
                return RedirectToAction("BranchLogin");

            var user = await _authService.RegisterUserAsync(branchId.Value, model.Username, model.FullName, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Пользователь с таким именем уже существует.");
                return View(model);
            }

            return RedirectToAction("UserLogin");
        }
    }
}
