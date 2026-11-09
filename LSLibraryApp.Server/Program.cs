using LSLibraryApp.Server.Data;
using LSLibraryApp.Server.Extensions;
using LSLibraryApp.Server.Repositories;
using LSLibraryApp.Server.Repositories.Interface;
using LSLibraryApp.Server.Services;
using LSLibraryApp.Server.Services.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//builder.Services.AddDbContext<LibraryDbContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseInMemoryDatabase("LibraryAppDB"));

builder.Services
    .AddScoped<ITopicService, TopicService>()
    .AddScoped<IBookService, BookService>()
    .AddScoped<ITopicRepository, TopicRepository>()
    .AddScoped<IBookRepository, BookRepository>()
    .AddScoped<IBookReferenceRepository, BookReferenceRepository>();

MappingConfiguration.RegisterMappings();

var app = builder.Build();

// Initialize the database in development
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        await dbContext.SeedTopicsAsync();
        await dbContext.SeedBooksAsync();
    }
}

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
