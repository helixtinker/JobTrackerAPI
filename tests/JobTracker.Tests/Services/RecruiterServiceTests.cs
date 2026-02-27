using FluentAssertions;
using JobTracker.Application.Dtos;
using JobTracker.Application.Repositories;
using JobTracker.Application.Services;
using JobTracker.Domain;
using Moq;
using Xunit;

namespace JobTracker.Tests.Services;

public class RecruiterServiceTests
{
    private readonly Mock<IRecruiterRepository> _mockRepo;
    private readonly RecruiterService _service;

    public RecruiterServiceTests()
    {
        _mockRepo = new Mock<IRecruiterRepository>();
        _service = new RecruiterService(_mockRepo.Object);
    }

    // ── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoRecruitersExist()
    {
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var result = await _service.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos_ForAllRecruiters()
    {
        var recruiters = new List<Recruiter>
        {
            new() { RecruiterId = 1, RecruiterName = "Alice", Company = "Acme" },
            new() { RecruiterId = 2, RecruiterName = "Bob", Company = "Globex" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(recruiters);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().ContainSingle(d => d.RecruiterId == 1 && d.RecruiterName == "Alice");
        result.Should().ContainSingle(d => d.RecruiterId == 2 && d.RecruiterName == "Bob");
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenRecruiterNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Recruiter?)null);

        var result = await _service.GetByIdAsync(99);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDto_WithNavigationProperties_WhenFound()
    {
        var recruiter = new Recruiter
        {
            RecruiterId = 1,
            RecruiterName = "Alice",
            Status = new RecruiterStatus { StatusName = "Active" }
        };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(recruiter);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.RecruiterId.Should().Be(1);
        result.RecruiterName.Should().Be("Alice");
        result.StatusName.Should().Be("Active");
    }

    // ── GetByCompanyAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task GetByCompanyAsync_ReturnsMappedDtos_ForMatchingCompany()
    {
        var recruiters = new List<Recruiter>
        {
            new() { RecruiterId = 1, RecruiterName = "Alice", Company = "Acme" }
        };
        _mockRepo.Setup(r => r.GetByCompanyAsync("Acme")).ReturnsAsync(recruiters);

        var result = await _service.GetByCompanyAsync("Acme");

        result.Should().HaveCount(1);
        result.First().RecruiterName.Should().Be("Alice");
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_DefaultsRecruiterStatusIdToOne_WhenNotProvided()
    {
        var dto = new CreateRecruiterDto { RecruiterName = "Alice" };
        Recruiter? captured = null;
        var reloaded = new Recruiter { RecruiterId = 1, RecruiterName = "Alice", RecruiterStatusId = 1, CreatedAt = DateTime.UtcNow };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Recruiter>()))
            .Callback<Recruiter>(r => captured = r)
            .Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(reloaded);

        await _service.CreateAsync(dto);

        captured.Should().NotBeNull();
        captured!.RecruiterStatusId.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_UsesProvidedRecruiterStatusId_WhenSupplied()
    {
        var dto = new CreateRecruiterDto { RecruiterName = "Bob", RecruiterStatusId = 3 };
        Recruiter? captured = null;
        var reloaded = new Recruiter { RecruiterId = 1, RecruiterName = "Bob", RecruiterStatusId = 3, CreatedAt = DateTime.UtcNow };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Recruiter>()))
            .Callback<Recruiter>(r => captured = r)
            .Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(reloaded);

        await _service.CreateAsync(dto);

        captured!.RecruiterStatusId.Should().Be(3);
    }

    [Fact]
    public async Task CreateAsync_SetsCreatedAt_AndReturnsDto()
    {
        var dto = new CreateRecruiterDto { RecruiterName = "Alice" };
        var reloaded = new Recruiter { RecruiterId = 1, RecruiterName = "Alice", CreatedAt = DateTime.UtcNow };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Recruiter>())).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(reloaded);

        var result = await _service.CreateAsync(dto);

        result.RecruiterName.Should().Be("Alice");
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Recruiter>()), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenRecruiterNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Recruiter?)null);

        var result = await _service.UpdateAsync(99, new UpdateRecruiterDto());

        result.Should().BeNull();
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsDto_AndCallsSave_WhenFound()
    {
        var existing = new Recruiter { RecruiterId = 1, RecruiterName = "Alice", Company = "Acme" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(1, new UpdateRecruiterDto { Company = "NewCo" });

        result.Should().NotBeNull();
        result!.Company.Should().Be("NewCo");
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_PreservesRecruiterName_WhenDtoNameIsNullOrEmpty()
    {
        var existing = new Recruiter { RecruiterId = 1, RecruiterName = "Alice", Company = "Acme" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.UpdateAsync(1, new UpdateRecruiterDto { RecruiterName = null, Company = "NewCo" });

        existing.RecruiterName.Should().Be("Alice");
        existing.Company.Should().Be("NewCo");
    }

    [Fact]
    public async Task UpdateAsync_SetsUpdatedAt_WhenUpdateSucceeds()
    {
        var existing = new Recruiter { RecruiterId = 1, RecruiterName = "Alice" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        await _service.UpdateAsync(1, new UpdateRecruiterDto());

        existing.UpdatedAt.Should().NotBeNull();
        existing.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenRecruiterNotFound()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Recruiter?)null);

        var result = await _service.DeleteAsync(99);

        result.Should().BeFalse();
        _mockRepo.Verify(r => r.Remove(It.IsAny<Recruiter>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_AndCallsRemoveAndSave_WhenFound()
    {
        var existing = new Recruiter { RecruiterId = 1, RecruiterName = "Alice" };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        result.Should().BeTrue();
        _mockRepo.Verify(r => r.Remove(existing), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
