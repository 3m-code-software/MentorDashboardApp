using Microsoft.AspNetCore.Mvc;
using MentorDashboardApp.Data;
using MentorDashboardApp.ViewModels;
using MentorDashboardApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MentorDashboardApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalEnrollments = await _context.Enrollments.CountAsync();
            var totalRevenue = await _context.Payments.SumAsync(p => (decimal?)p.Amount) ?? 0;
            var totalStudents = await _context.Users.CountAsync(u => u.Role == "Student"); // updated to "Student" instead of "User" to match our system

            var model = new DashboardViewModel
            {
                TotalEnrollments = totalEnrollments,
                TotalRevenue = totalRevenue,
                TotalStudents = totalStudents,
                EnrollmentIncreasePercent = 12, // Dummy static data for demonstration
                RevenueIncreasePercent = 8,
                StudentsIncreasePercent = 5,
                
                // Static chart data for now, ideally this would be grouped by date from database
                MonthlyEnrollments = new List<int> { 31, 40, 28, 51, 42, 82, 56 },
                MonthlyRevenue = new List<decimal> { 110, 320, 450, 320, 340, 520, 410 },
                Months = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul" }
            };

            return View(model);
        }

        // --- Trainers Management ---
        
        public async Task<IActionResult> ManageTrainers()
        {
            var trainers = await _context.Users.Where(u => u.Role == "Trainer").ToListAsync();
            return View(trainers);
        }

        [HttpGet]
        public async Task<IActionResult> EditTrainer(int? id)
        {
            if (id == null)
            {
                return View(new MentorDashboardApp.Models.User { Role = "Trainer" });
            }

            var trainer = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id && u.Role == "Trainer");
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        [HttpPost]
        public async Task<IActionResult> EditTrainer(MentorDashboardApp.Models.User model)
        {
            if (model.UserId == 0)
            {
                model.Role = "Trainer";
                model.CreatedAt = DateTime.UtcNow;
                _context.Users.Add(model);
            }
            else
            {
                var existing = await _context.Users.FindAsync(model.UserId);
                if (existing != null)
                {
                    existing.FullName = model.FullName;
                    existing.Email = model.Email;
                    if (!string.IsNullOrEmpty(model.PasswordHash)) existing.PasswordHash = model.PasswordHash;
                    existing.IsActive = model.IsActive;
                    
                    existing.ProfileImageUrl = model.ProfileImageUrl;
                    existing.Specialty = model.Specialty;
                    existing.SpecialtyAr = model.SpecialtyAr;
                    existing.Bio = model.Bio;
                    existing.BioAr = model.BioAr;
                    existing.FacebookUrl = model.FacebookUrl;
                    existing.TwitterUrl = model.TwitterUrl;
                    existing.LinkedInUrl = model.LinkedInUrl;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageTrainers));
        }
        // --- Users Management ---
        
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _context.Users.Where(u => u.Role == "Student").ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null && user.Role != "Admin")
            {
                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageUsers));
        }

        // --- Settings Management ---
        public IActionResult Settings()
        {
            // In a real application, you would load these from a database or appsettings.json
            ViewBag.SiteName = "Mentor Platform";
            ViewBag.ContactEmail = "info@mentor.com";
            ViewBag.ContactPhone = "+1 5589 55488 55";
            ViewBag.Currency = "USD";
            ViewBag.TrainerCommission = 70; // 70%
            
            return View();
        }

        [HttpPost]
        public IActionResult SaveSettings(string siteName, string contactEmail, string contactPhone, string currency, int trainerCommission)
        {
            // Mock saving logic
            TempData["SuccessMessage"] = "System settings have been successfully updated.";
            return RedirectToAction(nameof(Settings));
        }
    }
}
