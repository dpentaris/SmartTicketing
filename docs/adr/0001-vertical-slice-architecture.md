# 1. Vertical slice architecture in a single API project

Date: 2026-09-30
Status: Accepted

## Context

The system is small (a handful of features: timetable, booking, payment, ticket
validation) and built by one developer. A classic Clean Architecture split into
`Domain` / `Application` / `Infrastructure` / `Web` projects adds a lot of
ceremony (interfaces, mappers, cross-project references) for every feature.

## Decision

Use a single `FerryBooking.Api` project organised by feature:

- `Domain/` holds entities and business rules. Entities protect their invariants
  through factory methods (e.g. `Sailing.Schedule`) and throw `DomainException`.
- `Features/<Feature>/` holds one file per use case (request, handler, endpoint
  mapping), talking to `FerryDbContext` directly.
- `Infrastructure/` holds EF Core configuration, migrations and seed data.

`DbContext` is used directly instead of repositories: it already is a unit of work
and a repository, and wrapping it hides useful EF Core features.

## Consequences

- Adding a feature touches one folder; related code is easy to find.
- Business rules stay testable without a database (see `FerryBooking.UnitTests`).
- If the system grows, slices can be extracted into separate projects later.
