using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentorDashboardApp.Models
{
    public class CourseModule
    {
        [Key]
        public int ModuleId { get; set; }

        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        public string? Title { get; set; }
        public string? TitleAr { get; set; }

        public int OrderIndex { get; set; }

        public ICollection<CourseLesson> Lessons { get; set; } = new List<CourseLesson>();
    }
}
