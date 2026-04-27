using MentorDashboardApp.Data;
using MentorDashboardApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentorDashboardApp.Controllers
{
    [Route("Curriculum")]
    public class CurriculumController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CurriculumController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Builder/{courseId}")]
        public async Task<IActionResult> Builder(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null) return NotFound();

            // Sort modules and lessons
            foreach (var module in course.Modules)
            {
                module.Lessons = module.Lessons.OrderBy(l => l.OrderIndex).ToList();
            }
            course.Modules = course.Modules.OrderBy(m => m.OrderIndex).ToList();

            return View(course);
        }

        // --- AJAX Endpoints for Modules ---
        
        [HttpPost("AddModule")]
        public async Task<IActionResult> AddModule(int courseId, string title, string titleAr)
        {
            var maxOrder = await _context.CourseModules
                .Where(m => m.CourseId == courseId)
                .Select(m => (int?)m.OrderIndex)
                .MaxAsync() ?? 0;

            var module = new CourseModule
            {
                CourseId = courseId,
                Title = title,
                TitleAr = titleAr,
                OrderIndex = maxOrder + 1
            };

            _context.CourseModules.Add(module);
            await _context.SaveChangesAsync();

            return Json(new { success = true, module = new { module.ModuleId, module.Title, module.TitleAr, module.OrderIndex } });
        }

        [HttpPost("EditModule")]
        public async Task<IActionResult> EditModule(int moduleId, string title, string titleAr)
        {
            var module = await _context.CourseModules.FindAsync(moduleId);
            if (module == null) return Json(new { success = false, message = "Not found" });

            module.Title = title;
            module.TitleAr = titleAr;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost("DeleteModule")]
        public async Task<IActionResult> DeleteModule(int moduleId)
        {
            var module = await _context.CourseModules.FindAsync(moduleId);
            if (module == null) return Json(new { success = false });

            _context.CourseModules.Remove(module);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost("ReorderModules")]
        public async Task<IActionResult> ReorderModules([FromBody] List<int> moduleIds)
        {
            for (int i = 0; i < moduleIds.Count; i++)
            {
                var m = await _context.CourseModules.FindAsync(moduleIds[i]);
                if (m != null) m.OrderIndex = i + 1;
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // --- AJAX Endpoints for Lessons ---

        [HttpPost("AddLesson")]
        public async Task<IActionResult> AddLesson(int moduleId, string title, string titleAr, string lessonType, string contentUrl, string textContent, int duration, bool isFreePreview)
        {
            var maxOrder = await _context.CourseLessons
                .Where(l => l.ModuleId == moduleId)
                .Select(l => (int?)l.OrderIndex)
                .MaxAsync() ?? 0;

            var lesson = new CourseLesson
            {
                ModuleId = moduleId,
                Title = title,
                TitleAr = titleAr,
                LessonType = lessonType,
                ContentUrl = contentUrl,
                TextContent = textContent,
                DurationMinutes = duration,
                IsFreePreview = isFreePreview,
                IsPublished = true,
                OrderIndex = maxOrder + 1
            };

            _context.CourseLessons.Add(lesson);
            await _context.SaveChangesAsync();

            return Json(new { success = true, lesson = new { lesson.LessonId, lesson.Title, lesson.TitleAr, lesson.LessonType, lesson.IsFreePreview, lesson.OrderIndex } });
        }

        [HttpPost("EditLesson")]
        public async Task<IActionResult> EditLesson(int lessonId, string title, string titleAr, string lessonType, string contentUrl, string textContent, int duration, bool isFreePreview)
        {
            var lesson = await _context.CourseLessons.FindAsync(lessonId);
            if (lesson == null) return Json(new { success = false });

            lesson.Title = title;
            lesson.TitleAr = titleAr;
            lesson.LessonType = lessonType;
            lesson.ContentUrl = contentUrl;
            lesson.TextContent = textContent;
            lesson.DurationMinutes = duration;
            lesson.IsFreePreview = isFreePreview;

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost("DeleteLesson")]
        public async Task<IActionResult> DeleteLesson(int lessonId)
        {
            var lesson = await _context.CourseLessons.FindAsync(lessonId);
            if (lesson == null) return Json(new { success = false });

            _context.CourseLessons.Remove(lesson);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost("ReorderLessons")]
        public async Task<IActionResult> ReorderLessons([FromBody] ReorderLessonsDto data)
        {
            // data contains ModuleId and List of LessonIds
            var lessons = await _context.CourseLessons.Where(l => l.ModuleId == data.ModuleId).ToListAsync();
            
            for (int i = 0; i < data.LessonIds.Count; i++)
            {
                var lessonId = data.LessonIds[i];
                var lesson = lessons.FirstOrDefault(l => l.LessonId == lessonId);
                if (lesson != null)
                {
                    lesson.OrderIndex = i + 1;
                }
            }
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost("TogglePublishLesson")]
        public async Task<IActionResult> TogglePublishLesson(int lessonId, bool isPublished)
        {
            var lesson = await _context.CourseLessons.FindAsync(lessonId);
            if (lesson == null) return Json(new { success = false });

            lesson.IsPublished = isPublished;
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }

    public class ReorderLessonsDto
    {
        public int ModuleId { get; set; }
        public List<int> LessonIds { get; set; } = new List<int>();
    }
}
