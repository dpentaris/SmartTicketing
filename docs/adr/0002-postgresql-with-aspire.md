# 2. PostgreSQL, orchestrated locally with .NET Aspire

Date: 2026-09-30
Status: Accepted

## Context

The app needs a relational database with strong transactional guarantees
(seat reservations must never oversell). It should run locally with minimal
setup and be deployable for free for a public demo.

## Decision

- Use **PostgreSQL** via EF Core (Npgsql provider).
- Use a **.NET Aspire AppHost** for local development: it starts PostgreSQL (and
  pgAdmin) in Docker, wires the connection string into the API and provides a
  dashboard with logs, traces and metrics through OpenTelemetry.
- Use **Testcontainers** in integration tests so they run against a real
  PostgreSQL, not an in-memory fake.

## Consequences

- Local setup is: install .NET SDK + Docker, run the AppHost.
- Tests catch SQL translation issues, constraint violations and migration errors.
- Free managed PostgreSQL offerings (e.g. Neon) can host the demo database.
