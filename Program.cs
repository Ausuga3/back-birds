using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BackBird.Api.src.Bird.Modules.Users.Infrastructure.Config;
using System;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

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
		// Allow deserializing enum values from their string names (e.g. "Usuario")
		opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	});
// registrar la infraestructura (DbContext, repos, servicios)
builder.Services.AddUsersInfrastructure(builder.Configuration);

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

// Crear tablas manualmente si no existen
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var usersDb = services.GetRequiredService<BackBird.Api.src.Bird.Modules.Users.Infrastructure.Persistence.UsersDbContext>();
    
    // Asegurar que la base de datos existe
    usersDb.Database.EnsureCreated();
    
    // Crear tabla Users si no existe (manual por problemas multi-context)
    var conn = usersDb.Database.GetDbConnection();
    conn.Open();
    using var cmd = conn.CreateCommand();
    cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS Users (
            Id TEXT PRIMARY KEY,
            Email TEXT NOT NULL UNIQUE,
            Name TEXT NOT NULL,
            PasswordHash TEXT NOT NULL,
            Role INTEGER NOT NULL,
            Created_At TEXT NOT NULL,
            Updated_At TEXT NOT NULL
        );
    ";
    cmd.ExecuteNonQuery();
    conn.Close();
    
    // Seed admin user if not exists
    var userRepo = services.GetRequiredService<BackBird.Api.src.Bird.Modules.Users.Domain.Repositories.IUserRepository>();
    var hasher = services.GetRequiredService<BackBird.Api.src.Bird.Modules.Users.Domain.Interfaces.IPasswordHasher>();
    
    var adminEmail = "admin@test.com";
    var existingAdmin = await userRepo.GetByEmailAsync(adminEmail);
    if (existingAdmin == null)
    {
        var adminHash = hasher.Hash("Admin123!");
        var admin = new BackBird.Api.src.Bird.Modules.Users.Domain.Entities.User(
            adminEmail,
            adminHash,
            "Admin Test",
            BackBird.Api.src.Bird.Modules.Users.Domain.Enums.Role.Admin
        );
        await userRepo.AddAsync(admin);
        Console.WriteLine("✅ Usuario admin creado: admin@test.com / Admin123!");
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
