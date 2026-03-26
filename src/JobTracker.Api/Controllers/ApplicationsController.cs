using JobTracker.Application.Dtos;
using JobTracker.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly IQuestionService _questionService;

    public ApplicationsController(IApplicationService applicationService, IQuestionService questionService)
    {
        _applicationService = applicationService;
        _questionService = questionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobApplicationDto>>> GetAll()
    {
        var applications = await _applicationService.GetAllAsync();
        return Ok(applications);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobApplicationDto>> GetById(int id)
    {
        var application = await _applicationService.GetByIdAsync(id);
        if (application is null)
            return NotFound();

        return Ok(application);
    }

    [HttpGet("by-StatusId/{statusId}")]
    public async Task<ActionResult<IEnumerable<JobApplicationDto>>> GetByStatusId(int statusId)
    {
        var applications = await _applicationService.GetByStatusIdAsync(statusId);
        return Ok(applications);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<JobApplicationDto>>> Search(
        [FromQuery] string? companyName = null,
        [FromQuery] string? recruiterName = null,
        [FromQuery] string? techFocus = null,
        [FromQuery] string? notes = null,
        [FromQuery] int? statusId = null)
    {
        if (string.IsNullOrWhiteSpace(companyName) &&
            string.IsNullOrWhiteSpace(recruiterName) &&
            string.IsNullOrWhiteSpace(techFocus) &&
            string.IsNullOrWhiteSpace(notes) &&
            !statusId.HasValue)
        {
            return BadRequest("At least one search parameter must be provided.");
        }

        var applications = await _applicationService.SearchAsync(companyName, recruiterName, techFocus, notes, statusId);
        return Ok(applications);
    }

    [HttpPost]
    public async Task<ActionResult<JobApplicationDto>> Create(CreateJobApplicationDto createDto)
    {
        var application = await _applicationService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = application.ApplicationId }, application);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateJobApplicationDto updateDto)
    {
        var found = await _applicationService.UpdateAsync(id, updateDto);
        if (!found)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id}/questions")]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestions(int id)
    {
        var questions = await _questionService.GetByApplicationTechFocusAsync(id);
        if (questions is null)
            return NotFound();

        return Ok(questions);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var found = await _applicationService.DeleteAsync(id);
        if (!found)
            return NotFound();

        return NoContent();
    }
}
