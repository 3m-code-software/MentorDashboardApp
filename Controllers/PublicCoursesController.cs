using MentorDashboardApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentorDashboardApp.Controllers
{
    public class PublicCoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PublicCoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string category, string query)
        {
            var coursesQuery = _context.Courses.AsQueryable();
            
            // Only show published courses
            coursesQuery = coursesQuery.Where(c => c.Status == "Published");

            if (!string.IsNullOrEmpty(category))
            {
                coursesQuery = coursesQuery.Where(c => c.Category == category);
            }

            if (!string.IsNullOrEmpty(query))
            {
                coursesQuery = coursesQuery.Where(c => c.Title.Contains(query) || c.TitleAr.Contains(query));
            }

            var courses = await coursesQuery
                .Include(c => c.Trainer)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.Categories = await _context.Courses
                .Where(c => c.Category != null)
                .Select(c => c.Category)
                .Distinct()
                .ToListAsync();

            return View(courses);
        }

        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Trainer)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            // Order curriculum
            foreach(var mod in course.Modules)
            {
                mod.Lessons = mod.Lessons.OrderBy(l => l.OrderIndex).ToList();
            }
            course.Modules = course.Modules.OrderBy(m => m.OrderIndex).ToList();

            return View(course);
        }
    }
}
