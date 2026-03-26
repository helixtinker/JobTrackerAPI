using JobTracker.Application.Dtos;
using JobTracker.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAll()
    {
        var questions = await _questionService.GetAllAsync();
        return Ok(questions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionDto>> GetById(int id)
    {
        var question = await _questionService.GetByIdAsync(id);
        if (question is null)
            return NotFound();

        return Ok(question);
    }

    [HttpGet("by-type/{typeId}")]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetByType(int typeId)
    {
        var questions = await _questionService.GetByTypeAsync(typeId);
        return Ok(questions);
    }

    [HttpGet("by-tech")]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetByTech([FromQuery] string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return BadRequest("A tag must be provided.");

        var questions = await _questionService.GetByTechTagsAsync([tag]);
        return Ok(questions);
    }

    [HttpPost]
    public async Task<ActionResult<QuestionDto>> Create(CreateQuestionDto createDto)
    {
        var question = await _questionService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = question.QuestionId }, question);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateQuestionDto updateDto)
    {
        var found = await _questionService.UpdateAsync(id, updateDto);
        if (!found)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var found = await _questionService.DeleteAsync(id);
        if (!found)
            return NotFound();

        return NoContent();
    }
}
