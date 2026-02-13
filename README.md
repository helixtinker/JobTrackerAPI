# JobTracker API

A multi-layer .NET 9 Web API for tracking job applications and interview questions.

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

### Optional: Sample Data
Sample data files are available in the `scripts/` folder but are not included in the repository:
- `sample-applications.sql` - Sample job applications
- `sample-questions.sql` - Sample interview questions

## API Endpoints

### Applications
- `GET /api/applications` - Get all applications
- `GET /api/applications/{id}` - Get application by ID
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

## Status
✅ EF Core DbContext configured and tested
✅ Domain entities created
✅ Full CRUD API endpoints implemented
✅ Swagger UI enabled for testing
✅ SQL Server integration working
✅ Sample data loaded 

## Running the API
From the project root:
```bash
dotnet run --project "src/JobTracker.Api/JobTracker.Api.csproj"
```
Swagger UI will be available at `http://localhost:5085`

## Next Steps
- Create Angular frontend in separate folder
- Build views for applications management
- Build views for interview questions
- Connect frontend to API endpoints
