using JobTracker.Application.Dtos;
using DomainModel = JobTracker.Domain;
using JobTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly JobTrackerDbContext _dbContext;

    public ApplicationsController(JobTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetAll()
    {
        var applications = await _dbContext.Applications
            .Include(a => a.Status)
            .Include(a => a.Recruiter)
            .ToListAsync();

        var dtos = applications.Select(a => new ApplicationDto
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
        }).ToList();

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationDto>> GetById(int id)
    {
        var application = await _dbContext.Applications
            .Include(a => a.Status)
            .Include(a => a.Recruiter)
            .FirstOrDefaultAsync(a => a.ApplicationId == id);

        if (application is null)
            return NotFound();

        var dto = new ApplicationDto
        {
            ApplicationId = application.ApplicationId,
            AppliedDate = application.AppliedDate,
            JobTitle = application.JobTitle,
            CompanyName = application.CompanyName,
            Location = application.Location,
            JobPostUrl = application.JobPostUrl,
            StatusId = application.StatusId,
            StatusName = application.Status?.StatusName,
            CompanyWebsite = application.CompanyWebsite,
            NetworkContacts = application.NetworkContacts,
            CompanyResearchKeyPoints = application.CompanyResearchKeyPoints,
            Notes = application.Notes,
            TechFocus = application.TechFocus,
            JobPublishedDate = application.JobPublishedDate,
            RecruiterId = application.RecruiterId,
            RecruiterName = application.Recruiter?.RecruiterName,
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        };

        return Ok(dto);
    }

    [HttpGet("by-StatusId/{statusId}")]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetByStatusId(int statusId)
    {
        var applications = await _dbContext.Applications
            .Include(a => a.Status)
            .Include(a => a.Recruiter)
            .Where(a => a.StatusId == statusId)
            .OrderByDescending(x => x.AppliedDate)
            .ToListAsync();

        var dtos = applications.Select(a => new ApplicationDto
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
        }).ToList();

        return Ok(dtos);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> Search(
        //[FromQuery] string? jobTitle = null,
        [FromQuery] string? companyName = null,
        //[FromQuery] string? location = null,
        [FromQuery] string? recruiterName = null,
        [FromQuery] string? techFocus = null,
        [FromQuery] string? notes = null,
        [FromQuery] int? statusId = null)
    {
        // Validate that at least one parameter is provided
        if (//string.IsNullOrWhiteSpace(jobTitle) &&
            string.IsNullOrWhiteSpace(companyName) &&
            //string.IsNullOrWhiteSpace(location) &&
            string.IsNullOrWhiteSpace(recruiterName) &&
            string.IsNullOrWhiteSpace(techFocus) &&
            string.IsNullOrWhiteSpace(notes) &&
            !statusId.HasValue)
        {
            return BadRequest("At least one search parameter must be provided (jobTitle, companyName, location, recruiterName, techFocus, notes, or statusId).");
        }

        var query = _dbContext.Applications
            .Include(a => a.Status)
            .Include(a => a.Recruiter)
            .AsQueryable();

        // Apply filters for each provided parameter
        // if (!string.IsNullOrWhiteSpace(jobTitle))
        //     query = query.Where(a => a.JobTitle != null && a.JobTitle.Contains(jobTitle));

        if (!string.IsNullOrWhiteSpace(companyName))
            query = query.Where(a => a.CompanyName != null && a.CompanyName.Contains(companyName));

        // if (!string.IsNullOrWhiteSpace(location))
        //     query = query.Where(a => a.Location != null && a.Location.Contains(location));

        if (!string.IsNullOrWhiteSpace(recruiterName))
            query = query.Where(a => a.Recruiter != null && a.Recruiter.RecruiterName != null && a.Recruiter.RecruiterName.Contains(recruiterName));

        if (!string.IsNullOrWhiteSpace(techFocus))
            query = query.Where(a => a.TechFocus != null && a.TechFocus.Contains(techFocus));

        if (!string.IsNullOrWhiteSpace(notes))
            query = query.Where(a => a.Notes != null && a.Notes.Contains(notes));

        if (statusId.HasValue)
            query = query.Where(a => a.StatusId == statusId.Value);

        var applications = await query
            .OrderByDescending(x => x.AppliedDate)
            .ToListAsync();

        var dtos = applications.Select(a => new ApplicationDto
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
        }).ToList();

        return Ok(dtos);
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationDto>> Create(CreateApplicationDto createDto)
    {
        var application = new DomainModel.Application
        {
            AppliedDate = createDto.AppliedDate,
            JobTitle = createDto.JobTitle,
            CompanyName = createDto.CompanyName,
            Location = createDto.Location,
            JobPostUrl = createDto.JobPostUrl,
            StatusId = createDto.StatusId,
            CompanyWebsite = createDto.CompanyWebsite,
            NetworkContacts = createDto.NetworkContacts,
            CompanyResearchKeyPoints = createDto.CompanyResearchKeyPoints,
            Notes = createDto.Notes,
            TechFocus = createDto.TechFocus,
            JobPublishedDate = createDto.JobPublishedDate,
            RecruiterId = createDto.RecruiterId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Applications.Add(application);
        await _dbContext.SaveChangesAsync();

        var resultDto = new ApplicationDto
        {
            ApplicationId = application.ApplicationId,
            AppliedDate = application.AppliedDate,
            JobTitle = application.JobTitle,
            CompanyName = application.CompanyName,
            Location = application.Location,
            JobPostUrl = application.JobPostUrl,
            StatusId = application.StatusId,
            CompanyWebsite = application.CompanyWebsite,
            StatusName = application.Status?.StatusName,
            NetworkContacts = application.NetworkContacts,
            CompanyResearchKeyPoints = application.CompanyResearchKeyPoints,
            Notes = application.Notes,
            TechFocus = application.TechFocus,
            JobPublishedDate = application.JobPublishedDate,
            RecruiterId = application.RecruiterId,
            RecruiterName = application.Recruiter?.RecruiterName,
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = application.ApplicationId }, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateApplicationDto updateDto)
    {
        var application = await _dbContext.Applications.FindAsync(id);
        if (application is null)
            return NotFound();

        application.AppliedDate = updateDto.AppliedDate ?? application.AppliedDate;
        application.JobTitle = updateDto.JobTitle ?? application.JobTitle;
        application.CompanyName = updateDto.CompanyName ?? application.CompanyName;
        application.Location = updateDto.Location ?? application.Location;
        application.JobPostUrl = updateDto.JobPostUrl ?? application.JobPostUrl;
        application.StatusId = updateDto.StatusId ?? application.StatusId;
        application.CompanyWebsite = updateDto.CompanyWebsite ?? application.CompanyWebsite;
        application.NetworkContacts = updateDto.NetworkContacts ?? application.NetworkContacts;
        application.CompanyResearchKeyPoints = updateDto.CompanyResearchKeyPoints ?? application.CompanyResearchKeyPoints;
        application.Notes = updateDto.Notes ?? application.Notes;
        application.TechFocus = updateDto.TechFocus ?? application.TechFocus;
        application.JobPublishedDate = updateDto.JobPublishedDate ?? application.JobPublishedDate;
        application.RecruiterId = updateDto.RecruiterId ?? application.RecruiterId;
        application.UpdatedAt = DateTime.UtcNow;

        _dbContext.Applications.Update(application);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var application = await _dbContext.Applications.FindAsync(id);
        if (application is null)
            return NotFound();

        _dbContext.Applications.Remove(application);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
