using System.Diagnostics;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stat_reports.Models;

namespace Stat_reports.Controllers;

public class HomeController : Controller
{
    private readonly IPostService _postService;


    public HomeController(IPostService postService)
    {
        _postService = postService;
       
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _postService.GetRecentPostsForUserAsync();
        return View(posts); // Передаем список Post
    }

    [Authorize(Roles = "Admin,OBUnF,PEB")]
    [HttpPost]
    public async Task<IActionResult> AddPost(string header, string text)
    {
        var user = HttpContext.Session.GetInt32("UserId");
        if (user != null)
        {
            await _postService.AddPostAsync(header, text, (int)user);
        }
        return RedirectToAction("Index");
    }
}