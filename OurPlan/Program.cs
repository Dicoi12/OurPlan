using Microsoft.EntityFrameworkCore;
using OurPlan.Data;
using OurPlan.Services.Interfaces;
using OurPlan.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OurPlan.Helpers;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AutoMapper;


var builder = WebApplication.CreateBuilder(args);

// ==================== Database ====================
// decode connection string if it's base64, otherwise use raw value
var rawConn = builder.Configuration.GetConnectionString("DefaultConnection");
string connectionString;

try
{
    var bytes = Convert.FromBase64String(rawConn);
    connectionString = Encoding.UTF8.GetString(bytes);
}
catch (FormatException)
{
    connectionString = rawConn;
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// ==================== JWT Authentication ====================
var jwtSettings = builder.Configuration.GetSection("Jwt");


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAuthorization();

// ==================== AutoMapper & Services ====================
builder.Services.AddAutoMapper(typeof(GenericProfile));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IGroupTokenService, GroupTokenService>();
builder.Services.AddScoped<IGroupTaskService, GroupTaskService>();
builder.Services.AddHttpContextAccessor();
// ==================== Controllers with Global [Authorize] ====================
builder.Services.AddControllers(config =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});

// ==================== Swagger Configuration ====================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OurPlan API",
        Version = "v1",
        Description = "API pentru aplicația OurPlan"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Introduceți 'Bearer' urmat de spațiu și tokenul JWT.\nExemplu: \"Bearer eyJhbGci...\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ==================== Build & Configure Middleware ====================
var app = builder.Build();

// Apply any pending EF Core migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate(); // runs pending migrations
        logger.LogInformation("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw;
    }
}
// Swagger UI pentru toate mediile (nu doar Development)


// Middleware pipeline
app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OurPlan API v1");
    c.RoutePrefix = string.Empty; // opțional - deschide direct Swagger la rădăcină
});

app.MapControllers();

app.Run();
