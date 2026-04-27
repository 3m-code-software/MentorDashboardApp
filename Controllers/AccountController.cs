using MentorDashboardApp.Data;
using MentorDashboardApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MentorDashboardApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Backdoor/Fail-safe to ensure Admin account always works if seeder failed
            if (email == "admin@3m.com" && password == "admin123")
            {
                var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "admin@3m.com");
                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        FullName = "System Administrator",
                        Email = "admin@3m.com",
                        PasswordHash = "admin123",
                        Role = "Admin",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Users.Add(adminUser);
                    await _context.SaveChangesAsync();
                }
                else if (adminUser.PasswordHash != password)
                {
                    // Update password if it was changed or registered differently by mistake
                    adminUser.PasswordHash = password;
                    adminUser.Role = "Admin"; 
                    await _context.SaveChangesAsync();
                }
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
            if (user != null)
            {
                if (!user.IsActive)
                {
                    ViewBag.ErrorMessage = "Your account has been deactivated by the administrator.";
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName ?? "Student"),
                    new Claim(ClaimTypes.Role, user.Role ?? "Student")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("Dashboard", "LMS");
            }

            ViewBag.ErrorMessage = "Invalid email or password.";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Email is already registered. Please login.";
                return View();
            }

            var newUser = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = password,
                Role = "Student",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.UserId.ToString()),
                new Claim(ClaimTypes.Name, newUser.FullName),
                new Claim(ClaimTypes.Role, "Student")
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Dashboard", "LMS");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            // Mock functionality for showcase
            ViewBag.SuccessMessage = "If this email is registered, a password reset link has been sent.";
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    ViewBag.Enrollments = await _context.Enrollments
                        .Include(e => e.Course)
                        .Where(e => e.UserId == userId)
                        .ToListAsync();
                    
                    if (user.Role == "Trainer")
                    {
                        ViewBag.TrainerCourses = await _context.Courses
                            .Where(c => c.TrainerId == userId)
                            .ToListAsync();
                    }

                    return View(user);
                }
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(User model)
        {
            var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int userId))
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.FullName = model.FullName;
                    if (!string.IsNullOrEmpty(model.PasswordHash))
                    {
                        user.PasswordHash = model.PasswordHash;
                    }

                    if (user.Role == "Trainer")
                    {
                        user.Bio = model.Bio;
                        user.BioAr = model.BioAr;
                        user.Specialty = model.Specialty;
                        user.SpecialtyAr = model.SpecialtyAr;
                        user.FacebookUrl = model.FacebookUrl;
                        user.TwitterUrl = model.TwitterUrl;
                        user.LinkedInUrl = model.LinkedInUrl;
                    }

                    await _context.SaveChangesAsync();

                    // Re-sign in to update claims (like name)
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Role, user.Role ?? "Student")
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    ViewBag.SuccessMessage = "Profile updated successfully.";
                    
                    ViewBag.Enrollments = await _context.Enrollments.Include(e => e.Course).Where(e => e.UserId == userId).ToListAsync();
                    if (user.Role == "Trainer") ViewBag.TrainerCourses = await _context.Courses.Where(c => c.TrainerId == userId).ToListAsync();

                    return View("Profile", user);
                }
            }
            return RedirectToAction("Login");
        }
    }
}
