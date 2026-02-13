namespace JobTracker.Domain;

public class ApplicationStatus
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;

    public ICollection<Application> Applications { get; set; } = new List<Application>();
}