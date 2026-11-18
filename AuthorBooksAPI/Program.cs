using AuthorBooksAPI.Domain.Entities;
using AuthorBooksAPI.Infrastructure;
using AuthorBooksAPI.Infrastructure.Implementations;
using AuthorBooksAPI.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database: in-memory for now (no SQL Server needed).
// Later, to use real SQL Server, replace this with:
//   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
builder.Services.AddDbContext<AuthorBooksAPIDbContext>(options =>
    options.UseInMemoryDatabase("AuthorBooksDb"));

builder.Services.AddScoped<IBooksRepository, BooksRepository>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Swagger (Swashbuckle) — gives a web page to view and test the API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed the in-memory database with the same data as the SQL script.
// (Delete this block when switching to SQL Server — the script inserts the data there.)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthorBooksAPIDbContext>();
    if (!db.Authors.Any())
    {
        var author = new Author
        {
            AuthorId = 1,
            FirstName = "Pemba",
            LastName = "Tamang",
            Address = "Lalitpur, Bagmati Province, Nepal",
            CreatedDate = DateTime.UtcNow
        };
        var book = new Book
        {
            BookId = 1,
            Title = "C# Clean Architecture and DDD",
            BookNumber = "BK-2026-0987",
            PublishedYear = 2026,
            CreatedDate = DateTime.UtcNow
        };
        book.Authors.Add(author); // fills the AuthorBook junction table
        db.Books.Add(book);
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(Options =>
    {
        Options.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthorBooksAPI v1");

    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
