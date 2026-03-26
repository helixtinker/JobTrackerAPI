using FluentAssertions;
using JobTracker.Application.Dtos;
using JobTracker.Application.Repositories;
using JobTracker.Application.Services;
using JobTracker.Domain;
using Moq;
using Xunit;

namespace JobTracker.Tests.Services;

public class QuestionServiceTests
{
    private readonly Mock<IQuestionRepository> _mockRepo;
    private readonly Mock<IApplicationRepository> _mockAppRepo;
    private readonly QuestionService _service;

    public QuestionServiceTests()
    {
        _mockRepo = new Mock<IQuestionRepository>();
        _mockAppRepo = new Mock<IApplicationRepository>();
        _service = new QuestionService(_mockRepo.Object, _mockAppRepo.Object);
    }

    // ── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoQuestionsExist()
    {
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var result = await _service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos_ForAllQuestions()
    {
        var questions = new List<Question>
        {
            new() { QuestionId = 1, QuestionText = "What is DI?", QuestionTypeId = 1 },
            new() { QuestionId = 2, QuestionText = "What is async/await?", QuestionTypeId = 2 }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(questions);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().ContainSingle(d => d.QuestionId == 1 && d.QuestionText == "What is DI?");
        result.Should().ContainSingle(d => d.QuestionId == 2 && d.QuestionText == "What is async/await?");
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenQuestionNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Question?)null);

        var result = await _service.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WithNavigationProperties_WhenFound()
    {
        var question = new Question
        {
            QuestionId = 1,
            QuestionText = "What is DI?",
            QuestionTypeId = 1,
            QuestionType = new QuestionType { TypeName = "Behavioral" },
            TechTags = [new QuestionTechTag { Tag = "dotnet" }]
        };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(question);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.QuestionId.Should().Be(1);
        result.QuestionText.Should().Be("What is DI?");
        result.QuestionTypeName.Should().Be("Behavioral");
        result.TechTags.Should().ContainSingle(t => t == "dotnet");
    }

    // ── GetByTypeAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetByTypeAsync_ReturnsMappedDtos_ForMatchingType()
    {
        var questions = new List<Question>
        {
            new() { QuestionId = 1, QuestionTypeId = 3 },
            new() { QuestionId = 2, QuestionTypeId = 3 }
        };
        _mockRepo.Setup(r => r.GetByTypeAsync(3)).ReturnsAsync(questions);

        var result = await _service.GetByTypeAsync(3);

        result.Should().HaveCount(2);
        result.Should().AllSatisfy(d => d.QuestionTypeId.Should().Be(3));
    }

    // ── GetByTechTagsAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task GetByTechTagsAsync_ReturnsMappedDtos_ForMatchingTags()
    {
        var tags = new[] { "React", "TypeScript" };
        var questions = new List<Question>
        {
            new() { QuestionId = 1, QuestionText = "What is JSX?", QuestionTypeId = 1,
                TechTags = [new QuestionTechTag { Tag = "React" }] },
            new() { QuestionId = 2, QuestionText = "What are generics?", QuestionTypeId = 1,
                TechTags = [new QuestionTechTag { Tag = "TypeScript" }] }
        };
        _mockRepo.Setup(r => r.GetByTechTagsAsync(tags)).ReturnsAsync(questions);

        var result = await _service.GetByTechTagsAsync(tags);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByTechTagsAsync_ReturnsEmpty_WhenNoQuestionsMatchTags()
    {
        _mockRepo.Setup(r => r.GetByTechTagsAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync([]);

        var result = await _service.GetByTechTagsAsync(["Cobol"]);

        result.Should().BeEmpty();
    }

    // ── GetByApplicationTechFocusAsync ───────────────────────────────────────

    [Fact]
    public async Task GetByApplicationTechFocusAsync_ReturnsNull_WhenApplicationNotFound()
    {
        _mockAppRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((JobApplication?)null);

        var result = await _service.GetByApplicationTechFocusAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByApplicationTechFocusAsync_ReturnsEmpty_WhenTechFocusIsEmpty()
    {
        var app = new JobApplication { ApplicationId = 1, TechFocus = null };
        _mockAppRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(app);

        var result = await _service.GetByApplicationTechFocusAsync(1);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _mockRepo.Verify(r => r.GetByTechTagsAsync(It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [Fact]
    public async Task GetByApplicationTechFocusAsync_ParsesTechFocusAndQueriesRepo()
    {
        var app = new JobApplication { ApplicationId = 1, TechFocus = "React, TypeScript, .NET" };
        var questions = new List<Question>
        {
            new() { QuestionId = 1, QuestionText = "What is JSX?", QuestionTypeId = 1,
                TechTags = [new QuestionTechTag { Tag = "React" }] }
        };

        _mockAppRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(app);
        _mockRepo.Setup(r => r.GetByTechTagsAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(questions);

        var result = await _service.GetByApplicationTechFocusAsync(1);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        _mockRepo.Verify(r => r.GetByTechTagsAsync(
            It.Is<IEnumerable<string>>(tags => tags.Contains("React") && tags.Contains("TypeScript") && tags.Contains(".NET"))),
            Times.Once);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_CallsAddAndSave_AndReturnsReloadedDto()
    {
        var dto = new CreateQuestionDto { QuestionText = "What is SOLID?", QuestionTypeId = 1 };
        var reloaded = new Question
        {
            QuestionId = 1,
            QuestionText = "What is SOLID?",
            QuestionTypeId = 1,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Question>())).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(reloaded);

        var result = await _service.CreateAsync(dto);

        result.QuestionText.Should().Be("What is SOLID?");
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Question>()), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SetsTechTags_WhenProvided()
    {
        var dto = new CreateQuestionDto
        {
            QuestionText = "What is JSX?",
            QuestionTypeId = 1,
            TechTags = ["React", "JavaScript"]
        };
        Question? captured = null;
        var reloaded = new Question
        {
            QuestionId = 1,
            QuestionText = "What is JSX?",
            QuestionTypeId = 1,
            TechTags = [new QuestionTechTag { Tag = "React" }, new QuestionTechTag { Tag = "JavaScript" }],
            CreatedAt = DateTime.UtcNow
        };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Question>()))
            .Callback<Question>(q => captured = q)
            .Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(reloaded);

        var result = await _service.CreateAsync(dto);

        captured.Should().NotBeNull();
        captured!.TechTags.Should().HaveCount(2);
        captured.TechTags.Select(t => t.Tag).Should().Contain(["React", "JavaScript"]);
        result.TechTags.Should().Contain(["React", "JavaScript"]);
    }

    [Fact]
    public async Task CreateAsync_SetsCreatedAt_OnNewQuestion()
    {
        var dto = new CreateQuestionDto { QuestionText = "What is SOLID?", QuestionTypeId = 1 };
        Question? captured = null;
        var reloaded = new Question { QuestionId = 1, QuestionText = "What is SOLID?", QuestionTypeId = 1, CreatedAt = DateTime.UtcNow };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Question>()))
            .Callback<Question>(q => captured = q)
            .Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(reloaded);

        await _service.CreateAsync(dto);

        captured.Should().NotBeNull();
        captured!.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenQuestionNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Question?)null);

        var result = await _service.UpdateAsync(99, new UpdateQuestionDto { QuestionText = "Test" });

        result.Should().BeFalse();
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue_AndCallsSave_WhenFound()
    {
        var existing = new Question { QuestionId = 1, QuestionText = "Old text", QuestionTypeId = 1 };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(1, new UpdateQuestionDto { QuestionText = "New text", QuestionTypeId = 2 });

        result.Should().BeTrue();
        existing.QuestionText.Should().Be("New text");
        existing.QuestionTypeId.Should().Be(2);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ReplacesTechTags_WhenProvided()
    {
        var existing = new Question
        {
            QuestionId = 1,
            QuestionText = "Old text",
            QuestionTypeId = 1,
            TechTags = [new QuestionTechTag { Tag = "OldTag" }]
        };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.UpdateAsync(1, new UpdateQuestionDto
        {
            QuestionText = "Old text",
            QuestionTypeId = 1,
            TechTags = ["React", "TypeScript"]
        });

        existing.TechTags.Should().HaveCount(2);
        existing.TechTags.Select(t => t.Tag).Should().Contain(["React", "TypeScript"]);
        existing.TechTags.Select(t => t.Tag).Should().NotContain("OldTag");
    }

    [Fact]
    public async Task UpdateAsync_PreservesTechTags_WhenTechTagsNotProvided()
    {
        var existing = new Question
        {
            QuestionId = 1,
            QuestionText = "Old text",
            QuestionTypeId = 1,
            TechTags = [new QuestionTechTag { Tag = "React" }]
        };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.UpdateAsync(1, new UpdateQuestionDto { QuestionText = "New text", QuestionTypeId = 1 });

        existing.TechTags.Should().ContainSingle(t => t.Tag == "React");
    }

    [Fact]
    public async Task UpdateAsync_SetsUpdatedAt_WhenUpdateSucceeds()
    {
        var existing = new Question { QuestionId = 1, QuestionText = "Old text", QuestionTypeId = 1 };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.UpdateAsync(1, new UpdateQuestionDto { QuestionText = "New text", QuestionTypeId = 1 });

        existing.UpdatedAt.Should().NotBeNull();
        existing.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenQuestionNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Question?)null);

        var result = await _service.DeleteAsync(99);

        result.Should().BeFalse();
        _mockRepo.Verify(r => r.Remove(It.IsAny<Question>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_AndCallsRemoveAndSave_WhenFound()
    {
        var existing = new Question { QuestionId = 1, QuestionText = "What is DI?" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        result.Should().BeTrue();
        _mockRepo.Verify(r => r.Remove(existing), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
