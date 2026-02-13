namespace JobTracker.Application.Dtos;

public class QuestionDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? AnswerText { get; set; }
    public int QuestionTypeId { get; set; }
    public string? QuestionTypeName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateQuestionDto
{
    public string QuestionText { get; set; } = string.Empty;
    public string? AnswerText { get; set; }
    public int QuestionTypeId { get; set; }
}

public class UpdateQuestionDto
{
    public string QuestionText { get; set; } = string.Empty;
    public string? AnswerText { get; set; }
    public int QuestionTypeId { get; set; }
}
