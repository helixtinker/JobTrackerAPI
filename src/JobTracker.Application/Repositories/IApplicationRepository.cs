using JobTracker.Domain;

namespace JobTracker.Application.Repositories;

public interface IApplicationRepository
{
    Task<IEnumerable<JobApplication>> GetAllAsync();
    Task<JobApplication?> GetByIdAsync(int id);
    Task<IEnumerable<JobApplication>> GetByStatusIdAsync(int statusId);
    Task<IEnumerable<JobApplication>> SearchAsync(
        string? companyName,
        string? recruiterName,
        string? techFocus,
        string? notes,
        int? statusId);
    Task AddAsync(JobApplication application);
    void Remove(JobApplication application);
    Task SaveChangesAsync();
}
