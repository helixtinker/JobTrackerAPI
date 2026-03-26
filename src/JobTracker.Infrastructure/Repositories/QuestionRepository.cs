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
            .Include(q => q.TechTags)
            .ToListAsync();
    }

    public async Task<Question?> GetByIdAsync(int id)
    {
        return await _dbContext.Questions
            .Include(q => q.QuestionType)
            .Include(q => q.TechTags)
            .FirstOrDefaultAsync(q => q.QuestionId == id);
    }

    public async Task<IEnumerable<Question>> GetByTypeAsync(int typeId)
    {
        return await _dbContext.Questions
            .Include(q => q.QuestionType)
            .Include(q => q.TechTags)
            .Where(q => q.QuestionTypeId == typeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetByTechTagsAsync(IEnumerable<string> tags)
    {
        var normalised = tags
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToLower())
            .Distinct()
            .ToList();

        if (normalised.Count == 0)
            return [];

        return await _dbContext.Questions
            .Include(q => q.QuestionType)
            .Include(q => q.TechTags)
            .Where(q => q.TechTags.Any(t => normalised.Contains(t.Tag)))
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
