# JobTracker API

A .NET 9 Web API for tracking job applications, interview questions, and recruiter contacts. Built with Clean Architecture to demonstrate layered separation of concerns, the repository pattern, dependency inversion, and JWT authentication.

## Architecture

The solution is split into four projects with a strict one-way dependency flow:

```
Api  →  Application  ←  Infrastructure
              ↓
           Domain
```

| Project | Responsibility |
|---|---|
| `JobTracker.Domain` | Entities and enums. No external dependencies. |
| `JobTracker.Application` | DTOs, repository interfaces, service interfaces and implementations. Depends only on Domain. |
| `JobTracker.Infrastructure` | EF Core `DbContext` and repository implementations. Depends on Application and Domain. |
| `JobTracker.Api` | Controllers, `Program.cs`, DI registration. Depends on Application and Infrastructure. |

**Key design decisions:**
- Controllers depend on service **interfaces** (`IApplicationService`, etc.), not concrete classes — the composition root (`Program.cs`) is the only place that binds interfaces to implementations.
- Services depend on repository **interfaces** (`IApplicationRepository`, etc.), not on `DbContext` directly — EF Core is an Infrastructure detail, invisible to business logic.
- Service implementations live in the **Application layer** alongside their interfaces, because orchestrating business logic is an application concern, not an infrastructure one.
- The domain entity for a job application is named `JobApplication` (not `Application`) to avoid a namespace collision with the `JobTracker.Application` project — a concrete example of naming decisions that affect the whole codebase.

## Solution Structure

```
src/
├── JobTracker.Api/
│   ├── Controllers/          ApplicationsController, QuestionsController,
│   │                         RecruitersController, AuthController
│   └── Program.cs            DI registration, middleware pipeline
│
├── JobTracker.Application/
│   ├── Dtos/                 JobApplicationDto, QuestionDto, RecruiterDto
│   │                         (+ Create and Update variants for each)
│   ├── Repositories/         IApplicationRepository, IQuestionRepository,
│   │                         IRecruiterRepository
│   ├── Services/             IApplicationService / ApplicationService
│   │                         IQuestionService / QuestionService
│   │                         IRecruiterService / RecruiterService
│   └── Validators/           FluentValidation validators for all Create/Update DTOs
│
├── JobTracker.Domain/
│   ├── JobApplication.cs     Core job application entity
│   ├── ApplicationStatus.cs  Lookup entity
│   ├── Question.cs
│   ├── QuestionType.cs
│   ├── Recruiter.cs
│   ├── RecruiterStatus.cs    Lookup entity
│   └── RecruiterStatusCode.cs  Enum (Active, Inactive, DoNotContact)
│
└── JobTracker.Infrastructure/
    ├── Data/                 JobTrackerDbContext (EF Core + Fluent API config)
    └── Repositories/         ApplicationRepository, QuestionRepository,
                              RecruiterRepository

tests/
└── JobTracker.Tests/
    └── Services/             Unit tests for ApplicationService, QuestionService,
                              RecruiterService (xUnit + Moq + FluentAssertions)
```

## Setup

### Prerequisites
- .NET 9 SDK
- SQL Server or SQL Server Express

### Database Configuration
1. Create a SQL Server database named `JobTracker`
2. Run the schema script: [scripts/schema.sql](scripts/schema.sql)
3. Add your connection string to `src/JobTracker.Api/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "JobTracker": "Data Source=YOUR_SERVER\\SQLEXPRESS;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Initial Catalog=JobTracker"
     }
   }
   ```
   Replace `YOUR_SERVER` with your SQL Server instance name.

### Authentication Configuration
All API endpoints require a JWT Bearer token. Set the following in `appsettings.Development.json`:

**1. Generate a JWT signing key** (must be at least 32 characters):
```powershell
$bytes = [byte[]]::new(32)
[System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)
```

**2. Generate your password hash** (SHA-256, Base64-encoded):
```powershell
[Convert]::ToBase64String(
  [System.Security.Cryptography.SHA256]::Create().ComputeHash(
    [System.Text.Encoding]::UTF8.GetBytes("your-password-here")
  )
)
```

