using JobTracker.Domain;

namespace JobTracker.Application.Repositories;

public interface IQuestionRepository
{
    Task<IEnumerable<Question>> GetAllAsync();
    Task<Question?> GetByIdAsync(int id);
    Task<IEnumerable<Question>> GetByTypeAsync(int typeId);
    Task<IEnumerable<Question>> GetByTechTagsAsync(IEnumerable<string> tags);
    Task AddAsync(Question question);
    void Remove(Question question);
    Task SaveChangesAsync();
}
