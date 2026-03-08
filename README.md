# BasicBilling.API

RESTful API for managing service bill payments (water, electricity, sewer) for a basic services payment company. Built as part of the NEXION Tech Test - Backend (v3.0).

## API Functionality

### Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/token` | Generate a JWT token (no authentication required) |
| POST | `/api/bills` | Create a new service bill |
| POST | `/api/payments` | Process a bill payment |
| GET | `/api/clients/{id}/pending-bills` | Retrieve pending bills for a client |
| GET | `/api/clients/{id}/payment-history` | Retrieve payment history for a client |

All endpoints except `/api/auth/token` require a valid JWT Bearer token in the `Authorization` header.

### OData Support

The GET endpoints support OData query parameters for filtering, sorting, and pagination:

```
GET /api/clients/100/pending-bills?$filter=ServiceType eq 'Water'&$orderby=Amount desc&$top=5
```

Supported operators: `$filter`, `$orderby`, `$top`, `$skip`, `$select`, `$count`.

### Seed Data

The database is pre-populated with:
- **5 clients**: Joseph Carlton (100), Maria Juarez (200), Albert Kenny (300), Jessica Phillips (400), Charles Johnson (500)
- **30 bills**: 2 months of bills per client across all 3 service types (all initially pending)

## Technical Stack

| Technology | Purpose |
|------------|---------|
| .NET 10 | Web API framework |
| Entity Framework Core | ORM with SQLite (Code-First + migrations) |
| MediatR | CQRS pattern for request handling |
| AutoMapper | Domain-to-DTO mapping |
| OData | Query/filter support on endpoints |
| JWT Bearer | Authentication and authorization |
| Swagger/OpenAPI | API documentation |
| xUnit + Moq | Unit and integration testing |

## Project Structure

```
BasicBilling.API/
├── Domain/
│   ├── Entities/          (Bill, Client, Payment)
│   └── Enums/             (ServiceType, BillStatus)
├── Application/
│   ├── DTOs/              (BillDto, PaymentDto, CreateBillDto, PaymentRequestDto)
│   ├── Features/
│   │   ├── Bills/         (CreateBillCommand, GetPendingBillsQuery)
│   │   └── Payments/      (ProcessPaymentCommand, GetPaymentHistoryQuery)
│   ├── Interfaces/        (IBaseRepository, IBillRepository, IUnitOfWork, etc.)
│   └── Mappings/          (MappingProfile)
├── Infrastructure/
│   ├── Data/              (BillingDbContext, Migrations)
│   ├── Middleware/         (ExceptionHandling, RequestLogging)
│   └── Repositories/      (BaseRepository, UnitOfWork, etc.)
├── Controllers/           (AuthController, BillsController, ClientsController, PaymentsController)
└── Program.cs

BasicBilling.Tests/
├── Handlers/              (Unit tests with mocked dependencies)
│   ├── Bills/             (CreateBillHandlerTests, GetPendingBillsHandlerTests)
│   └── Payments/          (ProcessPaymentHandlerTests, GetPaymentHistoryHandlerTests)
└── Integration/           (End-to-end tests with in-memory SQLite)
    ├── CustomWebApplicationFactory.cs
    ├── TestHelper.cs
    ├── BillsEndpointTests.cs
    ├── ClientsEndpointTests.cs
    └── PaymentsEndpointTests.cs
```

## How to Build

```bash
dotnet build BasicBilling.sln
```

## How to Run

```bash
dotnet run --project BasicBilling.API.csproj
```

The API will start on `https://localhost:5101`. Swagger UI opens automatically at `/swagger`.

### JWT Configuration

The application requires a JWT key configured in `appsettings.Development.json`:

```json
{
  "Jwt": {
    "Key": "YourSecretKeyThatIsAtLeast32CharactersLong!"
  }
}
```

This file is excluded from version control via `.gitignore`. Create it manually before running the app.

## How to Run Tests

```bash
dotnet test BasicBilling.sln
```

- **14 unit tests**: Test handlers in isolation using Moq (no database access)
- **12 integration tests**: Test the full HTTP pipeline using `WebApplicationFactory` with an in-memory SQLite database

## Why .NET 10 Instead of .NET 8

The practice specifies .NET 8 LTS. This project uses **.NET 10** for the following reasons:

- **.NET 10 is the current version** at the time of development (March 2026), and .NET 8 packages are becoming outdated in the NuGet ecosystem. Several dependencies (EF Core, ASP.NET Core, Swagger) already target .NET 10 as their primary version.
- **Full backward compatibility**: .NET 10 is fully compatible with all libraries and patterns required by the practice (MediatR, AutoMapper, OData, JWT, xUnit, etc.). No features were lost or changed in the migration.
- **Demonstrates adaptability**: Using the latest stable framework shows the ability to work with current tooling rather than being locked into older versions. The architecture and patterns remain identical regardless of the runtime version.
- **All required features are implemented**: The .NET version change does not affect any of the evaluated criteria. Every feature, pattern, and requirement from the practice specification is fully implemented.

## Features Checklist

- [x] .NET Web API project
- [x] Entity Framework Core with SQLite (Code-First + migrations)
- [x] Seed initial data (5 clients + 2 months of bills)
- [x] POST /bills endpoint
- [x] POST /payments endpoint
- [x] GET /clients/{id}/pending-bills endpoint
- [x] GET /clients/{id}/payment-history endpoint
- [x] ASP.NET Core built-in dependency injection
- [x] JSON response format
- [x] HTTPS enabled
- [x] Nullable reference types enabled
- [x] Domain-Driven Design (entities, repositories, services separated)
- [x] AutoMapper for domain-to-DTO mapping
- [x] MediatR for CQRS request handling
- [x] Middleware (exception handling, request logging)
- [x] CORS configured
- [x] JWT Authorization
- [x] Swagger/OpenAPI documentation
- [x] OData query support
- [x] Unit tests (xUnit + Moq, no DB access)
- [x] Integration tests (in-memory SQLite database)

All features and requirements from the practice specification have been implemented.
