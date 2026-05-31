# EduFlow

A REST API for a learning management system, built as a backend architecture demonstration in ASP.NET Core 8.

EduFlow models the core workflow of an online course platform. Professors create courses with modules, quizzes, and assignments; students enroll, submit assignments, and receive grades, an admin role provisions professor accounts. The scope is narrow by intent. The goal is not feature functional equality with platform such as Moodle, but a clean, layered, well-tested implementation of the patterns a junior .NET backend engineer is expected to know: layered architecture, repository and service patterns, JWT-based role authorization, EF Core code-first migrations, structured logging, global exception handling, and unit testing with mocks.

## Tech Stack

- **.NET 8** — ASP.NET Core Web API
- **Entity Framework Core 8** (SQL Server, code-first)
- **JWT Bearer** authentication
- **BCrypt.Net-Next** for password hashing
- **Swashbuckle** (Swagger / OpenAPI)
- **xUnit** + **Moq** for unit testing
- **Microsoft.Extensions.Logging** for structured logging

## Architecture

EduFlow follows a strict layered architecture. Each layer depends only on the one directly below it. Controllers never touch DbContext directly, services never return HTTP responses, repositories never know about DTOs.
```
HTTP request
    │
    ▼
┌─────────────────────────────────┐
│  Controllers                    │  parse DTOs, extract JWT claims,
│  (DTOs in / DTOs out)           │  delegate to services
└─────────────────────────────────┘
    │
    ▼
┌─────────────────────────────────┐
│  Services                       │  business rules, throw typed exceptions
│  (primitives in / models out)   │  (ArgumentException, KeyNotFoundException,
│                                 │   UnauthorizedAccessException)
└─────────────────────────────────┘
    │
    ▼
┌─────────────────────────────────┐
│  Repositories                   │  EF Core queries, one repo per aggregate
│  (IRepository<T> + extensions)  │  root, extending a generic base
└─────────────────────────────────┘
    │
    ▼
┌─────────────────────────────────┐
│  AppDbContext                   │  EF Core code-first
└─────────────────────────────────┘
    │
    ▼
  SQL Server
```
A global ExceptionHandlingMiddleware maps service-thrown exceptions to HTTP status codes (ArgumentException → 400, UnauthorizedAccessException → 401, KeyNotFoundException → 404, anything else → 500 with the exception logged). Controllers contain no try/catch.

### Roles

Three roles, enforced via JWT claims and [Authorize(Roles = "...")] attributes:

- **Admin** — provisions professor accounts.
- **Professor** — creates courses, modules, quizzes, and assignments, grades submissions.
- **Student** — enrolls in courses, submits assignments.

Students self-register via the public /api/Auth/register endpoint. Professors must be created by an admin.

### Domain hierarchy
```
Course ──┬── Module ──┬── Quiz
         │            └── Assignment ── AssignmentSubmission
         └── Enrollment ── (links Student to Course)
```
A course has one professor (owner) and many enrolled students. A module belongs to one course. Quizzes and assignments belong to one module. Submissions belong to one assignment and one student.


### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB, Express, or full edition)
- A SQL client such as SSMS or the Visual Studio SQL Server Object Explorer (optional, for inspection)

### Setup

1. **Clone the repository**
2. 
   git clone https://github.com/Dalibor98/EduFlow.git
   cd EduFlow

3. **Configure the connection string** (optional)

   The default appsettings.json points at `Server=localhost;Database=EduFlowDb;Trusted_Connection=True;TrustServerCertificate=True;`. Adjust if your local SQL Server uses a different instance name or auth mode.

4. **Apply migrations**

   From the repository root:
   dotnet ef database update --project EduFlow

   Or, in Visual Studio's Package Manager Console with `EduFlow` as the default project:
   Update-Database

   This creates `EduFlowDb` and applies all migrations, including the seed migration that populates a sample course hierarchy.

5. **Run the API**

   dotnet run --project EduFlow

   Or press F5 in Visual Studio. Swagger UI opens at https://localhost:7056/swagger (or http://localhost:5232/swagger).

**OR**

## Live Demo

The API is deployed on Azure App Service with an Azure SQL backend:

**https://app-eduflow-awfjg8hyfua7drdy.westeurope-01.azurewebsites.net/swagger**

Log in via POST /api/Auth/login with one of the seeded accounts (see Test Credentials below), copy the returned token, and click **Authorize** in Swagger to attach it as a Bearer token.

 **Note on first request:**   The database runs on Azure SQL's serverless free tier, which auto-pauses after inactivity. The first request after a pause triggers a cold start and may take 30–60 seconds to respond. Subsequent requests are fast. If the first call seems to hang, wait and retry once.

### Test credentials

The seed migration creates three accounts. All use the password `password123`.

| Role      | Email                  |
|-----------|------------------------|
| Admin     | `admin@test.com`       |
| Professor | `professor@test.com`   |
| Student   | `student@test.com`     |

