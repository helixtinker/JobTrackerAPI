using JobTracker.Application.Dtos;

namespace JobTracker.Application.Services;

public interface IApplicationService
{
    Task<IEnumerable<JobApplicationDto>> GetAllAsync();
    Task<JobApplicationDto?> GetByIdAsync(int id);
    Task<IEnumerable<JobApplicationDto>> GetByStatusIdAsync(int statusId);
    Task<IEnumerable<JobApplicationDto>> SearchAsync(
        string? companyName,
        string? recruiterName,
        string? techFocus,
        string? notes,
        int? statusId);
    Task<JobApplicationDto> CreateAsync(CreateJobApplicationDto dto);
    Task<bool> UpdateAsync(int id, UpdateJobApplicationDto dto);
    Task<bool> DeleteAsync(int id);
}
