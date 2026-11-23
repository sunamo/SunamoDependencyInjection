# SunamoDependencyInjection

## Short description

Pomocné metody pro vkládání závislostí (dependency injection) v .NET.


Helpers for DI

## Overview

SunamoDependencyInjection is part of the Sunamo package ecosystem, providing modular, platform-independent utilities for .NET development.

## Main Components

### Key Classes

- **AddServicesEndingWithResult**
- **ServiceNotFoundException**
- **ServiceNotFoundExceptionT**
- **IServiceCollectionExtensions**
- **StaticLogger**

### Key Methods

- `AddServicesEndingWithService()`
- `AddServicesEndingWith()`

## Installation

```bash
dotnet add package SunamoDependencyInjection
```

## Dependencies

- **Microsoft.Extensions.Logging.Abstractions** (v9.0.3)

## Package Information

- **Package Name**: SunamoDependencyInjection
- **Version**: 25.6.19.1
- **Target Framework**: net9.0
- **Category**: Platform-Independent NuGet Package
- **Source Files**: 5

## Related Packages

This package is part of the Sunamo package ecosystem. For more information about related packages, visit the main repository.

## License

See the repository root for license information.

<!-- sunamo:test-coverage:start -->
## Test coverage

| Metric | Value |
|---|---|
| Method | static |
| Test projects | 1 |
| Public types referenced by tests | 0/5 (0.0 %) |
| Public methods referenced by tests | 1/2 (50.0 %) |
| Line coverage | n/a |
| Branch coverage | n/a |
| Generated (UTC) | 2026-11-22 21:24 |

<!-- sunamo:test-coverage:data {"package":"SunamoDependencyInjection","container":"pinp","testProjects":1,"publicTypes":5,"typesReferencedByTests":0,"publicMethods":2,"methodsReferencedByTests":1,"lineRate":null,"branchRate":null,"method":"static","generatedUtc":"2026-11-22 21:24"} -->
<!-- sunamo:test-coverage:end -->
