# AcxiomCRM backend starter

This is the ASP.NET Core Web API backend for the AcxiomCRM frontend prototype. It uses C#, ASP.NET Core Identity, Entity Framework Core and SQLite. The code is intentionally organized into Controllers, Models, Data, DTOs and Services so it is easier to explain during a project review.

## Requirements on your Mac

Install the .NET 8 SDK and Visual Studio Code. Check installation in Terminal:

```bash
dotnet --version
```

## Run the backend

1. Extract this folder.
2. In VS Code choose File, Open Folder, and open `AcxiomCRM-Backend`.
3. Open Terminal, New Terminal.
4. Run:

```bash
dotnet restore
dotnet run
```

The terminal will show the local address. In Development, open `/swagger` on that address to explore the API. The health check is `/api/health`. The SQLite database file `acxiomcrm.db` is created automatically.

## Main endpoints

| Method | Endpoint | Purpose |
| GET | `/api/health` | Health check |
| POST | `/api/auth/register` | Create an account |
| POST | `/api/auth/login` | Sign in and create an authentication cookie |
| POST | `/api/auth/logout` | Sign out |
| GET | `/api/auth/me` | Current signed in user |
| GET, POST | `/api/customers` | List and create customers |
| GET, PUT, DELETE | `/api/customers/{id}` | Read, update or deactivate a customer |
| GET, POST | `/api/leads` | List and create leads |
| GET, POST | `/api/opportunities` | List and create opportunities |
| GET, POST | `/api/followups` | List and create follow ups |
| GET, POST | `/api/activities` | List and create activities |
| GET | `/api/dashboard` | KPI and chart data |
| GET | `/api/reports/pipeline` | Manager and Admin pipeline report |
| GET | `/api/audit` | Admin audit log |
| GET | `/api/users` | Admin user listing |

## Important notes

The backend uses Identity password hashing, a password policy, failed login lockout, role names, authorization attributes, server side model validation, unique customer email and phone indexes, audit records, and business validation for opportunity and follow up dates. Customer deletion is implemented as deactivation.

For a simple class demo, the first registered account may select Admin. After that, public registration should not be used to grant privileged roles. A real deployment should have an administrator create users and assign roles through a protected workflow. Do not deploy this development configuration directly to production.

The frontend prototype currently reads and writes browser localStorage. It will not automatically use this backend until its JavaScript is changed to call these API endpoints with `fetch`, and the login flow is connected. The CORS policy currently allows the common Live Server addresses on port 5500. If Live Server uses another port, update `Program.cs` accordingly. Since authentication uses cookies, frontend requests must use `credentials: 'include'`. State changing requests should also be protected against CSRF before production deployment.

This is a starter implementation, not a claim that every acceptance criterion in the supplied assignment has been fully completed. Before submission, test each role, add secure Admin user provisioning, add antiforgery protection for cookie authenticated state changes, implement the remaining UI integration, and verify every requirement from the assignment checklist.
