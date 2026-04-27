using Microsoft.EntityFrameworkCore;
using MentorDashboardApp.Models;

namespace MentorDashboardApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CourseModule> CourseModules { get; set; }
        public DbSet<CourseLesson> CourseLessons { get; set; }
        public DbSet<StudentLessonProgress> StudentLessonProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Seed an Admin user to ensure we always have one
            modelBuilder.Entity<User>().HasData(new User
            {
                UserId = 1,
                FullName = "Admin User",
                Email = "admin@mentor.com",
                PasswordHash = "DEFAULT_HASH", // In a real app, hash the password!
                Role = "Admin",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });
        }
    }
}
