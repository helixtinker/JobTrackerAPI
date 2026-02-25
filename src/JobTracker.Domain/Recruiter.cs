namespace JobTracker.Domain;

public class Recruiter
{
    public int RecruiterId { get; set; }
    public string RecruiterName { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LinkedInUrl { get; set; }
    public DateTime DateContacted { get; set; }
    public int? RecruiterStatusId { get; set; }
    public RecruiterStatus? Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}
