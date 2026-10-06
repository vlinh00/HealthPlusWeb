using HealthPlus.API.Data;
using HealthPlus.API.Repositories.Interfaces;
using HealthPlus.API.Repositories.Implementations;
using HealthPlus.API.Services.Interfaces;
using HealthPlus.API.Services.Implementations;
using HealthPlus.API.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


// =========================
// Database
// =========================

builder.Services.AddDbContext<HealthPlusDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// =========================
// JWT Settings
// =========================

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// =========================
// Authentication
// =========================

var jwtSettings =
    builder.Configuration
        .GetSection("JwtSettings")
        .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "JwtSettings is not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,

                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings.Key))
            };
    });


// =========================
// Repositories
// =========================

builder.Services.AddScoped<IProductRepository,ProductRepository>();

builder.Services.AddScoped<IUserRepository,UserRepository>();

builder.Services.AddScoped<ICartRepository,CartRepository>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();


// =========================
// Services
// =========================

builder.Services.AddScoped<IProductService,ProductService>();

builder.Services.AddScoped<IAuthService,AuthService>();

builder.Services.AddScoped<ICartService,CartService>();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IPaymentService, PaymentService>();
// =========================
// Swagger
// =========================

builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("bearer", document)] = []
        });
});
// =========================
// App
// =========================

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// IMPORTANT:
// Authentication must be before Authorization

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();