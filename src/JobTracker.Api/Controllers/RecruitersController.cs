using JobTracker.Application.Dtos;
using JobTracker.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RecruitersController : ControllerBase
{
    private readonly IRecruiterService _recruiterService;

    public RecruitersController(IRecruiterService recruiterService)
    {
        _recruiterService = recruiterService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecruiterDto>>> GetAll()
    {
        var recruiters = await _recruiterService.GetAllAsync();
        return Ok(recruiters);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecruiterDto>> GetById(int id)
    {
        var recruiter = await _recruiterService.GetByIdAsync(id);
        if (recruiter is null)
            return NotFound();

        return Ok(recruiter);
    }

    [HttpGet("by-company/{company}")]
    public async Task<ActionResult<IEnumerable<RecruiterDto>>> GetByCompany(string company)
    {
        var recruiters = await _recruiterService.GetByCompanyAsync(company);
        return Ok(recruiters);
    }

    [HttpPost]
    public async Task<ActionResult<RecruiterDto>> Create(CreateRecruiterDto createDto)
    {
        var recruiter = await _recruiterService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = recruiter.RecruiterId }, recruiter);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RecruiterDto>> Update(int id, UpdateRecruiterDto updateDto)
    {
        var recruiter = await _recruiterService.UpdateAsync(id, updateDto);
        if (recruiter is null)
            return NotFound();

        return Ok(recruiter);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var found = await _recruiterService.DeleteAsync(id);
        if (!found)
            return NotFound();

        return NoContent();
    }
}
