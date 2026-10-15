# BikeRental API
Christian Tiensuu/e2501203
Backend project for a bike rental service,
 ASP.NET Core Web API, .NET 8 & Entity Framework Core 8.

## Level goal

Everything in the project should fulfill the requirements for a level 3.

## Getting started

Requirements: .NET 8 SDK.

1. Install the EF Core tool (once per computer):
```
   dotnet tool install --global dotnet-ef --version 8.*
```
2. Go to the project folder and create the database:
```
   cd BikeRental.Api
   dotnet ef database update
```
   This creates `bikerental.db` with 5 seed bikes and 3 seed users.
3. Start the API:
```
   dotnet run
```
   or press F5 in Visual Studio.
4. Open Swagger at `http://localhost:5260/swagger`.

To reset the database, stop the API, delete `bikerental.db`
and run `dotnet ef database update` again.

## Testing

`BikeRental.Api/BikeRental.http` runs the whole flow: bike CRUD,
renting, returning, price calculation and every error case.
Reset the database first, then run requests #1–#30 in order.
Each request's comment shows the expected status code.

## Endpoints

| Method | URL | Description |
|---|---|---|
| GET | `/api/bikes?page=1&pageSize=10` | List bikes (paging wrapper) |
| GET | `/api/bikes/{id}` | One bike |
| POST | `/api/bikes` | Create a bike → 201 + Location |
| PUT | `/api/bikes/{id}` | Update a bike (incl. `inMaintenance`) |
| DELETE | `/api/bikes/{id}` | Soft delete → 204 |
| GET | `/api/users` | List users |
| GET | `/api/rentals` | List rentals |
| GET | `/api/rentals/{id}` | One rental |
| POST | `/api/rentals` | Start a rental `{ "bikeId": 1, "userId": 1 }` → 201 + Location |
| POST | `/api/rentals/{id}/return` | Return a rental, calculates the price |

## Demo

Before starting: stop the API, delete `bikerental.db`,
run `dotnet ef database update` to get a fresh db, then start the API (F5).

Run these requests from `BikeRental.http` in this order:

| Order | Request | Shows |
|---|---|---|
| 1 | **#1** GET all bikes
| 2 | **#14** User 1 rents bike 1 | 201 Created
| 3 | **#15** User 1 rents another bike | 409 
| 4 | **#3** GET bike 1
| 5 | **#21** Return rental 1
| 6 | **#22** Return it again | 409

## Features

- **Layers:** controllers handle HTTP only, services contain the rules
  and database work, `AppDbContext` handles the database
- **Entities:** Bike, User, Rental with one-to-many relationships
  (a user has many rentals, a bike has many rentals)
- **DTOs** for all input and output; entities are never returned directly,
  and `passwordHash` is never returned
- **Computed status, never stored:** a bike is `InMaintenance`, `Rented`
  (has a rental with no end time) or `Available`; a rental is `Active`
  or `Completed` depending on `endUtc`
- **Price calculation:** `hours = max(1, ceil(duration))`,
  `total = hours × hourlyPrice`. The hourly price is copied into the
  rental when it starts, so later price changes don't affect it.
- **Business rules:**
  - S1 bike must exist and not be deleted → 404 `BikeNotFound`
  - S2 bike must not be in maintenance or rented → 409 `BikeNotAvailable`
  - S3 one active rental per user → 409 `UserHasActiveRental`
  - S4 only an active rental can be returned → 409 `RentalNotActive`
  - S6 status can't be set directly; only `inMaintenance` can be changed
  - S7/S8 a rented bike can't be set to maintenance or deleted → 409 `BikeRented`
  - S9 soft delete keeps the rental history
- **Errors** in ProblemDetails format with a machine-readable `code`
  field, handled in one place by `ExceptionMiddleware`
- **Paging** in the database (`Skip`/`Take`), always in the
  `{ items, totalCount, page, pageSize, totalPages }` wrapper
- Validation with data annotations (400 for invalid input)
- Enums as strings and times in UTC ending in `Z`
- EF Core migrations and seed data

## Use of AI

Claude was used for assistance in building and understanding concepts with
how to link each compononent together. Also helped a lot with the coding and debugging
when mistakes/errors were made.

## Known limitations

- No login (level 4). The user who rents is given in the request
  (`userId`), so anyone can rent as any user and see all rentals,
  and `GET /api/users` is open to everyone.
- Seed users have a placeholder instead of a real password hash.
- No stations, roles or filtering (level 5).
- Validation errors (400) use ASP.NET's default format, without a `code` field.