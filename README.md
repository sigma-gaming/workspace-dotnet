# Sigma Games — .NET backend

Backend of **Sigma Games**, a web gaming platform with dice and pincode games,
player balances, payments, a referral program, promo codes and real-time chat.

Written in C# on .NET 9 between January and May 2025. It replaced the earlier
TypeScript backend that lived in [`workspace`](https://github.com/sigma-gaming/workspace)
until March 2025; the web applications in that repository consume the APIs
defined here through generated OpenAPI clients.

> **Status:** archived. The project is no longer maintained and its production
> infrastructure has been shut down. Local configuration and credentials were
> removed from the git history before the repository was made public; replaced
> values appear as `SECRET_REMOVED`.

## Architecture

One solution, one shared domain layer, deployed as several independent services.
Each service is a thin ASP.NET Core host that wires the pieces it needs from
`Core.Services` and `Common.*`.

| Service | Responsibility |
|---|---|
| `Core.API` | Main player-facing HTTP API: games and game history, user profile, payments, affiliate program, promo codes, chat, notifications, tasks, settings |
| `Core.WS` | Real-time updates over SignalR: game history, chat and system broadcasts fed from NATS |
| `Core.Tasks` | Background jobs on Hangfire: currency rates, stale deposits and withdrawals, referral payouts, budget sync, game metrics; consumer of payment webhooks from RabbitMQ |
| `LetsAuth.API` | Sign-in through Telegram and VK, issuing one-time codes for apps |
| `Access.API` | Session exchange, refresh and logout |
| `Payment.API` | Webhook receivers for payment providers: verifies the signature and queues the webhook through an outbox |
| `Affiliate.API` | Referral campaign tracking |
| `Control.API` | Admin API: users, promo codes, notifications, maintenance mode |
| `Domain.API` | Registry of the current application domains |
| `Redirect.API` | Short-link redirects (`/r/{code}`) |
| `Core.Migrations` | Applies EF Core migrations |

```
src/
  Apps/            deployable services (table above)
  Domain/
    Core.Contracts   entities, enums, DTOs, mappers
    Core.Services    domain logic: Auth, Gaming, Payment, Finance, Promotions, Chat, Notifications, …
    Core.Services.Tests
  Infrastructure/
    Core.Db          EF Core context and configurations (PostgreSQL)
    Core.Cache       typed cache registry on top of Redis
    Core.Nats        NATS connection and subjects
    Core.RabbitMq    RabbitMQ connection, queues and health check
    Core.Builder     service registration entry point
    Core.Tests.Fixtures  Testcontainers fixtures for PostgreSQL, Redis, NATS and RabbitMQ
  Common/          reusable building blocks: Api (rate limiting, health checks),
                   Caching (typed cache entities, distributed locks), Db,
                   Monitoring (logging, metrics, health checks), Nats, OpenApi,
                   PubSub, RabbitMq, Validation
```

Some design points worth looking at:

- **Game runners** — `Core.Services/Gaming/Runners` keeps each game (`DiceGame`,
  `PincodeGame`) behind a common `BaseGame`, with the outcome calculation in
  separate calculator classes.
- **Payments** — `Core.Services/Payment` separates provider clients, webhook
  handlers and status mappers behind `PaymentProviderFactory`; deposit and
  withdrawal methods are configured per provider rather than hard-coded.
- **One-time sign-in codes** — `LetsAuth.API` authenticates through Telegram or VK
  and hands the app a one-time code, which `Access.API` exchanges for a session.
- **Pub/sub** — services publish domain events to NATS; `Core.WS` fans them out to
  connected clients through SignalR hubs.
- **Webhook queue** — `Payment.API` checks the provider signature, writes the webhook
  to an outbox table and answers at once; a background publisher sends it to a RabbitMQ
  quorum queue with publisher confirms. `Core.Tasks` processes it with manual acks:
  a failed message waits in a TTL retry queue and comes back, after the last attempt
  it is parked in a dead-letter queue. Confirming a deposit locks its row, so a repeated
  webhook never credits the balance twice. NATS stays for real-time fan-out, RabbitMQ
  carries work that must not be lost.
- **Integration tests** — payment flows (deposits, withdrawals, static wallets, the
  webhook queue) are tested against real PostgreSQL, Redis, NATS and RabbitMQ
  containers started by `Core.Tests.Fixtures` (see its [README](src/Infrastructure/Core.Tests.Fixtures/README.md)).

## Tech stack

- .NET 9, ASP.NET Core (slim builder), SignalR
- PostgreSQL via EF Core + Npgsql, PgBouncer
- Dragonfly (Redis-compatible) via StackExchange.Redis, RedLock.net
- NATS for pub/sub between services
- RabbitMQ (RabbitMQ.Client) for payment webhook processing with retries and a dead-letter queue
- Hangfire for scheduled jobs
- Serilog, prometheus-net, health checks
- OpenAPI with Swashbuckle and Scalar, Riok.Mapperly for mapping
- xUnit, Moq, Testcontainers, Respawn
- Docker; GitHub Actions building only affected services

## Running locally

Requirements: .NET 9 SDK, Docker, [mkcert](https://github.com/FiloSottile/mkcert).

```bash
# infrastructure: PostgreSQL, PgBouncer, Dragonfly, NATS, RabbitMQ
docker compose up -d postgres pgbouncer dragonfly nats rabbitmq

# local HTTPS certificates (see certs/README.md)
mkcert -key-file certs/local.key -cert-file certs/local.crt localhost sigma.local '*.sigma.local'

# configuration: every app has appsettings.Development.json.example
cp src/Apps/Core.Migrations/appsettings.Development.json.example \
   src/Apps/Core.Migrations/appsettings.Development.json
cp src/Apps/Core.API/appsettings.Development.json.example \
   src/Apps/Core.API/appsettings.Development.json
# …fill in connection settings, then:

dotnet run --project src/Apps/Core.Migrations
dotnet run --project src/Apps/Core.API
```

```bash
dotnet build Sigma.sln
dotnet test Sigma.sln     # needs Docker for Testcontainers
```

## Related repositories

- [`workspace`](https://github.com/sigma-gaming/workspace) — web apps, admin panel and Telegram bot (TypeScript)
- [`k8s`](https://github.com/sigma-gaming/k8s) — Kubernetes manifests delivered with Flux CD
- [`infra`](https://github.com/sigma-gaming/infra) — cluster and cloud resources in Pulumi

## Author

Evgenii Zakharov ([@risenxxx](https://github.com/risenxxx))
