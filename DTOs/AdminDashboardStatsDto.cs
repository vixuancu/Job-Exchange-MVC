namespace JobExchangeMvc.DTOs;

/// <summary>
/// DTO cho Admin Dashboard Statistics
/// </summary>
public class AdminDashboardStatsDto
{
    // User Statistics
    public int TotalUsers { get; set; }
    public int TotalAdmins { get; set; }
    public int TotalEmployers { get; set; }
    public int TotalApplicants { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int NewUsersThisMonth { get; set; }

    // Job Statistics
    public int TotalJobs { get; set; }
    public int PendingJobs { get; set; }
    public int ApprovedJobs { get; set; }
    public int RejectedJobs { get; set; }
    public int ClosedJobs { get; set; }
    public int ExpiredJobs { get; set; }
    public int NewJobsThisMonth { get; set; }
    public int TotalViews { get; set; }

    // Application Statistics
    public int TotalApplications { get; set; }
    public int PendingApplications { get; set; }
    public int ApprovedApplications { get; set; }
    public int InterviewingApplications { get; set; }
    public int AcceptedApplications { get; set; }
    public int RejectedApplications { get; set; }
    public int CancelledApplications { get; set; }
    public int NewApplicationsThisMonth { get; set; }

    // Response Rate (% applications reviewed vs pending)
    public double ResponseRate => TotalApplications > 0
        ? Math.Round((double)(TotalApplications - PendingApplications) / TotalApplications * 100, 2)
        : 0;

    // Acceptance Rate (% accepted vs total reviewed)
    public double AcceptanceRate => (TotalApplications - PendingApplications) > 0
        ? Math.Round((double)AcceptedApplications / (TotalApplications - PendingApplications) * 100, 2)
        : 0;

    // Top Categories (CategoryName -> Count)
    public Dictionary<string, int> TopCategories { get; set; } = new();

    // Top Companies (CompanyName -> JobCount)
    public Dictionary<string, int> TopCompanies { get; set; } = new();
}
