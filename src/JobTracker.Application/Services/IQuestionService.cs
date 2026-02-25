using JobTracker.Application.Dtos;

namespace JobTracker.Application.Services;

public interface IQuestionService
{
    Task<IEnumerable<QuestionDto>> GetAllAsync();
    Task<QuestionDto?> GetByIdAsync(int id);
    Task<IEnumerable<QuestionDto>> GetByTypeAsync(int typeId);
    Task<QuestionDto> CreateAsync(CreateQuestionDto dto);
    Task<bool> UpdateAsync(int id, UpdateQuestionDto dto);
    Task<bool> DeleteAsync(int id);
}
