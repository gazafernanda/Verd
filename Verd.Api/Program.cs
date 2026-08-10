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

// Apply migrations on startup (EnsureCreated for PostgreSQL, Migrate for SQLite).
// The container can boot before outbound DNS is ready, so the first connection
// may fail with a transient socket error. Retry a few times, and never let this
// take the whole service down — the app can still start and connect later.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var startupLog = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

    const int maxAttempts = 5;
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            if (databaseUrl != null)
            {
                dbContext.Database.EnsureCreated();
                // EnsureCreated() does NOT evolve the schema of a database that already
                // exists, so additive columns introduced after the first deploy must be
                // reconciled by hand. These statements are idempotent and safe to repeat.
                dbContext.Database.ExecuteSqlRaw(
                    """ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "AvatarUrl" text NOT NULL DEFAULT '';""");
                dbContext.Database.ExecuteSqlRaw(
                    """ALTER TABLE "Plants" ADD COLUMN IF NOT EXISTS "LastWateredAt" timestamp with time zone NULL;""");
                dbContext.Database.ExecuteSqlRaw(
                    """ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "Role" text NOT NULL DEFAULT 'Gardener';""");
                dbContext.Database.ExecuteSqlRaw(
                    """
                    CREATE TABLE IF NOT EXISTS "SystemSettings" (
                        "Id" serial PRIMARY KEY,
                        "Key" text NOT NULL,
                        "Value" text NOT NULL,
                        "UpdatedAt" timestamp with time zone NOT NULL
                    );
                    """);
                dbContext.Database.ExecuteSqlRaw(
                    """CREATE UNIQUE INDEX IF NOT EXISTS "IX_SystemSettings_Key" ON "SystemSettings" ("Key");""");
            }
            else
            {
                dbContext.Database.Migrate();
            }

            // Accounts that predate the Role column land with an empty string,
            // which matches neither role — normalise them to the default.
            dbContext.Database.ExecuteSqlRaw(
                """UPDATE "Users" SET "Role" = 'Gardener' WHERE "Role" IS NULL OR "Role" = '';""");

            // Bootstrapping an admin: there is no UI for granting the first one,
            // so promote the account named by ADMIN_EMAIL on every start.
            var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL")?.Trim();
            if (!string.IsNullOrWhiteSpace(adminEmail))
            {
                // Match loosely: a stray space or different casing in the dashboard
                // value should still find the account rather than silently do nothing.
                var admin = dbContext.Users
                    .FirstOrDefault(u => u.Email.ToLower() == adminEmail.ToLower());

                if (admin is null)
                {
                    startupLog.LogWarning(
                        "ADMIN_EMAIL is set to '{Email}' but no such user exists — register that " +
                        "account first, then restart. Known emails: {Emails}",
                        adminEmail,
                        string.Join(", ", dbContext.Users.Select(u => u.Email).Take(20)));
                }
                else if (admin.Role != "Admin")
                {
                    admin.Role = "Admin";
                    dbContext.SaveChanges();
                    startupLog.LogInformation("Promoted {Email} to Admin.", admin.Email);
                }
                else
                {
                    startupLog.LogInformation("{Email} is already an Admin.", admin.Email);
                }
            }
            else
            {
                var adminCount = dbContext.Users.Count(u => u.Role == "Admin");
                if (adminCount == 0)
                    startupLog.LogWarning(
                        "No admin account exists and ADMIN_EMAIL is not set — the admin console " +
                        "is unreachable. Set ADMIN_EMAIL to a registered address and restart.");
            }

            startupLog.LogInformation("Database bootstrap succeeded on attempt {Attempt}.", attempt);
            break;
        }
        catch (Exception ex) when (attempt < maxAttempts)
        {
            var delay = TimeSpan.FromSeconds(2 * attempt);
            startupLog.LogWarning(ex,
                "Database bootstrap attempt {Attempt}/{Max} failed; retrying in {Seconds}s.",
                attempt, maxAttempts, delay.TotalSeconds);
            Thread.Sleep(delay);
        }
        catch (Exception ex)
        {
            startupLog.LogError(ex,
                "Database bootstrap failed after {Max} attempts. Starting anyway — requests that " +
                "need the database will fail until it becomes reachable.", maxAttempts);
        }
    }
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
