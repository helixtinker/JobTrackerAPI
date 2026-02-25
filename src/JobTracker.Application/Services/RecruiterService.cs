using JobTracker.Application.Dtos;
using JobTracker.Application.Repositories;
using JobTracker.Domain;

namespace JobTracker.Application.Services;

public class RecruiterService : IRecruiterService
{
    private readonly IRecruiterRepository _repository;

    public RecruiterService(IRecruiterRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RecruiterDto>> GetAllAsync()
    {
        var recruiters = await _repository.GetAllAsync();
        return recruiters.Select(ToDto);
    }

    public async Task<RecruiterDto?> GetByIdAsync(int id)
    {
        var recruiter = await _repository.GetByIdAsync(id);
        return recruiter is null ? null : ToDto(recruiter);
    }

    public async Task<IEnumerable<RecruiterDto>> GetByCompanyAsync(string company)
    {
        var recruiters = await _repository.GetByCompanyAsync(company);
        return recruiters.Select(ToDto);
    }

    public async Task<RecruiterDto> CreateAsync(CreateRecruiterDto dto)
    {
        var recruiter = new Recruiter
        {
            RecruiterName = dto.RecruiterName,
            Company = dto.Company,
            Email = dto.Email,
            Phone = dto.Phone,
            LinkedInUrl = dto.LinkedInUrl,
            DateContacted = dto.DateContacted,
            RecruiterStatusId = dto.RecruiterStatusId ?? 1,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(recruiter);
        await _repository.SaveChangesAsync();

        // Reload with navigation properties for the response DTO
        var created = await _repository.GetByIdAsync(recruiter.RecruiterId);
        return ToDto(created!);
    }

    public async Task<RecruiterDto?> UpdateAsync(int id, UpdateRecruiterDto dto)
    {
        var recruiter = await _repository.GetByIdAsync(id);
        if (recruiter is null)
            return null;

        if (!string.IsNullOrEmpty(dto.RecruiterName))
            recruiter.RecruiterName = dto.RecruiterName;
        if (dto.Company != null)
            recruiter.Company = dto.Company;
        if (dto.Email != null)
            recruiter.Email = dto.Email;
        if (dto.Phone != null)
            recruiter.Phone = dto.Phone;
        if (dto.LinkedInUrl != null)
            recruiter.LinkedInUrl = dto.LinkedInUrl;
        if (dto.DateContacted.HasValue)
            recruiter.DateContacted = dto.DateContacted.Value;
        if (dto.RecruiterStatusId.HasValue)
            recruiter.RecruiterStatusId = dto.RecruiterStatusId;
        if (dto.Notes != null)
            recruiter.Notes = dto.Notes;

        recruiter.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return ToDto(recruiter);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var recruiter = await _repository.GetByIdAsync(id);
        if (recruiter is null)
            return false;

        _repository.Remove(recruiter);
        await _repository.SaveChangesAsync();
        return true;
    }

    private static RecruiterDto ToDto(Recruiter r) => new()
    {
        RecruiterId = r.RecruiterId,
        RecruiterName = r.RecruiterName,
        Company = r.Company,
        Email = r.Email,
        Phone = r.Phone,
        LinkedInUrl = r.LinkedInUrl,
        DateContacted = r.DateContacted,
        RecruiterStatusId = r.RecruiterStatusId,
        StatusName = r.Status?.StatusName,
        Notes = r.Notes,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };
}
