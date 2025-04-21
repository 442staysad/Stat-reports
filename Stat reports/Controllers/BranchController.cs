﻿using Core.DTO;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stat_reports.ViewModels;

namespace Stat_reports.Controllers
{
    [Authorize(Roles = "Admin,PEB,OBUnF")]
    public class BranchController : Controller
    {
        private readonly IBranchService _branchService;
        public BranchController(IBranchService branchService) =>
            _branchService = branchService;
        
        public async Task<IActionResult> Index()
        {
            var dtos = await _branchService.GetAllBranchesDtosAsync();
            return View(dtos);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Добавить филиал";
            return View(new BranchCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(BranchCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var dto = new BranchDto
            {
                Name = vm.Name,
                Shortname = vm.Shortname,
                UNP = vm.UNP,
                OKPO = vm.OKPO,
                OKYLP = vm.OKYLP,
                Region = vm.Region,
                Address = vm.Address,
                Email = vm.Email,
                GoverningName = vm.GoverningName,
                HeadName = vm.HeadName,
                Supervisor = vm.Supervisor,
                ChiefAccountant = vm.ChiefAccountant,
                Password = vm.Password
            };
            await _branchService.CreateBranchAsync(dto);
            TempData["Success"] = "Филиал создан";
            return RedirectToAction(nameof(Index));
        }
    }
}
