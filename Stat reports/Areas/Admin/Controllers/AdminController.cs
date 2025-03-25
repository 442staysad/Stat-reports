using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stat_reports.Models;
using Core.Interfaces;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
namespace Stat_reports.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {

        private readonly IBranchService branchService;
        private readonly IUserService userService;
        private readonly IReportService reportService;
        private readonly IReportTemplateService reportTemplateService;
     
        public AdminController(IBranchService _branchService, IUserService _userService, 
            IReportService _reportService, IReportTemplateService _reportTemplateService) {
            branchService = _branchService;
            userService = _userService;
            reportTemplateService = _reportTemplateService;    
            reportService = _reportService;

        

        }

        public IActionResult Index()
        {
            // Перенаправление на страницу в области Admin
            return View();
        }

    }
}
