using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MentorDashboardApp.Models;
using MentorDashboardApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
namespace MentorDashboardApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public async Task<IActionResult> Index([FromServices] ApplicationDbContext context)
    {
        var courses = await context.Courses
            .Where(c => c.IsFeatured || c.Status == "Published")
            .OrderByDescending(c => c.CreatedAt)
            .Take(6) // Only show 6 featured courses on home page
            .Include(c => c.Trainer)
            .ToListAsync();

        return View(courses);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
