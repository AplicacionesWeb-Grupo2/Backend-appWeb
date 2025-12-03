using Back_End_tb4.Domain.Entities;
using Back_End_tb4.Domain.Repositories;
using Back_End_tb4.Infrastructure.Persistence;
using Back_End_tb4.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
    throw new Exception("Database connection string is not set.");

builder.Services.AddDbContext<EiraMindDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging());

// Register repositories
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IRepository<User>, Repository<User>>();
builder.Services.AddScoped<IRepository<Psychologist>, Repository<Psychologist>>();
builder.Services.AddScoped<IRepository<Appointment>, Repository<Appointment>>();

// ============== AGREGAR CORS PARA VUE.JS ==============
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});
// =======================================================


var app = builder.Build();

// Apply migrations and create database automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EiraMindDbContext>();
    
    try
    {
        // Esto creará la base de datos si no existe
        dbContext.Database.EnsureCreated();
        Console.WriteLine("✅ Base de datos verificada/creada correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al crear la base de datos: {ex.Message}");
        throw;
    }
}

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EiraMindDbContext>();
    await SeedData.InitializeAsync(dbContext);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

// ============== USAR CORS (ANTES DE MapControllers) ==============
app.UseCors("VueFrontend");
// ================================================================

app.UseHttpsRedirection();

// ============== ENDPOINTS DE PRUEBA ==============
app.MapGet("/api/test", () => 
{
    return new 
    {
        message = "✅ Backend EiraMind funcionando",
        backend = "http://localhost:5293",
        frontend_allowed = "http://localhost:5173",
        endpoints = new[]
        {
            "/api/users",
            "/api/psychologists", 
            "/api/appointments",
            "/api/plans"
        },
        timestamp = DateTime.UtcNow
    };
});

app.MapGet("/api/test-cors", () => 
{
    return new 
    {
        message = "✅ CORS configurado para Vue.js",
        test = "Si ves esto desde Vue.js, CORS funciona",
        frontend = "http://localhost:5173",
        backend = "http://localhost:5293",
        timestamp = DateTime.UtcNow
    };
});
// ================================================

app.MapControllers();
app.Run();