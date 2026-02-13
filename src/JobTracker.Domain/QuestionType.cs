namespace JobTracker.Domain;

public class QuestionType
{
    public int QuestionTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}