using WebApiEntityFramework.Contracts.Repositories;
using WebApiEntityFramework.Data;
using WebApiEntityFramework.DatabaseContext;
using WebApiEntityFramework.Implementations.Middleware;
using WebApiEntityFramework.Implementations.Repositories;
using WebApiEntityFramework.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

builder.Services.AddDbContext<InMemoryDbContext>();

// Add services to the container.
builder.Services.AddScoped<IEmployeeRepository, EmployeeEFRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Register your custom error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//seed the database with some initial records
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

app.Run();

public partial class Program { }