using JobTracker.Application.Repositories;
using JobTracker.Domain;
using JobTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly JobTrackerDbContext _dbContext;

    public ApplicationRepository(JobTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<JobApplication>> GetAllAsync()
    {
        return await _dbContext.Applications
            .Include(a => a.Status)
            .Include(a => a.Recruiter)
            .ToListAsync();
    }

    public async Task<JobApplication?> GetByIdAsync(int id)
    {
        return await _dbContext.Applications
            .Include(a => a.Status)
            .Include(a => a.Recruiter)
            .FirstOrDefaultAsync(a => a.ApplicationId == id);
    }

    public async Task<IEnumerable<JobApplication>> GetByStatusIdAsync(int statusId)
    {
        return await _dbContext.Applications
            .Include(a => a.Status)
            .Include(a => a.Recruiter)
            .Where(a => a.StatusId == statusId)
            .OrderByDescending(a => a.AppliedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<JobApplication>> SearchAsync(
        string? companyName,
        string? recruiterName,
        string? techFocus,
        string? notes,
        int? statusId)
    {
        var query = _dbContext.Applications
            .Include(a => a.Status)
            .Include(a => a.Recruiter)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(companyName))
            query = query.Where(a => a.CompanyName != null && a.CompanyName.Contains(companyName));

        if (!string.IsNullOrWhiteSpace(recruiterName))
            query = query.Where(a => a.Recruiter != null && a.Recruiter.RecruiterName != null && a.Recruiter.RecruiterName.Contains(recruiterName));

        if (!string.IsNullOrWhiteSpace(techFocus))
            query = query.Where(a => a.TechFocus != null && a.TechFocus.Contains(techFocus));

        if (!string.IsNullOrWhiteSpace(notes))
            query = query.Where(a => a.Notes != null && a.Notes.Contains(notes));

        if (statusId.HasValue)
            query = query.Where(a => a.StatusId == statusId.Value);

        return await query
            .OrderByDescending(a => a.AppliedDate)
            .ToListAsync();
    }

    public async Task AddAsync(JobApplication application)
    {
        await _dbContext.Applications.AddAsync(application);
    }

    public void Remove(JobApplication application)
    {
        _dbContext.Applications.Remove(application);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
