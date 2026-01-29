using backend.Application.Services;
using backend.Domain.Interfaces;
using backend.Infrastructure.Data;
using backend.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKeyString = jwtSettings["SecretKey"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

// ⬇️ ADICIONE ESTES LOGS
Console.WriteLine($"🔑 Secret Key: {secretKeyString}");
Console.WriteLine($"🔑 Tamanho da Secret Key: {secretKeyString?.Length}");
Console.WriteLine($"🏢 Issuer: {issuer}");
Console.WriteLine($"👥 Audience: {audience}");

var secretKey = Encoding.UTF8.GetBytes(secretKeyString!);
Console.WriteLine($"🔑 Secret Key (bytes): {secretKey.Length} bytes");

if (string.IsNullOrEmpty(secretKeyString) || secretKeyString.Length < 32)
{
    throw new InvalidOperationException("SecretKey no appsettings.json inválida ou muito curta (mín. 32 caracteres)");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

                // Log completo do que está sendo processado
                var authHeader = context.Request.Headers["Authorization"].ToString();
                logger.LogInformation($"📨 OnMessageReceived - Header completo: '{authHeader}'");

                // Log do token que o middleware vai processar
                if (!string.IsNullOrEmpty(context.Token))
                {
                    logger.LogInformation($"🎫 Token no context (primeiros 100 chars): '{context.Token.Substring(0, Math.Min(100, context.Token.Length))}'");

                    // Contar os pontos no token
                    int dots = context.Token.Count(c => c == '.');
                    logger.LogInformation($"🔢 Número de pontos no token: {dots}");
                }
                else
                {
                    logger.LogWarning("⚠️ context.Token está vazio!");
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError($"❌ Falha autenticação: {context.Exception?.Message}");
                logger.LogError($"❌ Exception completa: {context.Exception}");

                // Verificar o que foi tentado validar
                var authHeader = context.Request.Headers["Authorization"].ToString();
                logger.LogError($"❌ Header recebido: '{authHeader}'");

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("✅ Token validado com sucesso");

                // Log das claims
                var claims = context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}");
                if (claims != null)
                {
                    logger.LogInformation($"✅ Claims: {string.Join(", ", claims)}");
                }

                return Task.CompletedTask;
            }
        };
    });

// CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    // Log do header Authorization
    var authHeader = context.Request.Headers["Authorization"].ToString();
    logger.LogInformation($"🔍 Authorization Header: '{authHeader}'");

    if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
    {
        var token = authHeader.Substring("Bearer ".Length).Trim();
        logger.LogInformation($"🎫 Token extraído: {token.Substring(0, Math.Min(50, token.Length))}...");
    }
    else
    {
        logger.LogWarning("⚠️ Token não encontrado ou formato inválido");
    }

    await next();
});

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReact");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
