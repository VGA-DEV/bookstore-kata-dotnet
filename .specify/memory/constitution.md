<!-- Sync Impact Report
Version change: none → 1.0.0
Modified principles: none (initial constitution)
Added sections: Core Principles, API and Platform Standards, Testing and Documentation Standards, Governance
Removed sections: none
Deferred items: none
-->

# Bookstore API Constitution

## Core Principles

### I. .NET 10 Minimal APIs Only
The service MUST target `net10.0` and use ASP.NET Core Minimal APIs exclusively. Controllers, `ControllerBase`, and `[ApiController]` are prohibited. Endpoint mappings MUST be organized with route groups or endpoint modules so the HTTP surface stays modular and easy to review.

### II. Contract-First, Kebab-Case HTTP Surface
The public API MUST expose an OpenAPI 3 specification at `/openapi/v1.json`, and Scalar MUST be available in non-production environments. Routes MUST use kebab-case plural nouns and MUST avoid verbs in the URI. Every endpoint MUST have documented request and response shapes plus all relevant HTTP status codes.

### III. Async, Immutable, and Validated Handlers
All endpoint handlers MUST be asynchronous, accept a `CancellationToken`, and return typed results or `Task<IResult>`. Request and response DTOs MUST be C# `record` types. Input validation MUST use FluentValidation before business logic runs so invalid requests fail early and predictably.

### IV. Reliable Errors and Observability
Client-visible errors MUST follow RFC 7807 Problem Details and MUST NOT expose raw stack traces. The service MUST emit JSON logs with Serilog, include trace IDs in request logging, and propagate trace context downstream where applicable. Standard HTTP status codes MUST be used consistently for success and failure paths.

### V. Truthful Documentation and Tested Behavior
Documentation MUST describe actual behavior, not intended behavior. Legacy documentation MUST explicitly describe known bugs, edge cases, and other deviations from the intended design until they are fixed. The codebase MUST maintain unit and integration tests, and business logic MUST retain at least 70% line coverage.

## API and Platform Standards

- The service MUST use the .NET CLI (`dotnet`) as the only build and test entry point.
- Routes MUST remain plural and resource-oriented; nouns come from the domain, not the action.
- Central package management SHOULD be used when the repository contains multiple projects.
- Official Microsoft.Extensions packages, Serilog, and FluentValidation are the preferred foundation libraries.

## Testing and Documentation Standards

- Unit tests MUST use xUnit and mock external dependencies.
- Integration tests MUST exercise the web layer with `WebApplicationFactory<Program>`.
- Database-backed integration tests SHOULD use Testcontainers when persistence is involved.
- README.md MUST exist at the repository root and include: project title and description, prerequisites, getting started, API documentation link, testing setup, architecture/design notes, and maintainers.
- OpenAPI output, README content, and runtime behavior MUST be kept in sync.

## Governance

This constitution supersedes informal conventions and any conflicting local practices. Every pull request MUST be checked for compliance with these rules, and any exception MUST be documented before merge. Amendments require a written rationale, a version bump, and an update to the ratification metadata.

Versioning rules: MAJOR for incompatible governance changes, MINOR for added or materially expanded principles, and PATCH for clarifications or wording fixes.

**Version**: 1.0.0 | **Ratified**: 2026-10-15 | **Last Amended**: 2026-10-15
