# BikeRental API

Backend for a bike rental service, built as the project work for
Backend Programming. ASP.NET Core Web API, .NET 8, Entity Framework Core 8 and SQLite.

## Getting started

Requirements: .NET 8 SDK.

1. Install the EF Core tool (once per computer):
```
   dotnet tool install --global dotnet-ef --version 8.*
```
2. Go to the project folder and create the database:
```
   cd BikeRental.Api
   dotnet ef database update-> this creates `bikerental.db` with 5 seed bikes.
3. Start the API:

   dotnet run  or press F5 in Visual Studio.
4. Open Swagger at `http://localhost:5077/swagger`.

To reset the database, stop the API, delete `bikerental.db` and run `dotnet ef database update` again.

## Features

- Bike CRUD: `GET /api/bikes`, `GET /api/bikes/{id}`, `POST /api/bikes`,
  `PUT /api/bikes/{id}`, `DELETE /api/bikes/{id}`
- Soft delete: deleted bikes get a `DeletedUtc` timestamp and are hidden,
  but stay in the database
- Paging: `GET /api/bikes?page=1&pageSize=10` always returns
  `{ items, totalCount, page, pageSize, totalPages }`
- Bike status (`Available` / `InMaintenance`) is computed, not stored
- Validation with data annotations (400 for invalid input)
- DTOs for all input and output; entities are never returned directly
- Enums as strings in JSON (`"type": "City"`)
- EF Core migrations and seed data
- `BikeRental.http` covers every bike endpoint, including error cases

## Use of AI

Claude was used primarily for assistance in building and understanding concepts with
how to link each compononent together. Also helped with code itself and debugging
when mistakes/errors were made.

## Known limitations

- Users, rentals and the rental rules (level 3) are not implemented.
- Errors use the default ASP.NET format, without the `code` field.
- No authentication (level 4).