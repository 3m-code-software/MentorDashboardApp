using System;

namespace MentorDashboardApp.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Active"; // 'Active', 'Completed', 'Cancelled'

        // Navigation properties
        public Course? Course { get; set; }
        public User? User { get; set; }
    }
}
