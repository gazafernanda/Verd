using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Verd.Api.Data;
using Verd.Api.Services;

var builder = WebApplication.CreateBuilder(args);


// ── Database ─────────────────────────────────────────────────────────────────
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrWhiteSpace(databaseUrl)) databaseUrl = null;

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (databaseUrl != null)
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        // Neon (and most managed Postgres) omit the port in the URL — default to 5432.
        var port = uri.Port > 0 ? uri.Port : 5432;
        // Railway's internal host doesn't use SSL; external hosts (Neon, etc.) require it.
        var sslMode = uri.Host.EndsWith(".railway.internal") || uri.Host == "localhost"
            ? "Allow"
            : "Require";
        var connStr = $"Host={uri.Host};Port={port};Database={uri.AbsolutePath.TrimStart('/')};" +
                      $"Username={userInfo[0]};Password={Uri.UnescapeDataString(userInfo[1])};" +
                      $"SSL Mode={sslMode};Trust Server Certificate=true";
        options.UseNpgsql(connStr);
    }
    else
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Secret"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            NameClaimType = "sub",
        };
    });

builder.Services.AddAuthorization();

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<RecommendationAiService>();
builder.Services.AddHttpClient();

var groqApiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY")
    ?? Environment.GetEnvironmentVariable("Groq__ApiKey")
    ?? builder.Configuration["Groq:ApiKey"]
    ?? "";

builder.Services.AddHttpClient("Groq", client =>
{
    client.BaseAddress = new Uri("https://api.groq.com/openai/v1/");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {groqApiKey}");
});

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("VueDev", policy =>
        policy
          .SetIsOriginAllowed(origin => {
              try {
                  var host = new Uri(origin).Host;
                  return host == "localhost" || host == "gazafernanda.github.io";
              } catch {
                  return false;
              }
          })
          .AllowAnyHeader()
          .AllowAnyMethod()
    );
});

// ── Controllers & Swagger ─────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Verd API", Version = "v1" });
    var bearer = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste your JWT token here",
    };
    c.AddSecurityDefinition("Bearer", bearer);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            []
        }
    });
});

// ── Build ─────────────────────────────────────────────────────────────────────
var app = builder.Build();

// Apply migrations on startup (EnsureCreated for PostgreSQL, Migrate for SQLite)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (databaseUrl != null)
        dbContext.Database.EnsureCreated();
    else
        dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("VueDev");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
