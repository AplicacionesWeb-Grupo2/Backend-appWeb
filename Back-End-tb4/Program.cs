using Back_End_tb4.Domain.Entities;
using Back_End_tb4.Domain.Repositories;
using Back_End_tb4.Infrastructure.Persistence;
using Back_End_tb4.Infrastructure.Repositories;
using Back_End_tb4.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============== CONFIGURACIÓN JWT ==============
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Solo para desarrollo
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
// ===============================================

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

// ============== REGISTRAR JwtService ==============
builder.Services.AddScoped<JwtService>();
// =================================================

// ============== AGREGAR CORS PARA VUE.JS ==============
builder.Services.AddCors(options =>
{
    options.AddPolicy("NetlifyFrontend",
        policy =>
        {
            policy.WithOrigins(
                    // URL DE NETLIFY
                    "https://eiramindfront.netlify.app",
                
                    // Para desarrollo local
                    "http://localhost:5173",
                    "http://127.0.0.1:5173",
                    "https://localhost:5173"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Authorization"); // Importante para JWT
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

// ============== IMPORTANTE: El orden es crucial ==============
app.UseHttpsRedirection();
app.UseCors("NetlifyFrontend"); // CORS debe ir después de UseHttpsRedirection
app.UseAuthentication(); // Authentication debe ir antes de Authorization
app.UseAuthorization(); // Authorization debe ir antes de MapControllers
// =============================================================

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
}).AllowAnonymous();

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
}).AllowAnonymous();

app.MapGet("/api/test-auth", () =>
{
    return new
    {
        message = "✅ Endpoint protegido por JWT",
        timestamp = DateTime.UtcNow
    };
}).RequireAuthorization(); // Este endpoint requiere autenticación
// ================================================

app.MapControllers();
app.Run();