using JobTracker.Application.Dtos;
using DomainModel = JobTracker.Domain;
using JobTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RecruitersController : ControllerBase
{
    private readonly JobTrackerDbContext _dbContext;

    public RecruitersController(JobTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecruiterDto>>> GetAll()
    {
        var recruiters = await _dbContext.Recruiters
            .Include(r => r.Status)
            .ToListAsync();

        var dtos = recruiters.Select(r => new RecruiterDto
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
        }).ToList();

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecruiterDto>> GetById(int id)
    {
        var recruiter = await _dbContext.Recruiters
            .Include(r => r.Status)
            .FirstOrDefaultAsync(r => r.RecruiterId == id);

        if (recruiter is null)
            return NotFound();

        var dto = new RecruiterDto
        {
            RecruiterId = recruiter.RecruiterId,
            RecruiterName = recruiter.RecruiterName,
            Company = recruiter.Company,
            Email = recruiter.Email,
            Phone = recruiter.Phone,
            LinkedInUrl = recruiter.LinkedInUrl,
            DateContacted = recruiter.DateContacted,
            RecruiterStatusId = recruiter.RecruiterStatusId,
            StatusName = recruiter.Status?.ToString(),
            Notes = recruiter.Notes,
            CreatedAt = recruiter.CreatedAt,
            UpdatedAt = recruiter.UpdatedAt
        };

        return Ok(dto);
    }

    [HttpGet("by-company/{company}")]
    public async Task<ActionResult<IEnumerable<RecruiterDto>>> GetByCompany(string company)
    {
        var recruiters = await _dbContext.Recruiters
            .Include(r => r.Status)
            .Where(r => r.Company != null && r.Company.Contains(company))
            .ToListAsync();

        var dtos = recruiters.Select(r => new RecruiterDto
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
        }).ToList();

        return Ok(dtos);
    }

    [HttpPost]
    public async Task<ActionResult<RecruiterDto>> Create(CreateRecruiterDto createDto)
    {
        var recruiter = new DomainModel.Recruiter
        {
            RecruiterName = createDto.RecruiterName,
            Company = createDto.Company,
            Email = createDto.Email,
            Phone = createDto.Phone,
            LinkedInUrl = createDto.LinkedInUrl,
            DateContacted = createDto.DateContacted,
            RecruiterStatusId = createDto.RecruiterStatusId ?? 1,
            Notes = createDto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Recruiters.Add(recruiter);
        await _dbContext.SaveChangesAsync();

        var dto = new RecruiterDto
        {
            RecruiterId = recruiter.RecruiterId,
            RecruiterName = recruiter.RecruiterName,
            Company = recruiter.Company,
            Email = recruiter.Email,
            Phone = recruiter.Phone,
            LinkedInUrl = recruiter.LinkedInUrl,
            DateContacted = recruiter.DateContacted,
            RecruiterStatusId = recruiter.RecruiterStatusId,
            StatusName = recruiter.Status?.StatusName,
            Notes = recruiter.Notes,
            CreatedAt = recruiter.CreatedAt,
            UpdatedAt = recruiter.UpdatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = recruiter.RecruiterId }, dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RecruiterDto>> Update(int id, UpdateRecruiterDto updateDto)
    {
        var recruiter = await _dbContext.Recruiters
            .Include(r => r.Status)
            .FirstOrDefaultAsync(r => r.RecruiterId == id);

        if (recruiter is null)
            return NotFound();

        if (!string.IsNullOrEmpty(updateDto.RecruiterName))
            recruiter.RecruiterName = updateDto.RecruiterName;
        if (updateDto.Company != null)
            recruiter.Company = updateDto.Company;
        if (updateDto.Email != null)
            recruiter.Email = updateDto.Email;
        if (updateDto.Phone != null)
            recruiter.Phone = updateDto.Phone;
        if (updateDto.LinkedInUrl != null)
            recruiter.LinkedInUrl = updateDto.LinkedInUrl;
        if (updateDto.DateContacted.HasValue)
            recruiter.DateContacted = updateDto.DateContacted.Value;
        if (updateDto.RecruiterStatusId.HasValue)
            recruiter.RecruiterStatusId = updateDto.RecruiterStatusId;
        if (updateDto.Notes != null)
            recruiter.Notes = updateDto.Notes;

        recruiter.UpdatedAt = DateTime.UtcNow;

        _dbContext.Recruiters.Update(recruiter);
        await _dbContext.SaveChangesAsync();

        var dto = new RecruiterDto
        {
            RecruiterId = recruiter.RecruiterId,
            RecruiterName = recruiter.RecruiterName,
            Company = recruiter.Company,
            Email = recruiter.Email,
            Phone = recruiter.Phone,
            LinkedInUrl = recruiter.LinkedInUrl,
            DateContacted = recruiter.DateContacted,
            RecruiterStatusId = recruiter.RecruiterStatusId,
            StatusName = recruiter.Status?.StatusName,
            Notes = recruiter.Notes,
            CreatedAt = recruiter.CreatedAt,
            UpdatedAt = recruiter.UpdatedAt
        };

        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var recruiter = await _dbContext.Recruiters
            .FirstOrDefaultAsync(r => r.RecruiterId == id);

        if (recruiter is null)
            return NotFound();

        _dbContext.Recruiters.Remove(recruiter);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