Add both to your config:
```json
{
  "Jwt": {
    "Key": "<output of step 1>",
    "Issuer": "JobTrackerAPI",
    "Audience": "JobTrackerClient",
    "ExpirationHours": 8
  },
  "Auth": {
    "Username": "admin",
    "PasswordHash": "<output of step 2>"
  }
}
```

> These values contain secrets — keep them out of source control. Use `appsettings.Development.json` (already in `.gitignore`) or [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for local development.

### Optional: Sample Data
Sample data files are available in the `scripts/` folder but are not included in the repository:
- `sample-applications.sql` — Sample job applications
- `sample-questions.sql` — Sample interview questions

## Running the API

```bash
dotnet run --project "src/JobTracker.Api/JobTracker.Api.csproj"
```
Swagger UI will be available at `http://localhost:5085`

## Authentication

### Login
```
POST /api/auth/login
Content-Type: application/json

{ "username": "admin", "password": "your-password" }
```
Response:
```json
{ "token": "eyJ...", "expiration": "2026-02-25T..." }
```

### Using the token
Include the token in the `Authorization` header on all subsequent requests:
```
Authorization: Bearer eyJ...
```

In Swagger UI, click the **Authorize** button (top right) and enter your token to authenticate all requests.

### Angular integration
```typescript
// Login and store token
this.http.post<{ token: string }>('/api/auth/login', { username, password })
  .subscribe(res => localStorage.setItem('token', res.token));

// Attach token via HttpInterceptor
headers = headers.set('Authorization', `Bearer ${localStorage.getItem('token')}`);
```

### React integration
```typescript
// Login and store token
const res = await fetch('/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ username, password })
});
const { token } = await res.json();
localStorage.setItem('token', token);

// Authenticated request
const data = await fetch('/api/applications', {
  headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
});

// Or set globally with Axios
axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
```

## API Endpoints

All endpoints require a valid JWT token except `POST /api/auth/login`.

### Auth
- `POST /api/auth/login` — Authenticate and receive a JWT token

### Job Applications
- `GET /api/applications` — Get all applications
- `GET /api/applications/{id}` — Get application by ID
- `GET /api/applications/by-StatusId/{statusId}` — Filter by status
- `GET /api/applications/search` — Search (companyName, recruiterName, techFocus, notes, statusId)
- `POST /api/applications` — Create new application
- `PUT /api/applications/{id}` — Update application
- `DELETE /api/applications/{id}` — Delete application

### Interview Questions
- `GET /api/questions` — Get all questions
- `GET /api/questions/{id}` — Get question by ID
- `GET /api/questions/by-type/{typeId}` — Filter by type (1=Behavioral, 2=Technical, 3=Experience)
- `POST /api/questions` — Create new question
- `PUT /api/questions/{id}` — Update question
- `DELETE /api/questions/{id}` — Delete question

### Recruiters
- `GET /api/recruiters` — Get all recruiters
- `GET /api/recruiters/{id}` — Get recruiter by ID
- `GET /api/recruiters/by-company/{company}` — Filter by company name
- `POST /api/recruiters` — Create new recruiter
- `PUT /api/recruiters/{id}` — Update recruiter
- `DELETE /api/recruiters/{id}` — Delete recruiter

## Implementation Status

| Feature | Status |
|---|---|
| Clean Architecture layering | ✅ |
| Repository pattern with interfaces | ✅ |
| Dependency Inversion at all layers | ✅ |
| JWT Bearer authentication | ✅ |
| CORS for Angular/React frontend | ✅ |
| Swagger UI with Bearer auth support | ✅ |
| EF Core + SQL Server | ✅ |
| Full CRUD for all three resources | ✅ |
| Search endpoint (applications) | ✅ |
| Input validation (FluentValidation) | ✅ |
| Global error handling (ProblemDetails) | 🔲 |
| Unit and integration tests | ✅ |
| Pagination | 🔲 |
| EF Core migrations | 🔲 |
| Structured logging (Serilog) | 🔲 |
| GitHub Actions CI | 🔲 |
