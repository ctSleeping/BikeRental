# BikeRental API

Backend for a bike rental service: ASP.NET Core Web API, .NET 8,
Entity Framework Core 8 and SQLite. Built for the Backend Programming
course (TT00BW43) and used as the backend for the frontend course project.

## Getting started

Requirements: .NET 8 SDK.

1. Set the JWT signing key (once per computer, from the `BikeRental.Api` folder).
   It is stored in user-secrets, outside the repository, and must be at
   least 32 characters:
```
   cd BikeRental.Api
   dotnet user-secrets set "Jwt:Key" "replace-with-a-long-random-secret-of-32-chars-or-more"
```
2. Start the API from the repository root:
```
   dotnet run --project BikeRental.Api
```
   or open the solution in Visual Studio and press F5.

   The database (`bikerental.db`) is created automatically on first start,
   with 3 stations, 5 bikes and 3 users.
3. Open Swagger at `http://localhost:5260/swagger`. To call protected
   endpoints, log in with `POST /api/auth/login`, click **Authorize** and
   paste the token.

To reset the database, stop the API, delete `bikerental.db` and start the
API again.

### Seed users

All seed users have the password `Password123!`.

| Email | Role |
|---|---|
| `matti@example.com` | Customer |
| `liisa@example.com` | Customer |
| `admin@example.com` | Admin |

New users registered through the API are always Customers.

## Using the API from a frontend

- Base URL: `http://localhost:5260`
- CORS allows `http://localhost:5173`. For a different frontend address,
  change the `WithOrigins(...)` line in `Program.cs`.
- Log in with `POST /api/auth/login` and send the returned token on
  protected requests: `Authorization: Bearer <token>`. Tokens are valid
  for 2 hours.
- JSON uses camelCase, enums are strings (`"type": "City"`) and times are
  UTC ending in `Z`.
- Errors use ProblemDetails with a machine-readable `code` field, for
  example `{ "title": "Conflict", "status": 409, "detail": "...", "code": "UserHasActiveRental" }`.

## Endpoints

| Method | URL | Access |
|---|---|---|
| POST | `/api/auth/register` | Everyone |
| POST | `/api/auth/login` | Everyone |
| GET | `/api/users/me` | Logged in |
| GET | `/api/users` | Admin |
| GET | `/api/bikes?type=&status=&stationId=&page=&pageSize=` | Everyone |
| GET | `/api/bikes/{id}` | Everyone |
| POST / PUT / DELETE | `/api/bikes`, `/api/bikes/{id}` | Admin |
| GET | `/api/stations`, `/api/stations/{id}` | Everyone |
| POST / PUT / DELETE | `/api/stations`, `/api/stations/{id}` | Admin |
| GET | `/api/rentals`, `/api/rentals/{id}` | Logged in (own rentals; Admin sees all) |
| POST | `/api/rentals` with `{ "bikeId": 1 }` | Logged in |
| POST | `/api/rentals/{id}/return` with optional `{ "returnStationId": 3 }` | Logged in (own rentals) |

Creating a resource returns `201 Created` with a `Location` header.
`GET /api/bikes` always returns `{ items, totalCount, page, pageSize, totalPages }`.

## Business rules

| Rule | Error |
|---|---|
| S1 The bike must exist and not be deleted | 404 `BikeNotFound` |
| S2 The bike must not be in maintenance or rented | 409 `BikeNotAvailable` |
| S3 A user can have only one active rental | 409 `UserHasActiveRental` |
| S4 Only an active rental can be returned | 409 `RentalNotActive` |
| S5 Only the owner can see and return a rental | 404 `RentalNotFound` |
| S6 Status cannot be set directly; only `inMaintenance` can be changed | – |
| S7/S8 A rented bike cannot be set to maintenance or deleted | 409 `BikeRented` |
| S9 Deleting a bike is a soft delete, so rental history is kept | – |
| S10 A station with bikes cannot be deleted | 409 `StationHasBikes` |
| S11 An email can be registered only once | 409 `EmailTaken` |

**Pricing:** `hours = max(1, ceil(duration in hours))`,
`total = hours × hourlyPrice`. The hourly price is copied into the rental
when it starts, so later price changes don't affect it.

**Stations:** a rental's pickup station is read from the bike. On return,
the bike moves to the chosen return station, or back to the pickup station
if none is given.

**Status** is computed, never stored: a bike is `InMaintenance`, `Rented`
(it has a rental without an end time) or `Available`. A rental is `Active`
or `Completed` depending on `endUtc`.

## Structure

- `Controllers/`: HTTP only (routing, status codes, authorization attributes)
- `Services/`: business rules and database work
- `Data/AppDbContext.cs`: EF Core model, relationships and seed data
- `Dtos/`: request and response shapes; entities are never returned directly
- `Common/`: error types and the exception middleware
- `BikeRental.Tests/`: xUnit tests for the price calculation

## Testing

- `BikeRental.Api/BikeRental.http` runs the whole flow (50 requests,
  including every error case). Reset the database first and run the
  requests in order; each comment shows the expected result.
- Unit tests: `dotnet test` from the repository root.

## Use of AI

Claude was used for assistance in building and understanding concepts with
how to link each compononent together. Also helped a lot with the coding and debugging
when mistakes/errors were made.

## Known limitations

- Local development setup: HTTP only, no HTTPS redirect, and CORS allows
  one hard-coded origin.
- No refresh tokens; log in again after the token expires (2 hours).
- Admin users can only come from seed data.
- 400 validation errors use ASP.NET's default format, and 401/403
  responses have an empty body; neither has a `code` field.
- Deleting a station clears the station ids of old rentals that used it.