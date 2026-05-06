From repository: Architecture - Clean Architecture + DDD + CQRS + Vertical Slice.   
From repository: Domain → no dependencies; Application → depends only on Domain + SharedKernel; Infrastructure → implements Application abstractions.

Architecture
Goal: Keep code aligned with Clean Architecture, DDD, CQRS, Vertical Slice.

Layers: Domain, Application, Infrastructure, Web.Api, UI, SharedKernel.

Principle: Business logic in Domain only; infrastructure and frameworks live in Infrastructure/Web.Api/UI.

Dependency Rules
Allowed:

Application → Domain, SharedKernel

Infrastructure → Application abstractions

Web.Api → Application

UI → Web.Api

Forbidden:

Domain or Application must not reference Infrastructure.

Conventions and Naming
DDD: Entities, Value Objects, Domain Events in Domain.

CQRS: Commands and Queries under Application/{Feature}/{Action} with Command, CommandHandler, CommandValidator or Query, QueryHandler, Response.

Handlers: Constructor injection only; single responsibility; no domain mutation or direct framework usage.

Naming: CreateXCommand, XCommandHandler, GetXQuery, XQueryHandler.

Rules and Patterns
Result pattern: Return Result / Result<T>; do not throw for business logic.

Validation: Every command must have a validator; use pipeline decorators (ValidationDecorator, LoggingDecorator).

Domain events: Raised inside entities; handlers in Application; dispatcher in Infrastructure.

No repository pattern: Use DbContext via abstractions only.

Testing: L0 unit tests for handlers/validators; L1 API tests; ArchitectureTests enforce layer rules.

Code Generation Modes
Planning Mode: When prompt contains "plan", "design", "approach" — produce architecture and skeletons only.

Code Generation Mode: When prompt contains "generate", "implement", "create" — produce full code following naming and commenting rules.

Comments: Minimal; explain why when non‑obvious.

Quick Usage Notes
Keep methods small and focused.

Prefer composition over inheritance.

Reuse existing patterns; avoid new abstractions unless necessary.

Follow folder structure and naming strictly.