using System.Text.Json.Serialization;
using SteamClone.Backend.Services;
using SteamClone.Backend.Services.Interfaces;
using SteamClone.Backend.Settings;
using SteamClone.Backend.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using AutoMapper;
using SteamClone.Backend.Profiles;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext with SQL Server
builder.Services.AddDbContext<BackendDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Manual AutoMapper configuration
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<UserProfile>();
    cfg.AddProfile<CouponsProfile>();
    cfg.AddProfile<GameProfile>();
    cfg.AddProfile<CartProfile>();
    cfg.AddProfile<OrderProfile>();
    cfg.AddProfile<ReviewProfile>();
});
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddSingleton<IReviewService, ReviewService>();
builder.Services.AddSingleton<ICouponService, CouponService>();
builder.Services.AddSingleton<IAnalyticsService, AnalyticsService>();
builder.Services.AddSingleton<JwtService>();

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

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = config.Issuer,
        ValidateAudience = true,
        ValidAudience = config.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Key))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

