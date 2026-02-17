namespace JobTracker.Domain;

public class RecruiterStatusEntity
{
    public int RecruiterStatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;

    public ICollection<Recruiter> Recruiters { get; set; } = new List<Recruiter>();
}
