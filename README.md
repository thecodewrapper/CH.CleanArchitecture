My personal take on a Clean Architecture. Can be used as a starting point for several types of projects.

Features:
- Localization for multiple language support
- Event sourcing using EF Core and SQL Server as persistent storage, including snapshots and retroactive events
- EventStore repository and DataEntity generic repository. Persistence can be mixed or swapped between them, fine-grained to individual entities
- Persistent application configurations
- Data operation auditing built-in (for entities which are not using the EventStore)
- Local user management with ASP.NET Core Identity
- Clean separation of data entities and domain objects and mapping between them for persistence/retrieval using AutoMapper
- ASP.NET Core MVC with Razor Components used for presentation
- CQRS using handler abstractions to support MassTransit or MediatR with very little change
- Service bus abstractions to support message-broker solutions like MassTransit or MediatR (default implementation uses MassTransit’s mediator)
- Unforcefully promotes Domain-Driven Design with aggregates, entities and domain event abstractions.
- Lightweight authorization framework using ASP.NET Core's AuthorizationHandler
- Docker containerization support

Others:
- Password generator implementation based on ASP.NET Core Identity password configuration
- RCL containing ready-made Blazor components for commonly used features such as CRUD buttons, toast functionality, modal components, Blazor Select2, DataTables integration and page loader
- Common library with various type extensions, result wrapper objects, paged result data structures, date format converter and more
