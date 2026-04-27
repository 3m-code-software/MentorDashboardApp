using MentorDashboardApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MentorDashboardApp.Controllers
{
    public class LMSController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LMSController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int studentId)) return RedirectToAction("Login", "Account");

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.UserId == studentId)
                .ToListAsync();

            return View(enrollments);
        }

        public async Task<IActionResult> Player(int courseId, int? lessonId)
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int studentId)) return RedirectToAction("Login", "Account");

            // Verify enrollment
            var isEnrolled = await _context.Enrollments
                .AnyAsync(e => e.CourseId == courseId && e.UserId == studentId);

            if (!isEnrolled)
            {
                return RedirectToAction("Details", "PublicCourses", new { id = courseId });
            }

            var course = await _context.Courses
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null) return NotFound();

            // Order the curriculum
            foreach(var mod in course.Modules)
            {
                mod.Lessons = mod.Lessons.OrderBy(l => l.OrderIndex).ToList();
            }
            course.Modules = course.Modules.OrderBy(m => m.OrderIndex).ToList();

            // Find current lesson
            Models.CourseLesson? currentLesson = null;
            if (lessonId.HasValue)
            {
                currentLesson = course.Modules.SelectMany(m => m.Lessons).FirstOrDefault(l => l.LessonId == lessonId.Value);
            }

            // If no lesson specified, pick the first one
            if (currentLesson == null)
            {
                currentLesson = course.Modules.FirstOrDefault()?.Lessons.FirstOrDefault();
            }

            // Fetch completed lessons
            var completedLessonIds = await _context.StudentLessonProgresses
                .Where(p => p.CourseId == courseId && p.UserId == studentId && p.IsCompleted)
                .Select(p => p.LessonId)
                .ToListAsync();

            ViewBag.CompletedLessonIds = completedLessonIds;
            ViewBag.CurrentLesson = currentLesson;

            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsComplete([FromBody] ProgressDto dto)
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int studentId)) return Json(new { success = false, message = "Not authenticated" });

            var progress = await _context.StudentLessonProgresses
                .FirstOrDefaultAsync(p => p.CourseId == dto.CourseId && p.LessonId == dto.LessonId && p.UserId == studentId);

            if (progress == null)
            {
                progress = new Models.StudentLessonProgress
                {
                    UserId = studentId,
                    CourseId = dto.CourseId,
                    LessonId = dto.LessonId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow
                };
                _context.StudentLessonProgresses.Add(progress);
            }
            else
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
                _context.StudentLessonProgresses.Update(progress);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }

    public class ProgressDto
    {
        public int CourseId { get; set; }
        public int LessonId { get; set; }
    }
}
