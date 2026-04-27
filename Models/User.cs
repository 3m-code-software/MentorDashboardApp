using System;

namespace MentorDashboardApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // 'Admin', 'User', 'Trainer'
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Trainer Specific Fields (Null for regular users)
        public string? ProfileImageUrl { get; set; }
        public string? Specialty { get; set; }
        public string? SpecialtyAr { get; set; }
        public string? Bio { get; set; }
        public string? BioAr { get; set; }
        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? LinkedInUrl { get; set; }
    }
}
