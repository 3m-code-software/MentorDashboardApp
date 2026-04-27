using MentorDashboardApp.Data;
using MentorDashboardApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentorDashboardApp.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var trainers = await _context.Users
                .Where(u => u.Role == "Trainer" && u.IsActive)
                .ToListAsync();

            return View(trainers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var trainer = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id && u.Role == "Trainer");
            if (trainer == null)
            {
                return NotFound();
            }

            var courses = await _context.Courses
                .Where(c => c.TrainerId == id && c.Status == "Published")
                .ToListAsync();

            ViewBag.TrainerCourses = courses;

            return View(trainer);
        }
    }
}
