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
public class QuestionsController : ControllerBase
{
    private readonly JobTrackerDbContext _dbContext;

    public QuestionsController(JobTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetAll()
    {
        var questions = await _dbContext.Questions
            .Include(q => q.QuestionType)
            .ToListAsync();

        var dtos = questions.Select(q => new QuestionDto
        {
            QuestionId = q.QuestionId,
            QuestionText = q.QuestionText,
            AnswerText = q.AnswerText,
            QuestionTypeId = q.QuestionTypeId,
            QuestionTypeName = q.QuestionType?.TypeName,
            CreatedAt = q.CreatedAt,
            UpdatedAt = q.UpdatedAt
        }).ToList();

        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionDto>> GetById(int id)
    {
        var question = await _dbContext.Questions
            .Include(q => q.QuestionType)
            .FirstOrDefaultAsync(q => q.QuestionId == id);

        if (question is null)
            return NotFound();

        var dto = new QuestionDto
        {
            QuestionId = question.QuestionId,
            QuestionText = question.QuestionText,
            AnswerText = question.AnswerText,
            QuestionTypeId = question.QuestionTypeId,
            QuestionTypeName = question.QuestionType?.TypeName,
            CreatedAt = question.CreatedAt,
            UpdatedAt = question.UpdatedAt
        };

        return Ok(dto);
    }

    [HttpGet("by-type/{typeId}")]
    public async Task<ActionResult<IEnumerable<QuestionDto>>> GetByType(int typeId)
    {
        var questions = await _dbContext.Questions
            .Include(q => q.QuestionType)
            .Where(q => q.QuestionTypeId == typeId)
            .ToListAsync();

        var dtos = questions.Select(q => new QuestionDto
        {
            QuestionId = q.QuestionId,
            QuestionText = q.QuestionText,
            AnswerText = q.AnswerText,
            QuestionTypeId = q.QuestionTypeId,
            QuestionTypeName = q.QuestionType?.TypeName,
            CreatedAt = q.CreatedAt,
            UpdatedAt = q.UpdatedAt
        }).ToList();

        return Ok(dtos);
    }

    [HttpPost]
    public async Task<ActionResult<QuestionDto>> Create(CreateQuestionDto createDto)
    {
        var question = new DomainModel.Question
        {
            QuestionText = createDto.QuestionText,
            AnswerText = createDto.AnswerText,
            QuestionTypeId = createDto.QuestionTypeId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Questions.Add(question);
        await _dbContext.SaveChangesAsync();

        var questionType = await _dbContext.QuestionTypes.FindAsync(question.QuestionTypeId);

        var resultDto = new QuestionDto
        {
            QuestionId = question.QuestionId,
            QuestionText = question.QuestionText,
            AnswerText = question.AnswerText,
            QuestionTypeId = question.QuestionTypeId,
            QuestionTypeName = questionType?.TypeName,
            CreatedAt = question.CreatedAt,
            UpdatedAt = question.UpdatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = question.QuestionId }, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateQuestionDto updateDto)
    {
        var question = await _dbContext.Questions.FindAsync(id);
        if (question is null)
            return NotFound();

        question.QuestionText = updateDto.QuestionText ?? question.QuestionText;
        question.AnswerText = updateDto.AnswerText ?? question.AnswerText;
        question.QuestionTypeId = updateDto.QuestionTypeId;
        question.UpdatedAt = DateTime.UtcNow;

        _dbContext.Questions.Update(question);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var question = await _dbContext.Questions.FindAsync(id);
        if (question is null)
            return NotFound();

        _dbContext.Questions.Remove(question);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}
