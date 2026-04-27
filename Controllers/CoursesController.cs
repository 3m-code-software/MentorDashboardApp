using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MentorDashboardApp.Data;
using MentorDashboardApp.Models;

namespace MentorDashboardApp.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CoursesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses.ToListAsync();
            return View(courses);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,Title,TitleAr,Description,DescriptionAr,Price,ImageUrl,VideoUrl,VideoPlatform,Status,Category,Level,Duration,Prerequisites,PrerequisitesAr,Tags,DiscountPrice,SeoTitle,SeoDescription,HasCertificate,IsFeatured,TotalLessons,Language,StartDate,EndDate,MaxCapacity,WhatYouWillLearn,WhatYouWillLearnAr,TargetAudience,TargetAudienceAr,SyllabusUrl,CommunityLink,PaymentType,AcceptsCoupons")] Course course, IFormFile? imageFile, IFormFile? syllabusFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "courses");
                    Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    course.ImageUrl = "/uploads/courses/" + uniqueFileName;
                }

                if (syllabusFile != null && syllabusFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "syllabus");
                    Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + syllabusFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await syllabusFile.CopyToAsync(fileStream);
                    }
                    course.SyllabusUrl = "/uploads/syllabus/" + uniqueFileName;
                }

                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,Title,TitleAr,Description,DescriptionAr,Price,ImageUrl,VideoUrl,VideoPlatform,Status,Category,Level,Duration,Prerequisites,PrerequisitesAr,Tags,DiscountPrice,SeoTitle,SeoDescription,HasCertificate,IsFeatured,TotalLessons,Language,StartDate,EndDate,MaxCapacity,WhatYouWillLearn,WhatYouWillLearnAr,TargetAudience,TargetAudienceAr,SyllabusUrl,CommunityLink,PaymentType,AcceptsCoupons")] Course course, IFormFile? imageFile, IFormFile? syllabusFile)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "courses");
                        Directory.CreateDirectory(uploadsFolder);
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        course.ImageUrl = "/uploads/courses/" + uniqueFileName;
                    }

                    if (syllabusFile != null && syllabusFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "syllabus");
                        Directory.CreateDirectory(uploadsFolder);
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + syllabusFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await syllabusFile.CopyToAsync(fileStream);
                        }
                        course.SyllabusUrl = "/uploads/syllabus/" + uniqueFileName;
                    }

                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }

        // GET: Courses/Duplicate/5
        public async Task<IActionResult> Duplicate(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            // Clone Course
            var newCourse = new Course
            {
                Title = course.Title + " (Copy)",
                TitleAr = string.IsNullOrEmpty(course.TitleAr) ? null : course.TitleAr + " (نسخة)",
                Description = course.Description,
                DescriptionAr = course.DescriptionAr,
                Price = course.Price,
                ImageUrl = course.ImageUrl,
                VideoUrl = course.VideoUrl,
                VideoPlatform = course.VideoPlatform,
                Status = "Draft", // Always set duplicated course to Draft
                Category = course.Category,
                Level = course.Level,
                Duration = course.Duration,
                Prerequisites = course.Prerequisites,
                PrerequisitesAr = course.PrerequisitesAr,
                Tags = course.Tags,
                DiscountPrice = course.DiscountPrice,
                HasCertificate = course.HasCertificate,
                IsFeatured = false,
                TotalLessons = course.TotalLessons,
                Language = course.Language,
                MaxCapacity = course.MaxCapacity,
                WhatYouWillLearn = course.WhatYouWillLearn,
                WhatYouWillLearnAr = course.WhatYouWillLearnAr,
                TargetAudience = course.TargetAudience,
                TargetAudienceAr = course.TargetAudienceAr,
                SyllabusUrl = course.SyllabusUrl,
                CreatedAt = DateTime.UtcNow
            };

            // Clone Modules and Lessons
            if (course.Modules != null && course.Modules.Any())
            {
                newCourse.Modules = new List<CourseModule>();
                foreach (var module in course.Modules)
                {
                    var newModule = new CourseModule
                    {
                        Title = module.Title,
                        TitleAr = module.TitleAr,
                        OrderIndex = module.OrderIndex,
                        Lessons = new List<CourseLesson>()
                    };

                    if (module.Lessons != null && module.Lessons.Any())
                    {
                        foreach (var lesson in module.Lessons)
                        {
                            var newLesson = new CourseLesson
                            {
                                Title = lesson.Title,
                                TitleAr = lesson.TitleAr,
                                ContentUrl = lesson.ContentUrl,
                                TextContent = lesson.TextContent,
                                DurationMinutes = lesson.DurationMinutes,
                                IsFreePreview = lesson.IsFreePreview,
                                LessonType = lesson.LessonType,
                                OrderIndex = lesson.OrderIndex,
                                IsPublished = false // Default to false for copied lessons to ensure review
                            };
                            newModule.Lessons.Add(newLesson);
                        }
                    }
                    newCourse.Modules.Add(newModule);
                }
            }

            _context.Courses.Add(newCourse);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
