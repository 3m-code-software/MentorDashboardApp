using System.Collections.Generic;

namespace MentorDashboardApp.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalEnrollments { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalStudents { get; set; }
        public int EnrollmentIncreasePercent { get; set; }
        public int RevenueIncreasePercent { get; set; }
        public int StudentsIncreasePercent { get; set; }
        
        // For the charts
        public List<int> MonthlyEnrollments { get; set; } = new List<int>();
        public List<decimal> MonthlyRevenue { get; set; } = new List<decimal>();
        public List<string> Months { get; set; } = new List<string>();
    }
}
