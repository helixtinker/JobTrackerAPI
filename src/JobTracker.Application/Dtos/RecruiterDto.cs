namespace JobTracker.Application.Dtos;

public class RecruiterDto
{
    public int RecruiterId { get; set; }
    public string RecruiterName { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LinkedInUrl { get; set; }
    public DateTime DateContacted { get; set; }
    public int? RecruiterStatusId { get; set; }
    public string? StatusName { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateRecruiterDto
{
    public string RecruiterName { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LinkedInUrl { get; set; }
    public DateTime DateContacted { get; set; }
    public int? RecruiterStatusId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateRecruiterDto
{
    public string? RecruiterName { get; set; }
    public string? Company { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? LinkedInUrl { get; set; }
    public DateTime? DateContacted { get; set; }
    public int? RecruiterStatusId { get; set; }
    public string? Notes { get; set; }
}
