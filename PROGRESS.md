# AuthorBooksAPI — Progress Notes

_Last updated: 2026-11-17_

## Goal

Build an **ASP.NET Core Web API** (.NET 10) for authors and books using **Clean Architecture**, following the class instructor — but on a **Mac** instead of Windows/Visual Studio.

The finished API should let you list, view, add, update and (soft) delete books and authors, stored in **SQL Server**.

---

## Project structure (the 4 layers)

| Project | Job | Contains |
|---|---|---|
| `AuthorBooksAPI.Domain` | Core data shapes, depends on nothing | `Author`, `Book`, `BaseEntity` |
| `AuthorBooksAPI.Application` | Logic + DTOs (data sent in/out of the API) | `AuthorDto`, `BookDto`, `BaseDto` |
| `AuthorBooksAPI.Infrastructure` | Talks to the database | `AuthorBooksAPIDbContext`, `IBooksRepository`, `BooksRepository` |
| `AuthorBooksAPI` | The web part — receives HTTP requests | `Program.cs`, controllers |

Request flow:

```
Browser → BooksController → IBooksRepository → BooksRepository → DbContext → database
```

---

## What's done ✅

### Setup
- [x] Confirmed the project runs on Mac (.NET 10 SDK installed; it's not Windows-only like WinForms)
- [x] Installed the **mssql** VS Code extension (viewer only — no SQL Server yet)

### Code
- [x] **Project references** connected: API → Application + Infrastructure, Application → Domain, Infrastructure → Domain
- [x] **DbContext**: added the instructor's `OnModelCreating` (many-to-many `AuthorBook` join table)
- [x] **DbContext**: added constructor taking `DbContextOptions` so `Program.cs` can choose the database
- [x] **Program.cs**: registered the DbContext with an **in-memory database** (temporary stand-in for SQL Server)
- [x] **Program.cs**: registered `IBooksRepository` → `BooksRepository` (dependency injection)
- [x] **Program.cs**: seeds the same data as the instructor's SQL script (1 author, 1 book, 1 link)
- [x] **BooksController**: `GET /api/books` returns all books as `BookDto`
- [x] **Swagger**: added `Swashbuckle.AspNetCore` **10.3.1** (11.x conflicts with `Microsoft.AspNetCore.OpenApi`)

### Packages
| Package | Project | Why |
|---|---|---|
| `Microsoft.EntityFrameworkCore` | Infrastructure | EF Core (from instructor) |
| `Microsoft.EntityFrameworkCore.InMemory` 10.0.14 | API | Temporary fake database |
| `Swashbuckle.AspNetCore` 10.3.1 | API | Swagger UI page |

---

## What's left ⏳

### Fix now
- [ ] **Swagger is broken** — `app.UseSwagger();` was removed from `Program.cs`, so `/swagger/v1/swagger.json` is 404 and the page says "Failed to load API definition". Add it back above `app.UseSwaggerUI(...)`.

### Current task
- [ ] **`GET /api/books/{id}`** — return one book, or 404 if not found
  1. `IBooksRepository`: add `Task<Book?> GetByIdAsync(int id);`
  2. `BooksRepository`: implement with `FirstOrDefaultAsync(x => x.BookId == id && x.DeletedDate == null)`
  3. `BooksController`: add `[HttpGet("{id}")]` action, return `NotFound()` or `Ok(dto)`
  4. Test in Swagger with id `1` (200) and `99` (404)

### Likely next in class
- [ ] `POST /api/books` — create a book
- [ ] `PUT /api/books/{id}` — update a book
- [ ] `DELETE /api/books/{id}` — soft delete (set `DeletedDate`)
- [ ] Same endpoints for **Authors** (`IAuthorsRepository`, `AuthorsRepository`, `AuthorsController`)
- [ ] Return authors with books (`.Include(b => b.Authors)`)

### Switch to real SQL Server (when ready)
- [ ] Install **Docker Desktop** (Apple chip) and turn on Rosetta in Settings
- [ ] Run SQL Server in Docker:
  ```bash
  docker run -d --name sqlserver --platform linux/amd64 \
    -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Pass123" \
    -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
  ```
- [ ] Connect with the mssql extension (`localhost`, `sa`, password, trust certificate)
- [ ] `CREATE DATABASE AuthorBooksDb;` then run the instructor's SQL script
- [ ] Add package `Microsoft.EntityFrameworkCore.SqlServer`
- [ ] Add connection string to `appsettings.json`:
  `Server=localhost,1433;Database=AuthorBooksDb;User Id=sa;Password=YourStrong@Pass123;TrustServerCertificate=True`
- [ ] In `Program.cs`: `UseInMemoryDatabase(...)` → `UseSqlServer(...)`, and delete the seed block

### Housekeeping
- [ ] Commit the work to git

---

## In-memory vs instructor's SQL Server

| Works the same now | Needs SQL Server (Docker) |
|---|---|
| Controllers, endpoints, routing | Running the SQL script |
| Repositories, DI, DTOs | Viewing tables (SSMS / mssql extension) |
| LINQ queries, `.Include()` | Migrations (`dotnet ef ...`) |
| Swagger | Raw SQL, stored procedures, transactions |
| Add/update/delete (until restart) | Data that survives a restart |

> In-memory also does **not** enforce max lengths, foreign keys or SQL defaults like `GETUTCDATE()`, and string comparisons are case-sensitive.

---

## My level

**Beginner with ASP.NET Core Web APIs** — coming from desktop C# (WinForms) and Visual Studio.

**Strengths**
- Asks *why*, not just *how* — wants to understand what each change does
- Checks whether something matches the instructor's way before accepting it
- Willing to edit code directly and see what happens (e.g. the Swagger change)

**Things that were confusing (now explained)**
- Terminal folders — the repo folder and project folder have the same name (`AuthorBooksAPI/AuthorBooksAPI`)
- A **database** (SQL Server) vs a **viewer** (SSMS / mssql extension)
- **NuGet packages** (`dotnet add package`) vs **VS Code extensions**
- What the Domain / Application / Infrastructure folders are for
- In-memory "simulated" database vs a real one

---

## What to work on

1. **Terminal basics** — `pwd`, `cd`, `ls`; know which folder you're in before running `dotnet` commands
2. **The request flow** — be able to trace a request through controller → repository → DbContext without help
3. **Dependency injection** — why `AddScoped<IBooksRepository, BooksRepository>()` lets the controller receive a repository
4. **Async/await** — what `Task<...>`, `async`, `await` and `ToListAsync()` mean
5. **HTTP basics** — methods (GET/POST/PUT/DELETE), status codes (200, 201, 204, 400, 404), route parameters (`{id}`)
6. **LINQ** — `Where`, `Select`, `FirstOrDefault`, `Include`
7. **Read error messages** — the build error usually names the file, line and the missing piece (like the Swagger package conflict)
8. **Writing code yourself** — do the `GET /api/books/{id}` task on your own, then get it checked

---

## Useful commands

```bash
pwd                                         # which folder am I in?
dotnet run                                  # run (from inside AuthorBooksAPI/AuthorBooksAPI)
dotnet run --project AuthorBooksAPI         # run (from the repo root)
dotnet build                                # check for errors without running
dotnet add package <Name> --version <x>     # add a NuGet package
git status                                  # see what changed
```

| URL | What |
|---|---|
| http://localhost:5229/swagger | Swagger UI (test endpoints) |
| http://localhost:5229/api/books | All books (JSON) |
| http://localhost:5229/weatherforecast | Template sample endpoint |
