[![Build Status](https://dev.azure.com/chadjiantoniou/CleanArchitecture/_apis/build/status/thecodewrapper.CH.CleanArchitecture?branchName=develop)](https://dev.azure.com/chadjiantoniou/CleanArchitecture/_build/latest?definitionId=2&branchName=develop)

# Clean Architecture
My personal take on Clean Architecture. Please read my post at [Implementing a Clean Architecture in ASP.NET Core 8](https://thecodewrapper.com/dev/tcw-clean-achitecture) for more information.

If you are looking for the Blazor version of this Clean Architecture, you can find it [here](https://github.com/thecodewrapper/CH.CleanArchitectureBlazor)

> ⚠️ **Warning:** This repository will no longer be maintained/updated. Use the Blazor version from the link above which is 100% maintained.

## Technologies used
- ASP.NET Core 8
- Entity Framework Core 8
- MassTransit
- AutoMapper
- Razor Components
- ASP.NET Core MVC
- GuardClauses
- xUnit
- Moq
- Fluent Assertions
- FakeItEasy
- Docker

## Features
The features of this particular solution are summarized briefly below, in no particular order:

- Localization for multiple language support
- Event sourcing using Entity Framework Core and SQL Server as persistent storage, including snapshots and retroactive events
- EventStore repository and DataEntity generic repository. Persistence can be swapped between them, fine-grained to individual entities
- Persistent application configurations with optional encryption
- Data operation auditing built-in (for entities which are not using the EventStore)
- Local user management with ASP.NET Core Identity
- Clean separation of data entities and domain objects and mapping between them for persistence/retrieval using AutoMapper
- ASP.NET Core MVC with Razor Components used for presentation
- CQRS using handler abstractions to support MassTransit or MediatR with very little change
- Service bus abstractions to support message-broker solutions like MassTransit or MediatR (default implementation uses MassTransit’s mediator)
- Unforcefully promoting Domain-Driven Design with aggregates, entities and domain event abstractions.
- Lightweight authorization framework using ASP.NET Core AuthorizationHandler
- Docker containerization support for SQL Server and Web app

### Some other goodies:
- Password generator implementation based on ASP.NET Core Identity password configuration
- Razor Class Library containing ready-made Blazor components for commonly used features such as CRUD buttons, toast functionality, modal components, Blazor Select2, DataTables integration and page loader
- Common library with various type extensions, result wrapper objects, paged result data structures, date format converter and more.
