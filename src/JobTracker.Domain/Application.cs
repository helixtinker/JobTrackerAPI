namespace JobTracker.Domain;

public class Application
{
    public int ApplicationId { get; set; }
    public DateTime? AppliedDate { get; set; }
    public string? JobTitle { get; set; }
    public string? CompanyName { get; set; }
    public string? Location { get; set; }
    public string? JobPostUrl { get; set; }
    public int? StatusId { get; set; }
    public ApplicationStatus? Status { get; set; }
    public string? CompanyWebsite { get; set; }
    public string? NetworkContacts { get; set; }
    public string? CompanyResearchKeyPoints { get; set; }
    public string? Notes { get; set; }
    public string? TechFocus { get; set; }
    public DateTime? JobPublishedDate { get; set; }
    public int? RecruiterId { get; set; }
    public Recruiter? Recruiter { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}