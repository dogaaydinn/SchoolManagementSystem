using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SchoolManagementSystem.Core.Entities;
using SchoolManagementSystem.Core.Interfaces;
using SchoolManagementSystem.Infrastructure.Caching;
using SchoolManagementSystem.Infrastructure.Data;
using SchoolManagementSystem.Infrastructure.Identity;
using SchoolManagementSystem.Infrastructure.Repositories;
using SchoolManagementSystem.API.Middleware;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq(builder.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
});

// Add Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add JWT Authentication
var jwtSecret = builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "SchoolManagementSystem";
var jwtAudience = builder.Configuration["JWT:Audience"] ?? "SchoolManagementSystem";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Set to true in production
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero
    };
});

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("SuperAdmin", "Admin"));
    options.AddPolicy("TeacherOnly", policy => policy.RequireRole("SuperAdmin", "Admin", "Teacher"));
    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
});

// Add Redis Distributed Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "SchoolManagementSystem_";
});

// Add Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("CorsOrigins").Get<string[]>() ?? Array.Empty<string>())
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register infrastructure services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Register application services
builder.Services.AddScoped<SchoolManagementSystem.Application.Interfaces.IStudentService, SchoolManagementSystem.Application.Services.StudentService>();
builder.Services.AddScoped<SchoolManagementSystem.Application.Interfaces.IGradeService, SchoolManagementSystem.Application.Services.GradeService>();
builder.Services.AddScoped<SchoolManagementSystem.Application.Interfaces.IReportingService, SchoolManagementSystem.Application.Services.ReportingService>();
builder.Services.AddScoped<SchoolManagementSystem.Application.Interfaces.INotificationService, SchoolManagementSystem.Application.Services.NotificationService>();
builder.Services.AddScoped<SchoolManagementSystem.Application.Interfaces.IDocumentService, SchoolManagementSystem.Application.Services.DocumentService>();
builder.Services.AddScoped<SchoolManagementSystem.Application.Interfaces.IBackgroundJobService, SchoolManagementSystem.Application.Services.BackgroundJobService>();
builder.Services.AddScoped<SchoolManagementSystem.Application.Interfaces.IPdfService, SchoolManagementSystem.Application.Services.PdfService>();
builder.Services.AddScoped<SchoolManagementSystem.Application.Interfaces.IExcelService, SchoolManagementSystem.Application.Services.ExcelService>();

// Register file storage service
builder.Services.AddScoped<SchoolManagementSystem.Application.Interfaces.IFileStorageService, SchoolManagementSystem.Infrastructure.Storage.LocalFileStorageService>();

// Add AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database")
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379", "redis");

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "School Management System API",
        Version = "v1",
        Description = "Enterprise-level School Management System REST API",
        Contact = new OpenApiContact
        {
            Name = "Development Team",
            Email = "dev@schoolmanagementsystem.com"
        }
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline

// Use Serilog request logging
app.UseSerilogRequestLogging();

// Global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Use Swagger
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "School Management System API v1");
        options.RoutePrefix = string.Empty; // Set Swagger UI at app root
    });
}

// Use HTTPS redirection
app.UseHttpsRedirection();

// Use CORS
app.UseCors(app.Environment.IsProduction() ? "Production" : "AllowAll");

// Use Rate Limiting
app.UseIpRateLimiting();

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map health checks
app.MapHealthChecks("/health");

// Database migration and seeding
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        Log.Information("Database migration completed successfully");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Database migration failed");
        throw;
    }
}

Log.Information("Starting School Management System API");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
