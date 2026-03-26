using JobTracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Data;

public class JobTrackerDbContext : DbContext
{
    public JobTrackerDbContext(DbContextOptions<JobTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<JobApplication> Applications => Set<JobApplication>();
    public DbSet<ApplicationStatus> ApplicationStatuses => Set<ApplicationStatus>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionType> QuestionTypes => Set<QuestionType>();
    public DbSet<QuestionTechTag> QuestionTechTags => Set<QuestionTechTag>();
    public DbSet<Recruiter> Recruiters => Set<Recruiter>();
    public DbSet<RecruiterStatus> RecruiterStatuses => Set<RecruiterStatus>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationStatus>(entity =>
        {
            entity.ToTable("ApplicationStatus");
            entity.HasKey(e => e.StatusId);
            entity.Property(e => e.StatusName)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<JobApplication>(entity =>
        {
            entity.ToTable("Applications");
            entity.HasKey(e => e.ApplicationId);
            entity.Property(e => e.JobTitle).HasMaxLength(200);
            entity.Property(e => e.CompanyName).HasMaxLength(200);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.JobPostUrl).HasMaxLength(1000);
            entity.Property(e => e.CompanyWebsite).HasMaxLength(1000);
            entity.Property(e => e.NetworkContacts).HasMaxLength(1000);
            entity.Property(e => e.TechFocus).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasOne(e => e.Status)
                .WithMany(s => s.Applications)
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Recruiter)
                .WithMany(r => r.Applications)
                .HasForeignKey(e => e.RecruiterId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<QuestionType>(entity =>
        {
            entity.ToTable("QuestionType");
            entity.HasKey(e => e.QuestionTypeId);
            entity.Property(e => e.TypeName)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("Questions");
            entity.HasKey(e => e.QuestionId);
            entity.Property(e => e.QuestionText)
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasOne(e => e.QuestionType)
                .WithMany(t => t.Questions)
                .HasForeignKey(e => e.QuestionTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<QuestionTechTag>(entity =>
        {
            entity.ToTable("QuestionTechTags");
            entity.HasKey(e => e.QuestionTechTagId);
            entity.Property(e => e.Tag)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(e => e.Question)
                .WithMany(q => q.TechTags)
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.Tag, e.QuestionId })
                .HasDatabaseName("IX_QuestionTechTags_Tag_QuestionId");
        });

        modelBuilder.Entity<Recruiter>(entity =>
        {
            entity.ToTable("Recruiters");
            entity.HasKey(e => e.RecruiterId);
            entity.Property(e => e.RecruiterName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Company).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.LinkedInUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasOne(e => e.Status)
                .WithMany(s => s.Recruiters)
                .HasForeignKey(e => e.RecruiterStatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RecruiterStatus>(entity =>
        {
            entity.ToTable("RecruiterStatus");
            entity.HasKey(e => e.RecruiterStatusId);
            entity.Property(e => e.StatusName)
                .IsRequired()
                .HasMaxLength(50);
        });
    }
}
