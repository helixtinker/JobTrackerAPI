using JobTracker.Application.Repositories;
using JobTracker.Domain;
using JobTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Repositories;

public class RecruiterRepository : IRecruiterRepository
{
    private readonly JobTrackerDbContext _dbContext;

    public RecruiterRepository(JobTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Recruiter>> GetAllAsync()
    {
        return await _dbContext.Recruiters
            .Include(r => r.Status)
            .ToListAsync();
    }

    public async Task<Recruiter?> GetByIdAsync(int id)
    {
        return await _dbContext.Recruiters
            .Include(r => r.Status)
            .FirstOrDefaultAsync(r => r.RecruiterId == id);
    }

    public async Task<IEnumerable<Recruiter>> GetByCompanyAsync(string company)
    {
        return await _dbContext.Recruiters
            .Include(r => r.Status)
            .Where(r => r.Company != null && r.Company.Contains(company))
            .ToListAsync();
    }

    public async Task AddAsync(Recruiter recruiter)
    {
        await _dbContext.Recruiters.AddAsync(recruiter);
    }

    public void Remove(Recruiter recruiter)
    {
        _dbContext.Recruiters.Remove(recruiter);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
