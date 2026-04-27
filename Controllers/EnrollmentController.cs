using MentorDashboardApp.Data;
using MentorDashboardApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MentorDashboardApp.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Checkout(int courseId)
        {
            var course = await _context.Courses.Include(c => c.Trainer).FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null) return NotFound();

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdStr, out int userId))
                {
                    var existing = await _context.Enrollments.FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);
                    if (existing != null)
                    {
                        return RedirectToAction("Player", "LMS", new { courseId = courseId });
                    }
                }
            }

            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEnrollment(int courseId, string fullName, string email, string password, string paymentMethod)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound();

            int userId = 0;

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(userIdStr, out userId);
            }
            else
            {
                // Create user if they don't exist
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                {
                    // For demo, just log them in if email matches. In a real app, verify password.
                    userId = existingUser.UserId;
                }
                else
                {
                    var newUser = new User
                    {
                        FullName = string.IsNullOrEmpty(fullName) ? "Student" : fullName,
                        Email = string.IsNullOrEmpty(email) ? $"student_{Guid.NewGuid().ToString().Substring(0,5)}@mentor.com" : email,
                        PasswordHash = string.IsNullOrEmpty(password) ? "default" : password,
                        Role = "Student",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();
                    userId = newUser.UserId;
                    existingUser = newUser; // so we can use fullName in claims
                }

                // Sign in the user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, existingUser?.FullName ?? fullName ?? "Student"),
                    new Claim(ClaimTypes.Role, "Student")
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            }

            // Check if already enrolled
            var existingEnrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);
            if (existingEnrollment != null)
            {
                return RedirectToAction("Player", "LMS", new { courseId = courseId });
            }

            // Create Enrollment
            var enrollment = new Enrollment
            {
                CourseId = courseId,
                UserId = userId,
                EnrollmentDate = DateTime.UtcNow,
                Status = "Active"
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            // Create Payment if not free
            decimal priceToPay = (course.DiscountPrice.HasValue && course.DiscountPrice > 0) ? course.DiscountPrice.Value : course.Price;
            if (priceToPay > 0)
            {
                var payment = new Payment
                {
                    EnrollmentId = enrollment.EnrollmentId,
                    Amount = priceToPay,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = string.IsNullOrEmpty(paymentMethod) ? "CreditCard" : paymentMethod
                };
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Success", new { courseId = courseId });
        }

        public async Task<IActionResult> Success(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound();
            return View(course);
        }
    }
}
