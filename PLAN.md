# JobTracker API Plan

## Goals
- Build a multi-layer .NET 9 Web API to track job applications and interview questions.
- SQL Server database with lookup tables for status and question type.
- Serve as a portfolio example demonstrating professional .NET practices.
- Prepare for future Angular frontend in a separate folder.

## Database (SSMS)
- Database: JobTracker
- Tables:
  - ApplicationStatus (lookup)
  - Applications
  - QuestionType (lookup)
  - Questions
  - QuestionTechTags (child of Questions — one-to-many, cascade delete)
  - RecruiterStatus (lookup)
  - Recruiters

## Project Structure

```
JobTrackerAPI.sln
src/
  JobTracker.Api/
    Controllers/         — Thin HTTP adapters; no business logic
    Program.cs           — DI registration, middleware pipeline
  JobTracker.Application/
    Dtos/                — JobApplicationDto, QuestionDto, RecruiterDto (+ Create/Update variants)
    Repositories/        — IApplicationRepository, IQuestionRepository, IRecruiterRepository
    Services/            — Service interfaces + implementations (business logic lives here)
  JobTracker.Domain/
    JobApplication.cs    — Core job application entity
    ApplicationStatus.cs — Lookup entity
    Question.cs
    QuestionTechTag.cs   — Child entity; tags a question with a technology name
    QuestionType.cs
    Recruiter.cs
    RecruiterStatus.cs   — Lookup entity (was RecruiterStatusEntity)
    RecruiterStatusCode.cs — Enum (was RecruiterStatus)
  JobTracker.Infrastructure/
    Data/                — JobTrackerDbContext, EF Core configuration
    Repositories/        — EF Core repository implementations
frontend/ (future Angular app)
```

### Dependency Flow
```
Api  →  Application  ←  Infrastructure
              ↓
           Domain
```
- **Domain** has no external dependencies — pure entities and enums
- **Application** depends only on Domain; owns the service and repository contracts
- **Infrastructure** depends on Application (to implement its interfaces) and Domain
- **Api** depends on Application (interfaces) and Infrastructure (for DI registration)

## API Scope
- Job Applications CRUD ✓
- Questions CRUD ✓
- Recruiters CRUD ✓
- Filter by status / type / company ✓
- Search endpoint (applications) ✓
- Questions by tech tag ✓
- Questions by application TechFocus ✓
- JWT authentication ✓
- CORS for Angular frontend ✓
- Swagger UI with Bearer auth ✓

---

## Improvement Roadmap

### Tier 1 — Employers Will Look For These

- **Service Layer** ✓ — Service interfaces and implementations in Application layer.
  Repository interfaces in Application; EF Core implementations in Infrastructure.
  Controllers depend only on service interfaces — no DbContext references in Api layer.

- **Unit / Integration Tests** ✓ — `JobTracker.Tests` xUnit project with 47 service-layer unit
  tests using Moq (ApplicationService, QuestionService, RecruiterService).

- **Input Validation** ✓ — FluentValidation added for all Create/Update DTOs (JobApplication,
  Question, Recruiter). Auto-validation wired via `AddFluentValidationAutoValidation()`; invalid
  requests return 400 automatically.

- **Global Error Handling** ✓ — `GlobalExceptionHandler` implementing `IExceptionHandler`
  (built-in .NET 8+) logs all unhandled exceptions and returns RFC 7807 `ProblemDetails`.
  No try-catch in controllers.

- **Pagination** — Add `PageNumber` / `PageSize` query params to all GetAll endpoints.
  Return a `PaginatedResult<T>` wrapper so callers know total count.

### Tier 2 — Strong Differentiators

- **EF Core Migrations** ✓ — Schema managed via EF Core migrations in Infrastructure project.
  `dotnet ef database update` creates/updates the database from a clean clone.

- **Structured Logging (Serilog)** — Add Serilog writing to console + rolling file with
  structured properties. Demonstrates observability awareness.

- **GitHub Actions CI/CD** ✓ — `.github/workflows/deploy.yml` builds and runs tests on all
  PRs; deploys to Azure App Service and runs migrations on merge to main.

