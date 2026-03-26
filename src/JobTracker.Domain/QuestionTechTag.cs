namespace JobTracker.Domain;

public class QuestionTechTag
{
    public int QuestionTechTagId { get; set; }
    public int QuestionId { get; set; }
    public string Tag { get; set; } = string.Empty;
    public Question? Question { get; set; }
}
