# Gym Inventory / Warehouse Management System

A backend-focused inventory and warehouse management system for a gym.

## Tech Stack

- **Backend:** C# / ASP.NET Core
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Auth:** JWT Bearer authentication, role-based authorization
- **Frontend:** React *(planned)*

## Status

🚧 Work in progress — authentication and authorization are in place; validation hardening and further testing are ongoing.

## What's implemented so far

- Domain model:
  - `Item` (abstract base) → `Equipment`, `Consumable` (TPT inheritance)
  - `Person` (abstract base) → `User`
  - `Category`, `Location`
  - `StockLevel` — current quantity snapshot, per item/location
  - `StockMovement` — append-only history of stock changes (received, used, adjusted, damaged)
  - `StockTransfer` — stock moved between two locations
- `AppDbContext` with EF Core configuration: relationships, TPT mapping, enum-to-string conversions
- Initial database migration applied
- Database seeder with sample data (10 rows across core tables) for local testing, including hashed passwords for seeded users
- CRUD API endpoints (Controllers + Services + DTOs + Mappers) for:
  - Categories, Locations, Consumables, Equipment
  - Stock Movements — recording a movement updates the matching `StockLevel` transactionally
  - Stock Transfers — moving stock between two locations, updating both `StockLevel` rows atomically, with an insufficient-stock check
  - Read endpoints for current stock levels (by item, by location, by item+location) and low-stock consumables
- Shared `StockLevelHelperService` (get-or-create pattern) used by both Stock Movements and Stock Transfers
- Global exception-handling middleware: custom exceptions (`NotFoundException`, `InvalidOperationBusinessException`) are caught centrally and mapped to consistent HTTP responses (404 / 400), instead of scattered null-checks
- **Authentication & Authorization:**
  - Password hashing via ASP.NET Core's `PasswordHasher<User>` (no plaintext passwords stored)
  - JWT issuance on register/login (`POST /api/auth/register`, `POST /api/auth/login`), signed with a secret key stored in User Secrets (never committed)
  - `[Authorize]` applied across all controllers; write operations (create/update/delete) on Categories, Locations, Equipment, and Consumables are restricted to the `Admin` role; Stock Movements/Transfers are open to any authenticated user
  - Swagger configured with Bearer token support for testing protected endpoints
- Enums serialized as strings over the API (not raw integers), matching how they're stored in the database

## Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (LocalDB or Express)
- `dotnet-ef` CLI tool (`dotnet tool install --global dotnet-ef`)

### Setup
```bash
# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update

# Set the JWT signing key (development secret, not committed to source control)
dotnet user-secrets set "Jwt:Key" "<a long random string, 32+ characters>"

# Run the app
dotnet run
```

On first run, the app seeds sample data automatically (categories, locations, items, users, stock levels, movements, and transfers) if the database is empty. Seeded users authenticate with the password `Password123!`.

### Configuration
- Update the `DefaultConnection` string in `appsettings.json` to point to your local SQL Server instance.
- `Jwt:Issuer`, `Jwt:Audience`, and `Jwt:ExpiryHours` are set in `appsettings.json`. `Jwt:Key` is kept out of source control and must be set via `dotnet user-secrets` (see Setup above).

### Testing protected endpoints in Swagger
1. Call `POST /api/auth/login` with a valid username/password and copy the returned `token`.
2. Click **Authorize** in the Swagger UI and enter `Bearer <token>`.
3. Protected endpoints will now include the token automatically.

## Design Notes

Some of the key modeling decisions behind this project:

- **Equipment vs. Consumable** are modeled as separate subtypes (not a boolean flag) via EF Core's Table-Per-Type (TPT) inheritance strategy, since they need genuinely different fields.
- **StockLevel vs. StockMovement**: `StockMovement` is the append-only source of truth (what happened, when, by whom); `StockLevel` is a cached current-quantity snapshot kept in sync with it. This mirrors how ledger/accounting systems track state.
- **Multi-location support** is built in from the start — every stock quantity and movement is tied to a specific `Location`.
- Enums (`MovementType`, `MaintenanceStatus`, `UserRole`) are stored as strings in the database (not raw integers) for readability and safety against reordering.
- **Error handling** is centralized: services throw specific exceptions (e.g. `NotFoundException`) instead of returning `null` for every failure case, and a single middleware translates them into consistent HTTP responses — avoiding repeated null-checks and mismatched status codes across controllers.
- **Authentication** uses hand-rolled JWT issuance (not ASP.NET Core Identity) paired with `PasswordHasher<User>` for hashing — kept deliberately lightweight for this project's scope, with no refresh tokens (a documented, intentional scope decision rather than an oversight).
