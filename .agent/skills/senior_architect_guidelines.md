---
name: Senior Architect Guidelines
description: A set of strict development rules to ensure enterprise-grade quality, performance, and maintainability.
---

# Senior Architect Guidelines

These rules must be followed in every step of the development process.

## General Mindset
- **Experience**: Think like a senior software architect with 10+ years of experience.
- **Scale**: Write code as if it will handle millions of requests; consider performance and impact.
- **Naming**: Never use Turkish characters in folder/class/property names.
- **Naming Convention**: Use enterprise naming postfixes: `*Entity`, `*Dto`, `*Service`, `*Repository`, `*Middleware`, `*OptionsEntity`, `*Command`, `*Handler`, `*Projection`.

## Technical Requirements
- **Database**: Prefer an ORM (Entity Framework Core) for database access.
- **Controllers**: Controllers must **never** directly access repositories or the database. They should act as thin layers (e.g., using MediatR).
- **Async/Await**: Prefer async for I/O; **never** block async flows with sync calls (e.g., `.Result`, `.Wait()`).
- **Dependencies**: Ensure there are no circular dependencies between projects.
- **Package Management**: Enforce central package management via `Directory.Packages.props` and `Directory.Build.props`.

## Code Quality & Patterns
- **SOLID/SRP**: Follow SOLID principles. Do not create god classes; split long methods into well-named private methods.
- **DTOs**: Application layer methods must **not** accept Entities as parameters; use DTOs.
- **Complex Logic**: If a method contains 3+ if-statements, implement a rule-engine pattern (`IRule` + engine); add tests for every rule.
- **Magic Strings**: Avoid magic strings; place constants in `Orchestify.Shared`.
- **Idempotency**: Ensure idempotency: never send the same provider request twice; never bill twice.

## Documentation & Verification
- **Build & Test**: On every commit, ensure the solution builds; run tests when impacted.
- **Comments**: Add detailed `///summary` comments (NOT XML documentation files; keep summaries clear and professional).
- **Documentation**: Update `README.md` and `architecture.md` when changes require it.
- **Structure**: Keep everything under `/src`, `/tests`, `/scripts`, `/infra`.

## Testing
- **Coverage**: Add unit tests and integration tests for all services/repositories.
