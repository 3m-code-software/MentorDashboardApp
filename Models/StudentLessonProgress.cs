using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentorDashboardApp.Models
{
    public class StudentLessonProgress
    {
        [Key]
        public int ProgressId { get; set; }

        public int UserId { get; set; } // The student ID

        public int CourseId { get; set; }

        public int LessonId { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CompletedAt { get; set; }
    }
}
