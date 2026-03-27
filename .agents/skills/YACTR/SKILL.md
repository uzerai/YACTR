```markdown
# YACTR Development Patterns

> Auto-generated skill from repository analysis

## Overview

This skill teaches you how to contribute to the YACTR C# codebase using its established conventions and workflows. YACTR is a modular C# project (no framework detected) that organizes API endpoints, domain models, and infrastructure with clear separation of concerns. It employs conventional commits, named exports, and a structured approach to implementing features, database schema changes, API contract refactors, and more. This guide will help you follow the project's patterns for consistent, high-quality contributions.

## Coding Conventions

**File Naming:**  
- Use PascalCase for all file names.  
  _Example:_ `UserController.cs`, `OrderDataMapper.cs`

**Import Style:**  
- Use relative imports.  
  _Example:_  
  ```csharp
  using YACTR.Api.Endpoints.User;
  ```

**Export Style:**  
- Use named exports (explicit class/interface names).  
  _Example:_  
  ```csharp
  public class UserDataMapper { ... }
  ```

**Commit Messages:**  
- Follow [Conventional Commits](https://www.conventionalcommits.org/)  
  _Prefixes:_ `feat`, `fix`, `chore`  
  _Example:_  
  ```
  feat(user): add GetUserById endpoint with integration tests
  ```

## Workflows

### Add or Update API Endpoint
**Trigger:** When adding a new API endpoint or modifying an existing one  
**Command:** `/new-endpoint`

1. Create or update the endpoint handler in `src/YACTR.Api/Endpoints/{Entity}/`.
2. Update or create the data mapper:  
   `src/YACTR.Api/Endpoints/{Entity}/{Entity}DataMapper.cs`
3. Update or create request/response DTOs:  
   `src/YACTR.Api/Endpoints/{Entity}/`
4. Update or create the endpoint group:  
   `src/YACTR.Api/Endpoints/{Entity}/{Entity}EndpointGroup.cs`
5. Update or create integration tests:  
   `tests/YACTR.Api.Tests/EndpointTests/{Entity}EntityEndpointsIntegrationTests.cs`

_Example:_
```csharp
// src/YACTR.Api/Endpoints/User/GetUserById.cs
public class GetUserById { ... }
```

### Database Schema Change with Migration
**Trigger:** When adding, removing, or altering a database table or column  
**Command:** `/new-table`

1. Update or create the model/entity in `src/YACTR.Domain/Model/**.cs`
2. Generate and add a migration in `src/YACTR.Infrastructure/Database/Migrations/*.cs`
3. Update table configuration in `src/YACTR.Infrastructure/Database/Table/**.cs`
4. Update repository or query extensions in  
   `src/YACTR.Infrastructure/Database/Repository/**.cs` or `QueryExtensions/**.cs`
5. Update `DatabaseContextModelSnapshot.cs`
6. Update or create integration tests if needed

_Example:_
```csharp
// src/YACTR.Domain/Model/User.cs
public class User { ... }
```

### API DTO Contract Refactor
**Trigger:** When decoupling API DTOs from domain models  
**Command:** `/refactor-dtos`

1. Update endpoint request/response DTOs in `src/YACTR.Api/Endpoints/**`
2. Update data mappers to map between domain and API models
3. Update tests to check for domain model usage in DTOs (e.g., `NoDomainDTOsTest`)
4. Update integration tests to use new DTOs

_Example:_
```csharp
// src/YACTR.Api/Endpoints/User/UserResponseDto.cs
public class UserResponseDto { ... }
```

### Feature Development, Implementation, and Tests
**Trigger:** When adding a new feature or fixing a bug with test coverage  
**Command:** `/new-feature`

1. Implement or update the feature in `src/YACTR.Api/Endpoints/**` or `src/YACTR.Domain/Model/**`
2. Update or add data mappers if necessary
3. Update or add integration/unit tests in `tests/YACTR.Api.Tests/` or `tests/YACTR.Domain.Tests/`
4. Update or add documentation if needed

_Example:_
```csharp
// src/YACTR.Api/Endpoints/Product/CreateProduct.cs
public class CreateProduct { ... }
```

### Pagination Standardization
**Trigger:** When standardizing pagination across API endpoints  
**Command:** `/standardize-pagination`

1. Update listing endpoint handlers in `src/YACTR.Api/Endpoints/*/GetAll*.cs`
2. Update or create pagination utility classes in `src/YACTR.Api/Pagination/`
3. Update repository methods to return `IQueryable` or support pagination in `src/YACTR.Infrastructure/Database/Repository/**.cs`
4. Update or add integration tests for endpoints in `tests/YACTR.Api.Tests/EndpointTests/*EntityEndpointsIntegrationTests.cs`

_Example:_
```csharp
// src/YACTR.Api/Pagination/PaginationHelper.cs
public static class PaginationHelper { ... }
```

## Testing Patterns

- **Test Framework:** Unknown (likely xUnit, NUnit, or MSTest; check project references)
- **Test File Pattern:** Suffix test files with `Tests.cs`  
  _Example:_ `UserServiceTests.cs`
- **Integration Tests:**  
  Place in `tests/YACTR.Api.Tests/EndpointTests/`
- **Unit Tests:**  
  Place in `tests/YACTR.Domain.Tests/`

_Test Example:_
```csharp
// tests/YACTR.Api.Tests/EndpointTests/UserEntityEndpointsIntegrationTests.cs
public class UserEntityEndpointsIntegrationTests { ... }
```

## Commands

| Command                  | Purpose                                                        |
|--------------------------|----------------------------------------------------------------|
| /new-endpoint            | Add or update an API endpoint with implementation and tests     |
| /new-table               | Make a database schema change with migration and model updates  |
| /refactor-dtos           | Refactor API DTOs to decouple from domain models               |
| /new-feature             | Implement a new feature or enhancement with tests              |
| /standardize-pagination  | Standardize pagination logic across endpoints and repositories  |
```