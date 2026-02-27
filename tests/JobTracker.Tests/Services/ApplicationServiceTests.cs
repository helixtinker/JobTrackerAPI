using FluentAssertions;
using JobTracker.Application.Dtos;
using JobTracker.Application.Repositories;
using JobTracker.Application.Services;
using JobTracker.Domain;
using Moq;
using Xunit;

namespace JobTracker.Tests.Services;

public class ApplicationServiceTests
{
    private readonly Mock<IApplicationRepository> _mockRepo;
    private readonly ApplicationService _service;

    public ApplicationServiceTests()
    {
        _mockRepo = new Mock<IApplicationRepository>();
        _service = new ApplicationService(_mockRepo.Object);
    }

    // ── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoApplicationsExist()
    {
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var result = await _service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos_ForAllApplications()
    {
        var applications = new List<JobApplication>
        {
            new() { ApplicationId = 1, CompanyName = "Acme", JobTitle = "Developer" },
            new() { ApplicationId = 2, CompanyName = "Globex", JobTitle = "Engineer" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(applications);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().ContainSingle(d => d.ApplicationId == 1 && d.CompanyName == "Acme");
        result.Should().ContainSingle(d => d.ApplicationId == 2 && d.CompanyName == "Globex");
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenApplicationNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((JobApplication?)null);

        var result = await _service.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WithNavigationProperties_WhenFound()
    {
        var application = new JobApplication
        {
            ApplicationId = 1,
            CompanyName = "Acme",
            JobTitle = "Developer",
            Status = new ApplicationStatus { StatusName = "Applied" },
            Recruiter = new Recruiter { RecruiterName = "Alice" }
        };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(application);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.ApplicationId.Should().Be(1);
        result.CompanyName.Should().Be("Acme");
        result.StatusName.Should().Be("Applied");
        result.RecruiterName.Should().Be("Alice");
    }

    // ── GetByStatusIdAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task GetByStatusIdAsync_ReturnsMappedDtos_ForMatchingStatus()
    {
        var applications = new List<JobApplication>
        {
            new() { ApplicationId = 1, StatusId = 2 },
            new() { ApplicationId = 2, StatusId = 2 }
        };
        _mockRepo.Setup(r => r.GetByStatusIdAsync(2)).ReturnsAsync(applications);

        var result = await _service.GetByStatusIdAsync(2);

        result.Should().HaveCount(2);
        result.Should().AllSatisfy(d => d.StatusId.Should().Be(2));
    }

    // ── SearchAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task SearchAsync_DelegatesToRepository_AndReturnsMappedDtos()
    {
        var applications = new List<JobApplication>
        {
            new() { ApplicationId = 1, CompanyName = "Acme" }
        };
        _mockRepo.Setup(r => r.SearchAsync("Acme", null, null, null, null))
            .ReturnsAsync(applications);

        var result = await _service.SearchAsync("Acme", null, null, null, null);

        result.Should().HaveCount(1);
        result.First().CompanyName.Should().Be("Acme");
        _mockRepo.Verify(r => r.SearchAsync("Acme", null, null, null, null), Times.Once);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_CallsAddAndSave_AndReturnsReloadedDto()
    {
        var dto = new CreateJobApplicationDto { CompanyName = "Acme", JobTitle = "Developer" };
        var reloaded = new JobApplication
        {
            ApplicationId = 1,
            CompanyName = "Acme",
            JobTitle = "Developer",
            CreatedAt = DateTime.UtcNow
        };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<JobApplication>())).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(reloaded);

        var result = await _service.CreateAsync(dto);

        result.CompanyName.Should().Be("Acme");
        result.JobTitle.Should().Be("Developer");
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<JobApplication>()), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SetsCreatedAt_OnNewApplication()
    {
        var dto = new CreateJobApplicationDto { CompanyName = "Acme" };
        JobApplication? captured = null;
        var reloaded = new JobApplication { ApplicationId = 1, CompanyName = "Acme", CreatedAt = DateTime.UtcNow };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<JobApplication>()))
            .Callback<JobApplication>(a => captured = a)
            .Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(reloaded);

        await _service.CreateAsync(dto);

        captured.Should().NotBeNull();
        captured!.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenApplicationNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((JobApplication?)null);

        var result = await _service.UpdateAsync(99, new UpdateJobApplicationDto());

        result.Should().BeFalse();
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue_AndCallsSave_WhenFound()
    {
        var existing = new JobApplication { ApplicationId = 1, CompanyName = "OldCo" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(1, new UpdateJobApplicationDto { CompanyName = "NewCo" });

        result.Should().BeTrue();
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_PreservesExistingValues_WhenDtoFieldsAreNull()
    {
        var existing = new JobApplication
        {
            ApplicationId = 1,
            JobTitle = "Original Title",
            CompanyName = "Original Co"
        };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.UpdateAsync(1, new UpdateJobApplicationDto { CompanyName = "New Co" });

        existing.JobTitle.Should().Be("Original Title");
        existing.CompanyName.Should().Be("New Co");
        existing.UpdatedAt.Should().NotBeNull();
        existing.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenApplicationNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((JobApplication?)null);

        var result = await _service.DeleteAsync(99);

        result.Should().BeFalse();
        _mockRepo.Verify(r => r.Remove(It.IsAny<JobApplication>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_AndCallsRemoveAndSave_WhenFound()
    {
        var existing = new JobApplication { ApplicationId = 1 };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        result.Should().BeTrue();
        _mockRepo.Verify(r => r.Remove(existing), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
