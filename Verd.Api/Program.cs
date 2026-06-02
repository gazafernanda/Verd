using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Verd.Api.Data;
using Verd.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Use AddJsonStream to bypass FileProvider restrictions in certain environments
// (the PhysicalFileProvider can't resolve paths inside .claude/worktrees/).
var settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
if (File.Exists(settingsPath))
    builder.Configuration.AddJsonStream(File.OpenRead(settingsPath));
var envSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), $"appsettings.{builder.Environment.EnvironmentName}.json");
if (File.Exists(envSettingsPath))
    builder.Configuration.AddJsonStream(File.OpenRead(envSettingsPath));

// ── Database ─────────────────────────────────────────────────────────────────
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (databaseUrl != null)
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var connStr = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};" +
                      $"Username={userInfo[0]};Password={Uri.UnescapeDataString(userInfo[1])};" +
                      "SSL Mode=Require;Trust Server Certificate=true";
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

builder.Services.AddHttpClient("Groq", client =>
{
    client.BaseAddress = new Uri("https://api.groq.com/openai/v1/");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["Groq:ApiKey"]}");
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
