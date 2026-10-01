Company .NET API Standards

1. Supported .NET Versions & Tooling

To ensure security patches, performance, and ecosystem compatibility, all services strictly adhere to unified runtime requirements.

Mandatory Version: .NET 10 (net10.0) is the default and only valid target framework for all services.

Build Tools: The .NET CLI (dotnet) is the sole permitted build tool across all local and CI/CD environments.

2. API Design & Architecture (Minimal APIs)

We enforce ASP.NET Core Minimal APIs for all HTTP service implementations. Controller-based architectures (ControllerBase / [ApiController]) are prohibited for new services.

Minimal API Pattern: Endpoints must be mapped using ASP.NET Core Minimal API routing methods (app.MapGet(), app.MapPost(), app.MapPut(), app.MapDelete()).

Route Modularization: Routes must be organized logically into route groups using RouteGroupBuilder or extension modules to keep code modular.

Asynchronous Handlers: Endpoint handlers must be asynchronous (async Task<IResult>) and accept a CancellationToken parameter to handle request aborts.

Immutability: Request and response DTOs must be implemented using C# record types with primary constructors.

3. API Specification (OpenAPI)

We follow a design-first or strictly documented code-first approach to API development.

Requirement: Every API must expose an OpenAPI 3.0+ specification.

Accessibility: The spec must be accessible via a standard endpoint (e.g., /openapi/v1.json) and include a visual UI (e.g., Scalar or Swagger UI) in non-production environments.

Completeness: The specification must include comprehensive descriptions, request/response payloads, authentication requirements, and all possible HTTP status codes for every endpoint.

4. Naming Conventions

Consistency in naming reduces cognitive load for developers and API consumers.

REST Endpoints (URIs)

Use kebab-case for all URIs (e.g., /user-profiles, not /userProfiles).

Use plural nouns for resources (e.g., /api/v1/users, not /api/v1/user).

Avoid using verbs in the URI; use HTTP methods instead (e.g., POST /users, not POST /create-user).

.NET Codebase

Namespaces: Strictly PascalCase domain hierarchy, using file-scoped namespaces (e.g., namespace Company.Service.Users;).

Classes/Interfaces: PascalCase, with interfaces prefixed with I (e.g., IUserService, PaymentService).

Methods/Properties: PascalCase (e.g., GetUserByIdAsync, CustomerName). Asynchronous methods must end with Async.

Variables/Parameters: camelCase (e.g., getUserById, customerName).

Private Fields: _camelCase with a leading underscore (e.g., _userService, _logger).

Constants: UPPER_SNAKE_CASE or PascalCase (e.g., MAX_RETRY_COUNT).

5. Project Setup & Package Best Practices

Project Configuration

Every .csproj file must enforce strict quality and compiler guards:

<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>


Nullable Reference Types: Enabled (<Nullable>enable</Nullable>) to prevent null reference issues.

Package Management

Central Package Management (CPM): Multi-project repositories must utilize a root Directory.Packages.props file to manage package versions centrally.

Standard Dependencies: Use official Microsoft.Extensions.* packages, Entity Framework Core for ORM needs, Serilog for structured logging, FluentValidation for validation, and xUnit for testing.

6. README Guidelines

Every repository must contain a README.md at the root as the front door to your service. It must include the following sections:

Project Title & Description: What the service does and its domain context.

Prerequisites: Required software (e.g., .NET 10 SDK, Docker, specific environment variables).

Getting Started: Step-by-step instructions to build and run the service locally (dotnet run).

API Documentation Link: Where to find the local and staging OpenAPI/Swagger UI.

Testing Setup: How to execute unit and integration tests (dotnet test).

Architecture/Design Notes: Brief overview of key design decisions, dependencies (e.g., Postgres, Redis), and downstream systems.

Maintainers: Code owners or Slack channel for support.

7. Error Handling

APIs must never return raw stack traces to the client.

Standard Format: All errors must follow RFC 7807 (Problem Details for HTTP APIs). Use builder.Services.AddProblemDetails().

Structure: Error responses must include a type, title, status, detail, and instance (trace ID).

HTTP Status Codes: Strictly adhere to standard HTTP codes (e.g., 200 for OK, 400 for Bad Request, 401 for Unauthorized, 403 for Forbidden, 404 for Not Found, 500 for Internal Server Error). Leverage TypedResults helpers for endpoint responses.

8. Logging and Observability

Format: Logs must be output in JSON format to ensure compatibility with log aggregation tools (e.g., ELK, Datadog).

Context: Every request must generate a unique Trace ID that is injected into all log entries and passed to downstream services via headers.

Levels: Use ERROR for system failures requiring intervention, WARN for handled exceptions/retries, INFO for lifecycle events, and DEBUG for troubleshooting (disabled in production).

9. Testing Requirements

Minimum Coverage: All APIs require a minimum of 70% line coverage for business logic.

Test Types: Projects must include both Unit Tests (mocked dependencies) and Integration Tests (testing the web layer using WebApplicationFactory<Program> and database integrations, ideally using Testcontainers).