- **Fix Password Hashing** — Replace SHA-256 (fast general-purpose hash, wrong for
  passwords) with BCrypt.Net or `PasswordHasher<T>` from ASP.NET Core Identity.

### Tier 3 — Polish

- **XML Doc Comments** — Add `<summary>` tags to controller actions to enrich Swagger output.

- **Docker Support** — `Dockerfile` + `docker-compose.yml` (including SQL Server) so the
  project runs with a single command.

---

## Progress Log
- 2026-02-11: Defined SQL schema for Applications + ApplicationStatus.
- 2026-02-11: Defined SQL schema for Questions + QuestionType.
- 2026-02-11: Created this plan file.
- 2026-02-11: Scaffolded solution and projects.
- 2026-02-11: Added EF Core packages and DbContext configuration.
- 2026-02-11: Added domain entities and SQL connection strings.
- 2026-02-11: Removed default demo endpoint.
- 2026-02-12: Added Swagger UI for API testing.
- 2026-02-12: Created ApplicationsController with full CRUD endpoints.
- 2026-02-12: Created QuestionsController with full CRUD endpoints and filtering by type.
- 2026-02-12: Configured EF Core table mappings for SQL Server schema.
- 2026-02-12: API fully functional with 27 job applications and 12 interview questions loaded.
- 2026-02-12: All endpoints tested and working via Swagger UI.
- 2026-02-16: Added Recruiters table and entity with one-to-many relationship to Applications.
- 2026-02-16: Prepared project for GitHub with .gitignore and security review.
- 2026-02-25: Added JWT Bearer authentication and CORS configuration for Angular frontend.
- 2026-02-25: Added service layer with IApplicationService, IQuestionService, IRecruiterService.
  Controllers now depend on interfaces only; all EF access moved out of Api layer.
- 2026-02-25: Added repository layer (IApplicationRepository, IQuestionRepository,
  IRecruiterRepository) in Application; EF implementations in Infrastructure. Services now
  depend on repository interfaces, completing the Dependency Inversion Principle at all layers.
- 2026-02-25: Moved service implementations from Infrastructure to Application — business logic
  now lives alongside its contracts in the Application layer as intended by Clean Architecture.
- 2026-02-25: Renamed domain types for clarity and to eliminate namespace collisions:
  Application → JobApplication, RecruiterStatusEntity → RecruiterStatus,
  RecruiterStatus (enum) → RecruiterStatusCode, ApplicationDto → JobApplicationDto.
  All Domain = alias workarounds removed from the codebase.
- 2026-02-27: Added JobTracker.Tests xUnit project with 39 service-layer unit tests using Moq
  and FluentAssertions. Covers ApplicationService, QuestionService, and RecruiterService
  (CRUD, null-guard paths, timestamp assignment, navigation property mapping).
- 2026-02-27: Added FluentValidation for all Create/Update DTOs (JobApplication, Question,
  Recruiter). Validators live in Application/Validators/. Auto-validation wired in Program.cs
  via AddFluentValidationAutoValidation(); invalid requests return 400 automatically.
- 2026-02-27: Added GlobalExceptionHandler (IExceptionHandler) + AddProblemDetails. All
  unhandled exceptions now return RFC 7807 ProblemDetails 500 and are logged via ILogger.
- 2026-03-16: Added EF Core migrations (InitialCreate). Schema now managed via migrations
  instead of scripts/schema.sql.
- 2026-03-16: Deployed to Azure App Service (job-tracker-api-krks.azurewebsites.net) with
  Azure SQL Database. GitHub Actions CI/CD pipeline builds, tests, runs migrations, and
  deploys on merge to main. Branch protection enforces PR + passing checks before merge.
- 2026-03-26: Added QuestionTechTag entity and QuestionTechTags table (one-to-many child of
  Questions, cascade delete). Questions can now carry multiple technology tags. Added
  GET /api/questions/by-tech?tag= to filter questions by a single tag, and
  GET /api/applications/{id}/questions to return interview prep questions matched to an
  application's TechFocus field (parsed and matched case-insensitively). QuestionService
  now takes IApplicationRepository to support the TechFocus lookup. 8 new unit tests added
  (47 total). EF Core migration AddQuestionTechTags applied.