The professor owns one seeded course (*Introduction to ASP.NET Core*) containing one module, one quiz, and one assignment. The student is pre-enrolled in that course. Log in via POST /api/Auth/login, copy the returned token, and click **Authorize** in Swagger to attach it as a Bearer token to subsequent requests. ("Bearer  generated token")

## API Reference

All endpoints are prefixed with /api. Authorization is enforced at the controller-action level via JWT roles.

### Auth - /api/Auth

| Method | Route       | Roles  | Description                                |
|--------|-------------|--------|--------------------------------------------|
| POST   | /register   | Public | Register a new student.                    |
| POST   | /login      | Public | Authenticate, returns a JWT.               |

### Admin - /api/Admin

| Method | Route                  | Roles | Description                       |
|--------|------------------------|-------|-----------------------------------|
| POST   | /register-professor    | Admin | Create a professor account.       |

### Course - /api/Course

| Method | Route             | Roles      | Description           |
|--------|-------------------|------------|-----------------------|
| POST   |  /create-course   | Professor  | Create a new course.  |

### Module - /api/Module

| Method | Route          | Roles      | Description                                            |
|--------|----------------|------------|--------------------------------------------------------|
| POST   |  /{courseId}   | Professor  | Create a module under one of the caller's courses.     |
| GET    |  /{courseId}   | Student    | List modules of a course the caller is enrolled in.    |

### Quiz - /api/Quiz

| Method | Route          | Roles      | Description                                            |
|--------|----------------|------------|--------------------------------------------------------|
| POST   |  /{moduleId}   | Professor  | Create a quiz under one of the caller's modules.       |

### Assignment - /api/Assignment

| Method | Route                                | Roles      | Description                                                  |
|--------|--------------------------------------|------------|--------------------------------------------------------------|
| POST   |  /{moduleId}                         | Professor  | Create an assignment under one of the caller's modules.      |
| POST   |  /submit-assignment/{assignmentId}   | Student    | Submit an answer to an assignment in an enrolled course.     |
| PATCH  |  /{submissionId}                     | Professor  | Grade a submission on one of the caller's assignments.       |

### Enrollment -  /api/Enrollment 

| Method | Route                  | Roles     | Description                                  |
|--------|------------------------|-----------|----------------------------------------------|
| POST   |  /enroll/{courseId}    | Student   | Enroll the caller in a course.               |
| DELETE |  /unenroll/{courseId}  | Student   | Unenroll the caller from a course.           |
| GET    |  /myenrollments        | Student   | List the caller's enrollments.               |

## Project Structure
```
EduFlow/
├── EduFlow.sln
├── EduFlow/                       ← main project
│   ├── Controllers/               ← thin HTTP layer
│   ├── Services/
│   │   ├── Interfaces/
│   │   └── Implementations/       ← business rules
│   ├── Repositories/
│   │   ├── Interfaces/
│   │   └── Implementations/       ← EF Core queries
│   ├── DTOs/                      ← grouped by feature
│   │   ├── Auth/
│   │   ├── Course/
│   │   ├── Module/
│   │   ├── Quiz/
│   │   ├── Assignment/
│   │   └── Enrollment/
│   ├── Models/                    ← EF entities + ErrorResponse
│   ├── Middleware/                ← ExceptionHandlingMiddleware
│   ├── Data/                      ← AppDbContext (incl. HasData seed)
│   ├── Migrations/
│   └── Program.cs                 ← DI registration, JWT, Swagger, pipeline
└── EduFlow.Tests/                 ← test project
    └── Services/                  ← one test class per service
```
## Testing

The EduFlow.Tests project contains 34 unit tests covering every service in the application - one test per branch of every public service method (happy path plus each thrown exception). Repositories are mocked with Moq- tests verify business behavior in isolation from EF Core.

dotnet test
Or use Visual Studio's Test Explorer.

## Roadmap

Phases 1–4 are complete. The project is in Phase 5.

- ✅ **Phase 1 — Foundation.** Auth, JWT, password hashing.
- ✅ **Phase 2 — Domain model.** Entities, relationships, migrations.
- ✅ **Phase 3 — Business logic.** Role-based endpoints for the full course lifecycle.
- ✅ **Phase 4 — Quality & Standards.** DTO validation, global exception middleware, repository pattern, service layer, structured logging, full unit test coverage of services.
- 🚧 **Phase 5 — Portfolio Ready.** README, seed data, Azure deployment, CI/CD.
- ⬜ **Phase 6 — Frontend.** Angular or React client.

### Deliberately deferred

- **Quiz questions and answers.** The current Quiz model is intentionally minimal. Implementing real quizzes would require a Question/Option/Attempt/Score sub-model that's a significant feature in its own. It was deferred in favor of completing the architecture quality work first.
