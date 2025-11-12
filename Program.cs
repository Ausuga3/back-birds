using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BackBird.Api.src.Bird.Modules.Users.Infrastructure.Config;
using BackBird.Api.src.Bird.Modules.Users.Infrastructure.Persistence;
using BackBird.Api.src.Bird.Modules.Birds.Infrastructure.Persistence;
using BackBird.Api.src.Bird.Modules.Birds.Domain.Repositories;
using BackBird.Api.src.Bird.Modules.Birds.Aplication.Commands.CreateBird;
using BackBird.Api.src.Bird.Modules.Birds.Aplication.Commands.UpdateBird;
using BackBird.Api.src.Bird.Modules.Birds.Aplication.Queries.GetAllBirds;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;
using BackBird.Api.src.Bird.Modules.Birds.Infrastructure.Json;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
 options.AddPolicy(name: "AllowFrontend",
 policy =>
 {
 policy.WithOrigins("http://localhost:4200")
 .AllowAnyHeader()
 .AllowAnyMethod();
 });
});

// Add services
builder.Services.AddControllers()
	.AddJsonOptions(opts =>
	{
		// Serializar enums usando los valores de EnumMember (valores en español)
		opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumMemberConverter());
	});

// registrar la infraestructura de Users (DbContext, repos, servicios)
builder.Services.AddUsersInfrastructure(builder.Configuration);

// registrar la infraestructura de Birds
builder.Services.AddDbContext<BirdsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IBirdRepository, BirdRepository>();
builder.Services.AddScoped<CreateBirdHandler>();
builder.Services.AddScoped<UpdateBirdHandler>();
builder.Services.AddScoped<GetAllBirdsHandler>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
 var cfg = builder.Configuration;
 // parse key as base64 if possible, otherwise UTF8
 var keyConfig = cfg["Jwt:Key"] ?? string.Empty;
 byte[] keyBytes;
 try
 {
 keyBytes = Convert.FromBase64String(keyConfig);
 }
 catch
 {
 keyBytes = Encoding.UTF8.GetBytes(keyConfig);
 }

 options.RequireHttpsMetadata = false; // allow HTTP in development if needed
 options.TokenValidationParameters = new TokenValidationParameters
 {
 ValidateIssuer = true,
 ValidateAudience = true,
 ValidateLifetime = true,
 ValidateIssuerSigningKey = true,
 ValidIssuer = cfg["Jwt:Issuer"],
 ValidAudience = cfg["Jwt:Audience"],
 IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
 };

 options.Events = new JwtBearerEvents
 {
 OnAuthenticationFailed = ctx =>
 {
 var logger = ctx.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILoggerFactory>()?.CreateLogger("JwtBearer");
 logger?.LogError(ctx.Exception, "Authentication failed");
 return System.Threading.Tasks.Task.CompletedTask;
 },
 OnChallenge = context =>
 {
 var logger = context.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILoggerFactory>()?.CreateLogger("JwtBearer");
 logger?.LogWarning("JwtBearer challenge: {0}", context.ErrorDescription);
 return System.Threading.Tasks.Task.CompletedTask;
 }
 };
 });

builder.Services.AddAuthorization();

var app = builder.Build();

// Crear base de datos y tablas automáticamente en desarrollo
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var birdDb = scope.ServiceProvider.GetRequiredService<BirdsDbContext>();
        var usersDb = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        
        // Crear tabla Birds (esto crea el archivo backbird.db)
        birdDb.Database.EnsureCreated();
        
        // WORKAROUND: EnsureCreated() no crea Users porque la BD ya existe
        // Solución: Crear manualmente la tabla Users con SQL
        var createUsersTable = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id TEXT NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
                Email TEXT NOT NULL,
                PasswordHash TEXT NOT NULL,
                Name TEXT NOT NULL,
                Role INTEGER NOT NULL,
                Created_At TEXT NOT NULL
            );
            CREATE UNIQUE INDEX IF NOT EXISTS IX_Users_Email ON Users (Email);
        ";
        usersDb.Database.ExecuteSqlRaw(createUsersTable);
        
        // Seed usuario admin si no existe
        try
        {
            if (!usersDb.Users.Any(u => u.Email == "admin@test.com"))
            {
                // Generar hash en tiempo de ejecución con BCrypt
                var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!", workFactor: 11);
                
                var admin = new BackBird.Api.src.Bird.Modules.Users.Domain.Entities.User(
                    email: "admin@test.com",
                    passwordHash: passwordHash,
                    name: "Admin Test",
                    role: BackBird.Api.src.Bird.Modules.Users.Domain.Enums.Role.Admin
                );
                usersDb.Users.Add(admin);
                usersDb.SaveChanges();
                Console.WriteLine("✅ Usuario admin@test.com creado (Password: Admin123!)");
            }
            else
            {
                Console.WriteLine("ℹ️  Usuario admin@test.com ya existe");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al crear usuario admin: {ex.Message}");
        }
    }
}

// Enable middleware for Swagger in development
if (app.Environment.IsDevelopment())
{
 app.UseSwagger();
 app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
