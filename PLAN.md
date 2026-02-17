# JobTracker API Plan

## Goals
- Build a multi-layer .NET 9 Web API to track job applications and interview questions.
- SQL Server database with lookup tables for status and question type.
- Prepare for future Angular frontend in a separate folder.

## Database (SSMS)
- Database: JobTracker
- Tables:
  - ApplicationStatus (lookup)
  - Applications
  - QuestionType (lookup)
  - Questions

## Project Structure (planned)
- JobTrackerAPI.sln
- src/
  - JobTracker.Api
  - JobTracker.Application
  - JobTracker.Domain
  - JobTracker.Infrastructure
- frontend/ (future Angular app)

## API Scope (planned)
- Applications CRUD ✓
- Questions CRUD ✓
- Suggest questions by type ✓

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

## Next Steps
### High Priority Improvements
- **Input Validation** - Add FluentValidation for all DTOs
- **Authentication/Authorization** - Implement JWT auth + role-based access control
- **Global Error Handling** - Add exception middleware for consistent error responses
- **Unit Tests** - Create xUnit test project with comprehensive test coverage

## Next Phase
- Build Angular frontend in separate folder
- Connect frontend to API endpoints
- Add additional features (filters, sorting, reporting)
