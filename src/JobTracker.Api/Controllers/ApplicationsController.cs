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
            CreatedAt = application.CreatedAt,
            UpdatedAt = application.UpdatedAt
        };

        return Ok(dto);
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
            NetworkContacts = application.NetworkContacts,
            CompanyResearchKeyPoints = application.CompanyResearchKeyPoints,
            Notes = application.Notes,
            TechFocus = application.TechFocus,
            JobPublishedDate = application.JobPublishedDate,
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
