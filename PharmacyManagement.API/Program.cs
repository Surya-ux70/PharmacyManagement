using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PharmacyManagement.API.Data;
using PharmacyManagement.API.Models;
using PharmacyManagement.API.Services;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(o =>
    o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantService, TenantService>();

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string is not configured.");

if (connectionString.StartsWith("postgresql://") || connectionString.StartsWith("postgres://"))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    var dbPort = uri.Port > 0 ? uri.Port : 5432;
    connectionString = $"Host={uri.Host};Port={dbPort};Database={uri.AbsolutePath.TrimStart('/')}"
        + $";Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}

builder.Services.AddDbContext<PharmacyDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<PharmacyDbContext>()
.AddDefaultTokenProviders();

var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
    ?? builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:4200" };
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<PharmacyDbContext>();

    if (db.Database.CanConnect())
    {
        db.Database.ExecuteSqlRaw(@"
            DO $$
            BEGIN
                IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema='public' AND table_name='Products') THEN
                    CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
                        ""MigrationId"" character varying(150) NOT NULL,
                        ""ProductVersion"" character varying(32) NOT NULL,
                        CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY (""MigrationId"")
                    );
                    INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
                    VALUES ('20260414064740_InitialSchema', '6.0.29')
                    ON CONFLICT DO NOTHING;
                    INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
                    VALUES ('20260414064801_AddIdentity', '6.0.29')
                    ON CONFLICT DO NOTHING;
                END IF;
            END $$;");
    }

    db.Database.Migrate();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "SuperAdmin", "Admin", "Pharmacist" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var superAdminEmail = Environment.GetEnvironmentVariable("SUPERADMIN_EMAIL") ?? "superadmin@pharmacare.com";
    var superAdminPassword = Environment.GetEnvironmentVariable("SUPERADMIN_PASSWORD") ?? "Super@123";

    if (await userManager.FindByEmailAsync(superAdminEmail) == null)
    {
        var superAdmin = new ApplicationUser
        {
            UserName = superAdminEmail,
            Email = superAdminEmail,
            FullName = "PharmaCare Platform Admin",
            EmailConfirmed = true,
            TenantId = null
        };
        var result = await userManager.CreateAsync(superAdmin, superAdminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
    }

    // Migrate the old admin user: if it exists and has no role upgrade, ensure it stays as-is
    var oldAdminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@pharmacare.com";
    var oldAdmin = await userManager.FindByEmailAsync(oldAdminEmail);
    if (oldAdmin != null && !await userManager.IsInRoleAsync(oldAdmin, "SuperAdmin"))
    {
        if (!await userManager.IsInRoleAsync(oldAdmin, "Admin"))
            await userManager.AddToRoleAsync(oldAdmin, "Admin");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var port = Environment.GetEnvironmentVariable("PORT") ?? "5268";
app.Urls.Add($"http://0.0.0.0:{port}");

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
