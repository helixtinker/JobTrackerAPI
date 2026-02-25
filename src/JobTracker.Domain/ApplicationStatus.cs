namespace JobTracker.Domain;

public class ApplicationStatus
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;

    public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
}