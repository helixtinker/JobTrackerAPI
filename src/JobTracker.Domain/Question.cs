namespace JobTracker.Domain;

public class Question
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? AnswerText { get; set; }
    public int QuestionTypeId { get; set; }
    public QuestionType? QuestionType { get; set; }
    public ICollection<QuestionTechTag> TechTags { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}