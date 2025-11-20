using System.Text.Json.Serialization;
using SteamClone.Backend.Services;
using SteamClone.Backend.Services.Interfaces;
using SteamClone.Backend.Settings;
using SteamClone.Backend.Entities;
using SteamClone.Backend.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using SteamClone.Backend.Validators.Game;

var builder = WebApplication.CreateBuilder(args);

// Configure Entity Framework DbContext with SQL Server connection
builder.Services.AddDbContext<BackendDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure AutoMapper for DTO-Entity mappings
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configure FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateGameRequestDtoValidator>();

// Load JWT settings from appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Register application services with dependency injection
// Use Scoped lifetime for services that interact with DbContext
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddSingleton<JwtService>(); // Singleton for stateless token generation

// Configure JWT Bearer Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var config = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    if (config == null)
        throw new InvalidOperationException("JwtSettings configuration section is missing or invalid.");

    // Configure token validation parameters
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = config.Issuer,
        ValidateAudience = true,
        ValidAudience = config.Audience,
        ValidateLifetime = true,  // Reject expired tokens
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Key))
    };
});
builder.Services.AddAuthorization();

// Configure MVC controllers with JSON serialization options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Convert enums to strings in JSON responses for better readability
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configure API documentation with Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed database with initial data on application startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BackendDbContext>();
    DbSeeder.SeedDatabase(dbContext);
}

// Configure middleware pipeline for development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // Enable Swagger JSON endpoint
    app.UseSwaggerUI();    // Enable Swagger UI for API testing
}

// Configure middleware pipeline
app.UseAuthentication();   // Enable JWT authentication
app.UseAuthorization();    // Enable role-based authorization
app.MapControllers();      // Map controller endpoints
app.Run();

