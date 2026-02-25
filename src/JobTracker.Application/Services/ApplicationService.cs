using JobTracker.Application.Dtos;
using JobTracker.Application.Repositories;
using JobTracker.Domain;

namespace JobTracker.Application.Services;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _repository;

    public ApplicationService(IApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<JobApplicationDto>> GetAllAsync()
    {
        var applications = await _repository.GetAllAsync();
        return applications.Select(ToDto);
    }

    public async Task<JobApplicationDto?> GetByIdAsync(int id)
    {
        var application = await _repository.GetByIdAsync(id);
        return application is null ? null : ToDto(application);
    }

    public async Task<IEnumerable<JobApplicationDto>> GetByStatusIdAsync(int statusId)
    {
        var applications = await _repository.GetByStatusIdAsync(statusId);
        return applications.Select(ToDto);
    }

    public async Task<IEnumerable<JobApplicationDto>> SearchAsync(
        string? companyName,
        string? recruiterName,
        string? techFocus,
        string? notes,
        int? statusId)
    {
        var applications = await _repository.SearchAsync(companyName, recruiterName, techFocus, notes, statusId);
        return applications.Select(ToDto);
    }

    public async Task<JobApplicationDto> CreateAsync(CreateJobApplicationDto dto)
    {
        var application = new JobApplication
        {
            AppliedDate = dto.AppliedDate,
            JobTitle = dto.JobTitle,
            CompanyName = dto.CompanyName,
            Location = dto.Location,
            JobPostUrl = dto.JobPostUrl,
            StatusId = dto.StatusId,
            CompanyWebsite = dto.CompanyWebsite,
            NetworkContacts = dto.NetworkContacts,
            CompanyResearchKeyPoints = dto.CompanyResearchKeyPoints,
            Notes = dto.Notes,
            TechFocus = dto.TechFocus,
            JobPublishedDate = dto.JobPublishedDate,
            RecruiterId = dto.RecruiterId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(application);
        await _repository.SaveChangesAsync();

        // Reload with navigation properties for the response DTO
        var created = await _repository.GetByIdAsync(application.ApplicationId);
        return ToDto(created!);
    }

    public async Task<bool> UpdateAsync(int id, UpdateJobApplicationDto dto)
    {
        var application = await _repository.GetByIdAsync(id);
        if (application is null)
            return false;

        application.AppliedDate = dto.AppliedDate ?? application.AppliedDate;
        application.JobTitle = dto.JobTitle ?? application.JobTitle;
        application.CompanyName = dto.CompanyName ?? application.CompanyName;
        application.Location = dto.Location ?? application.Location;
        application.JobPostUrl = dto.JobPostUrl ?? application.JobPostUrl;
        application.StatusId = dto.StatusId ?? application.StatusId;
        application.CompanyWebsite = dto.CompanyWebsite ?? application.CompanyWebsite;
        application.NetworkContacts = dto.NetworkContacts ?? application.NetworkContacts;
        application.CompanyResearchKeyPoints = dto.CompanyResearchKeyPoints ?? application.CompanyResearchKeyPoints;
        application.Notes = dto.Notes ?? application.Notes;
        application.TechFocus = dto.TechFocus ?? application.TechFocus;
        application.JobPublishedDate = dto.JobPublishedDate ?? application.JobPublishedDate;
        application.RecruiterId = dto.RecruiterId ?? application.RecruiterId;
        application.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var application = await _repository.GetByIdAsync(id);
        if (application is null)
            return false;

        _repository.Remove(application);
        await _repository.SaveChangesAsync();
        return true;
    }

    private static JobApplicationDto ToDto(JobApplication a) => new()
    {
        ApplicationId = a.ApplicationId,
        AppliedDate = a.AppliedDate,
        JobTitle = a.JobTitle,
        CompanyName = a.CompanyName,
        Location = a.Location,
        JobPostUrl = a.JobPostUrl,
        StatusId = a.StatusId,
        StatusName = a.Status?.StatusName,
        CompanyWebsite = a.CompanyWebsite,
        NetworkContacts = a.NetworkContacts,
        CompanyResearchKeyPoints = a.CompanyResearchKeyPoints,
        Notes = a.Notes,
        TechFocus = a.TechFocus,
        JobPublishedDate = a.JobPublishedDate,
        RecruiterId = a.RecruiterId,
        RecruiterName = a.Recruiter?.RecruiterName,
        CreatedAt = a.CreatedAt,
        UpdatedAt = a.UpdatedAt
    };
}
