using System;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentorDashboardApp.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string? Title { get; set; }
        public string? TitleAr { get; set; }

        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int? TrainerId { get; set; }
        public string? ImageUrl { get; set; }
        
        // New Video Fields
        public string? VideoUrl { get; set; }
        public string? VideoPlatform { get; set; } // e.g. "YouTube", "GoogleDrive"
        
        // New Additional Fields
        public string? Status { get; set; } = "Draft"; // Draft, Published
        public string? Category { get; set; }
        public string? Level { get; set; }
        public string? Duration { get; set; }

        // Advanced Content & Text
        public string? Prerequisites { get; set; }
        public string? Tags { get; set; }

        // Pricing & Options
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPrice { get; set; }

        // Marketing & Certification
        public string? SeoTitle { get; set; }
        public string? SeoDescription { get; set; }
        public bool HasCertificate { get; set; } = false;
        public bool IsFeatured { get; set; } = false;

        // Quick Stats
        public int? TotalLessons { get; set; }
        public string? Language { get; set; } = "Arabic";

        // Scheduling
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxCapacity { get; set; }

        // Objectives & Audience
        public string? WhatYouWillLearn { get; set; }
        public string? WhatYouWillLearnAr { get; set; }
        public string? TargetAudience { get; set; }
        public string? TargetAudienceAr { get; set; }

        public string? PrerequisitesAr { get; set; }

        // Resources
        public string? SyllabusUrl { get; set; }
        public string? CommunityLink { get; set; }

        // Advanced Payments
        public string? PaymentType { get; set; } = "Paid"; // Free, Paid, Subscription
        public bool AcceptsCoupons { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User? Trainer { get; set; }

        public ICollection<CourseModule> Modules { get; set; } = new List<CourseModule>();
    }
}
