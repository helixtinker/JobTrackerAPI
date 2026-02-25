# JobTracker API

A multi-layer .NET 9 Web API for tracking job applications and interview questions. Secured with JWT Bearer authentication for use with an Angular frontend.

## Solution Structure
- src/JobTracker.Api: API host (controllers, DI, configuration)
- src/JobTracker.Application: business logic (planned)
- src/JobTracker.Domain: entities and domain types
- src/JobTracker.Infrastructure: EF Core, data access
- scripts/schema.sql: SQL Server schema for manual setup

## Setup

### Prerequisites
- .NET 9 SDK
- SQL Server or SQL Server Express

### Database Configuration
1. Create a SQL Server database named `JobTracker`
2. Run the schema script: [scripts/schema.sql](scripts/schema.sql)
3. Update your connection string in `src/JobTracker.Api/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "JobTracker": "Data Source=YOUR_SERVER\\SQLEXPRESS;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Initial Catalog=JobTracker"
     }
   }
   ```
   Replace `YOUR_SERVER` with your SQL Server instance name.

### Authentication Configuration
All API endpoints require a JWT Bearer token. Before running the API, set the following values in `appsettings.Development.json`:

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

Add both values to your config:
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
- `sample-applications.sql` - Sample job applications
- `sample-questions.sql` - Sample interview questions

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

// Authenticated request (fetch)
const data = await fetch('/api/applications', {
  headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
});

// Or set globally with Axios
axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
```

## API Endpoints

All endpoints require a valid JWT token except `POST /api/auth/login`.

### Auth
- `POST /api/auth/login` - Authenticate and receive a JWT token

### Applications
- `GET /api/applications` - Get all applications
- `GET /api/applications/{id}` - Get application by ID
- `GET /api/applications/by-StatusId/{statusId}` - Filter by status
- `GET /api/applications/search` - Search (companyName, recruiterName, techFocus, notes, statusId)
- `POST /api/applications` - Create new application
- `PUT /api/applications/{id}` - Update application
- `DELETE /api/applications/{id}` - Delete application

### Questions
- `GET /api/questions` - Get all questions
- `GET /api/questions/{id}` - Get question by ID
- `GET /api/questions/by-type/{typeId}` - Filter questions by type (1=Behavioral, 2=Technical, 3=Experience)
- `POST /api/questions` - Create new question
- `PUT /api/questions/{id}` - Update question
- `DELETE /api/questions/{id}` - Delete question

### Recruiters
- `GET /api/recruiters` - Get all recruiters
- `GET /api/recruiters/{id}` - Get recruiter by ID
- `GET /api/recruiters/by-company/{company}` - Filter recruiters by company name
- `POST /api/recruiters` - Create new recruiter
- `PUT /api/recruiters/{id}` - Update recruiter
- `DELETE /api/recruiters/{id}` - Delete recruiter

## Status

### Current Implementation ✅
- ✅ EF Core DbContext configured and tested
- ✅ Domain entities created (Applications, Questions, Recruiters)
- ✅ Full CRUD API endpoints implemented
- ✅ Search endpoint for applications
- ✅ Swagger UI enabled for testing
- ✅ SQL Server integration working
- ✅ Sample data loaded (12 recruiters, 27 applications, 12 questions)
- ✅ Recruiter-Application one-to-many relationship
- ✅ GitHub-ready with .gitignore and security review
- ✅ JWT Bearer authentication
- ✅ CORS configured for Angular/React frontend (localhost:4200 / localhost:3000)

## Next Steps

### High Priority Improvements
- **Input Validation** - Add FluentValidation for all DTOs
- **Global Error Handling** - Add exception middleware for consistent error responses
- **Unit Tests** - Create xUnit test project with comprehensive test coverage

### Feature Development
- Create Angular frontend in separate folder
- Build views for applications management
- Build views for interview questions
- Build recruiter management UI
- Connect frontend to API endpoints

### Additional Enhancements
- Pagination for GetAll endpoints (currently loads all data)
- Logging infrastructure (Serilog)
- Response standardization with wrapper objects
- Advanced filtering and sorting options
