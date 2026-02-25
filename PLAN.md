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
- JWT authentication ✓
- CORS for Angular frontend ✓
- Swagger UI with Bearer auth ✓

---

## Improvement Roadmap

### Tier 1 — Employers Will Look For These

- **Service Layer** ✓ — Service interfaces and implementations in Application layer.
  Repository interfaces in Application; EF Core implementations in Infrastructure.
  Controllers depend only on service interfaces — no DbContext references in Api layer.

- **Unit / Integration Tests** — Create `JobTracker.Tests` xUnit project. Service-layer unit
  tests with an in-memory DB or Moq; controller integration tests via `WebApplicationFactory`.

- **Input Validation** — Add FluentValidation for all Create/Update DTOs. Wire up
  `AddFluentValidationAutoValidation()` so validation errors return 400 automatically.

- **Global Error Handling** — Add `IExceptionHandler` (built-in .NET 8+) returning RFC 7807
  `ProblemDetails`. Eliminates try-catch scattered across controllers.

- **Pagination** — Add `PageNumber` / `PageSize` query params to all GetAll endpoints.
  Return a `PaginatedResult<T>` wrapper so callers know total count.

### Tier 2 — Strong Differentiators

- **EF Core Migrations** — Replace `scripts/schema.sql` with EF migrations so the schema
  is always in sync with the model and the project is runnable from a clean clone.

- **Structured Logging (Serilog)** — Add Serilog writing to console + rolling file with
  structured properties. Demonstrates observability awareness.

- **GitHub Actions CI** — Add `.github/workflows/build.yml` that builds and runs tests on
  push/PR. Signals professional workflow to anyone browsing the repo.

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
