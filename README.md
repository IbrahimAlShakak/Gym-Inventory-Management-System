# Gym Inventory / Warehouse Management System

A backend-focused inventory and warehouse management system for a gym.

## Tech Stack

- **Backend:** C# / ASP.NET Core
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Frontend:** React *(planned)*

## Status

🚧 Work in progress — currently building out the data layer.

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
- Database seeder with sample data (10 rows across core tables) for local testing

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

# Run the app
dotnet run
```

On first run, the app seeds sample data automatically (categories, locations, items, users, stock levels, movements, and transfers) if the database is empty.

### Configuration
Update the `DefaultConnection` string in `appsettings.json` to point to your local SQL Server instance.

## Design Notes

Some of the key modeling decisions behind this project:

- **Equipment vs. Consumable** are modeled as separate subtypes (not a boolean flag) via EF Core's Table-Per-Type (TPT) inheritance strategy, since they need genuinely different fields.
- **StockLevel vs. StockMovement**: `StockMovement` is the append-only source of truth (what happened, when, by whom); `StockLevel` is a cached current-quantity snapshot kept in sync with it. This mirrors how ledger/accounting systems track state.
- **Multi-location support** is built in from the start — every stock quantity and movement is tied to a specific `Location`.
- Enums (`MovementType`, `MaintenanceStatus`, `UserRole`) are stored as strings in the database (not raw integers) for readability and safety against reordering.
