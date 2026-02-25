using JobTracker.Application.Dtos;
using JobTracker.Application.Repositories;
using JobTracker.Domain;

namespace JobTracker.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _repository;

    public QuestionService(IQuestionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<QuestionDto>> GetAllAsync()
    {
        var questions = await _repository.GetAllAsync();
        return questions.Select(ToDto);
    }

    public async Task<QuestionDto?> GetByIdAsync(int id)
    {
        var question = await _repository.GetByIdAsync(id);
        return question is null ? null : ToDto(question);
    }

    public async Task<IEnumerable<QuestionDto>> GetByTypeAsync(int typeId)
    {
        var questions = await _repository.GetByTypeAsync(typeId);
        return questions.Select(ToDto);
    }

    public async Task<QuestionDto> CreateAsync(CreateQuestionDto dto)
    {
        var question = new Question
        {
            QuestionText = dto.QuestionText,
            AnswerText = dto.AnswerText,
            QuestionTypeId = dto.QuestionTypeId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(question);
        await _repository.SaveChangesAsync();

        // Reload with navigation properties for the response DTO
        var created = await _repository.GetByIdAsync(question.QuestionId);
        return ToDto(created!);
    }

    public async Task<bool> UpdateAsync(int id, UpdateQuestionDto dto)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question is null)
            return false;

        question.QuestionText = dto.QuestionText ?? question.QuestionText;
        question.AnswerText = dto.AnswerText ?? question.AnswerText;
        question.QuestionTypeId = dto.QuestionTypeId;
        question.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question is null)
            return false;

        _repository.Remove(question);
        await _repository.SaveChangesAsync();
        return true;
    }

    private static QuestionDto ToDto(Question q) => new()
    {
        QuestionId = q.QuestionId,
        QuestionText = q.QuestionText,
        AnswerText = q.AnswerText,
        QuestionTypeId = q.QuestionTypeId,
        QuestionTypeName = q.QuestionType?.TypeName,
        CreatedAt = q.CreatedAt,
        UpdatedAt = q.UpdatedAt
    };
}
