# Bookstore

Basic app using Java 11 and MongoDB.

It requires a bookstore database to be created in MongoDB.

This API manages book inventory in a bookstore. 
List books, create new ones, update, and delete are some of the supported operations.

## Optional: Use docker compose to run the application

```bash
docker-compose up
``` 

# Harness Architecture

## Tools

| Tool | Why | Install (reference) |
|------|-----|---------------------|
| **GitHub MCP** | Fetch issues (included in `.mcp.json`) | Uses Copilot GitHub auth — `https://api.githubcopilot.com/mcp/` with `X-MCP-Toolsets: issues` |
| **CodeGraph** | Code navigation (base) | `npm i -g @colbymchenry/codegraph` → `codegraph install` → in repo `codegraph init` |
| **RTK** | Compress shell/test noise | `brew install rtk-ai/tap/rtk` (or Cargo from `rtk-ai/rtk`) → `rtk gain` to verify → `rtk init -g --copilot` |
| **Caveman** | Chat prose lite only | `npx skills add JuliusBrussee/caveman -g` → use `/caveman lite` in chat |
| **Graphify** (optional) | Project/docs/schema map | Install per [Graphify](https://github.com/safishamsi/graphify), then add the MCP block below |
| **Spec Kit** (optional) | Spec-driven delivery | [github/spec-kit](https://github.com/github/spec-kit) `specify init` for Copilot |
| **grill-with-docs** (optional) | ADRs / CONTEXT alignment | From [mattpocock/skills](https://github.com/mattpocock/skills) |

### Optional Graphify MCP

Add only if Graphify is installed. Portable `.mcp.json`:

```json
"graphify": {
  "type": "stdio",
  "command": "graphify",
  "args": [".", "--mcp"]
}
```

VS Code `.vscode/mcp.json` uses the same server entry under `"servers"`.

## How Harness is configured

- **GitHub MCP** to load an issue before work  
- **CodeGraph** for code structure  
- **Graphify** for project/docs (if present)  
- **RTK** for noisy shell  
- **Caveman lite** for chat only — never for SPEC/ADR/PR text  
- **Spec Kit** alone as the delivery spine if you use one  

No phase profiles. No parallel Superpowers + Spec Kit.

## Bookstore API (.NET 10 scaffold)

### Project Title & Description

Bookstore API is being scaffolded as a .NET 10 Minimal API for bookstore operations. The new solution lives alongside the existing Java code and currently exposes only a health endpoint under `/api/v1/books`.

### Prerequisites

- .NET 10 SDK
- Docker for integration tests that use MongoDB Testcontainers

### Getting Started

```powershell
dotnet restore Bookstore.sln
dotnet build Bookstore.sln
dotnet run --project src/Bookstore.Api
```

### API Documentation Link

- OpenAPI JSON: `http://localhost:<port>/openapi/v1.json`
- Scalar UI: available in Development at the default Scalar route

### Testing Setup

```powershell
dotnet test Bookstore.sln
dotnet test tests/Bookstore.Api.UnitTests/Bookstore.Api.UnitTests.csproj /p:CollectCoverage=true /p:Threshold=70 /p:ThresholdType=line /p:ThresholdStat=total
```

The second command fails the build if total line coverage drops below 70%.

### Architecture / Design Notes

- Central package management is used at the repository root.
- The .NET API uses Minimal APIs only; controllers are intentionally absent.
- MongoDB configuration is read from `MONGODB_URI` or configuration and is never hardcoded.

### Maintainers

See the repository owners / team channel for the Java service and the new Bookstore API initiative.
