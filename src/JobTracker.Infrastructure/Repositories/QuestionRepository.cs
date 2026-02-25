using JobTracker.Application.Repositories;
using JobTracker.Domain;
using JobTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly JobTrackerDbContext _dbContext;

    public QuestionRepository(JobTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Question>> GetAllAsync()
    {
        return await _dbContext.Questions
            .Include(q => q.QuestionType)
            .ToListAsync();
    }

    public async Task<Question?> GetByIdAsync(int id)
    {
        return await _dbContext.Questions
            .Include(q => q.QuestionType)
            .FirstOrDefaultAsync(q => q.QuestionId == id);
    }

    public async Task<IEnumerable<Question>> GetByTypeAsync(int typeId)
    {
        return await _dbContext.Questions
            .Include(q => q.QuestionType)
            .Where(q => q.QuestionTypeId == typeId)
            .ToListAsync();
    }

    public async Task AddAsync(Question question)
    {
        await _dbContext.Questions.AddAsync(question);
    }

    public void Remove(Question question)
    {
        _dbContext.Questions.Remove(question);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
