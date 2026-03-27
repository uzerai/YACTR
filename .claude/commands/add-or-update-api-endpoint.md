---
name: add-or-update-api-endpoint
description: Workflow command scaffold for add-or-update-api-endpoint in YACTR.
allowed_tools: ["Bash", "Read", "Write", "Grep", "Glob"]
---

# /add-or-update-api-endpoint

Use this workflow when working on **add-or-update-api-endpoint** in `YACTR`.

## Goal

Adds or updates an API endpoint, including implementation, data mapping, and integration tests.

## Common Files

- `src/YACTR.Api/Endpoints/*/*.cs`
- `src/YACTR.Api/Endpoints/*/*DataMapper.cs`
- `src/YACTR.Api/Endpoints/*/*EndpointGroup.cs`
- `tests/YACTR.Api.Tests/EndpointTests/*EntityEndpointsIntegrationTests.cs`

## Suggested Sequence

1. Understand the current state and failure mode before editing.
2. Make the smallest coherent change that satisfies the workflow goal.
3. Run the most relevant verification for touched files.
4. Summarize what changed and what still needs review.

## Typical Commit Signals

- Create or update endpoint handler file in src/YACTR.Api/Endpoints/{Entity}/
- Update or create data mapper in src/YACTR.Api/Endpoints/{Entity}/{Entity}DataMapper.cs
- Update or create request/response DTOs in src/YACTR.Api/Endpoints/{Entity}/
- Update or create endpoint group in src/YACTR.Api/Endpoints/{Entity}/{Entity}EndpointGroup.cs
- Update or create integration tests in tests/YACTR.Api.Tests/EndpointTests/{Entity}EntityEndpointsIntegrationTests.cs

## Notes

- Treat this as a scaffold, not a hard-coded script.
- Update the command if the workflow evolves materially.