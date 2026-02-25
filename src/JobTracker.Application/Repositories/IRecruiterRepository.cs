using JobTracker.Domain;

namespace JobTracker.Application.Repositories;

public interface IRecruiterRepository
{
    Task<IEnumerable<Recruiter>> GetAllAsync();
    Task<Recruiter?> GetByIdAsync(int id);
    Task<IEnumerable<Recruiter>> GetByCompanyAsync(string company);
    Task AddAsync(Recruiter recruiter);
    void Remove(Recruiter recruiter);
    Task SaveChangesAsync();
}
