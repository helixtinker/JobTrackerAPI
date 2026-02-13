using JobTracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Data;

public class JobTrackerDbContext : DbContext
{
    public JobTrackerDbContext(DbContextOptions<JobTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Application> Applications => Set<Application>();
    public DbSet<ApplicationStatus> ApplicationStatuses => Set<ApplicationStatus>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionType> QuestionTypes => Set<QuestionType>();

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

        modelBuilder.Entity<Application>(entity =>
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
    }
}