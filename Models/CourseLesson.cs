using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentorDashboardApp.Models
{
    public class CourseLesson
    {
        [Key]
        public int LessonId { get; set; }

        public int ModuleId { get; set; }
        [ForeignKey("ModuleId")]
        public CourseModule? Module { get; set; }

        public string? Title { get; set; }
        public string? TitleAr { get; set; }

        // "Video", "Text", "File"
        public string? LessonType { get; set; }

        public string? ContentUrl { get; set; }
        
        public string? TextContent { get; set; }

        public int OrderIndex { get; set; }

        public int? DurationMinutes { get; set; }

        public bool IsFreePreview { get; set; } = false;
        public bool IsPublished { get; set; } = true;
    }
}
