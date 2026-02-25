using JobTracker.Application.Dtos;

namespace JobTracker.Application.Services;

public interface IRecruiterService
{
    Task<IEnumerable<RecruiterDto>> GetAllAsync();
    Task<RecruiterDto?> GetByIdAsync(int id);
    Task<IEnumerable<RecruiterDto>> GetByCompanyAsync(string company);
    Task<RecruiterDto> CreateAsync(CreateRecruiterDto dto);
    Task<RecruiterDto?> UpdateAsync(int id, UpdateRecruiterDto dto);
    Task<bool> DeleteAsync(int id);
}
