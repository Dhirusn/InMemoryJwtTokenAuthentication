using IdentityInMemoryTest.Models;
using IdentityInMemoryTest.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ApplicationDbContext>(config =>
{
    config.UseInMemoryDatabase("MemoryDatabase");
});

builder.Services.AddIdentity<AppUser, IdentityRole>(config =>
{
    // User defined password policy settings.  
    config.Password.RequiredLength = 4;
    config.Password.RequireDigit = false;
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = "JWTIssuer",
        ValidAudience = "JWTIssuer",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JwtKeyqwertyupoiuhygtfrd"))
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    // Bearer token authentication
    var securityDefinition = new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        BearerFormat = "JWT",
        Scheme = "Bearer",
        Description = "Specify the authorization token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
    };
    c.AddSecurityDefinition("Bearer", securityDefinition);

    //New code to work with .NET6
    //I've splitted in two parts for better reading
    var securityRequirement = new OpenApiSecurityRequirement();
    var secondSecurityDefinition = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    securityRequirement.Add(secondSecurityDefinition, Array.Empty<string>());
    c.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddTransient<IJwtTokenHandler, JwtTokenHandler>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors(x => x
           .AllowAnyMethod()
           .AllowAnyHeader()
           .SetIsOriginAllowed(origin => true) // allow any origin
           .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
