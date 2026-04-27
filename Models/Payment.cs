using System;

using System.ComponentModel.DataAnnotations.Schema;

namespace MentorDashboardApp.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int EnrollmentId { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string? PaymentMethod { get; set; } // 'CreditCard', 'PayPal', 'Cash'

        // Navigation property
        public Enrollment? Enrollment { get; set; }
    }
}
