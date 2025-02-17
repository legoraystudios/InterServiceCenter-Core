using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using InterServiceCenter_Core.Contexts;
using InterServiceCenter_Core.Services;
using InterServiceCenter_Core.Utilities;
using InterServiceCenter_Core.Utilities.Authorization;
using InterServiceCenter_Core.Utilities.SMTPMail;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Get JWT Settings from appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Configuring authentication and JWT Bearer
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
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };

        // Custom Validation for the JWT Bearer Token
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

                // Get JwtToken Service (To make additional verification for the Bearer Token)
                var tokenService = context.HttpContext.RequestServices.GetRequiredService<JwtToken>();
                var verify = tokenService.VerifyBearer(claimsIdentity);

                if (claimsIdentity != null)
                    if (!verify)
                        // If no deviceId is found, invalidate the token immediately
                        context.Fail("Unauthorized - Invalid Token.");

                return Task.CompletedTask;
            }
        };
    });

// Adding the authorization
builder.Services.AddAuthorization(options =>
{
    // Adding Roles
    options.AddPolicy("AdminRole", policy =>
    {
        policy.Requirements.Add(new RoleRequirement("Admin", "Super Administrator"));
    });
});

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<StatusBarService>();
builder.Services.AddScoped<MetricService>();
builder.Services.AddScoped<FacilityService>();
builder.Services.AddScoped<DirectoryService>();
builder.Services.AddScoped<JwtToken>();
builder.Services.AddScoped<SmtpTool>();
builder.Services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();
builder.Services.AddScoped<GeneralUtilities>();

// Add Database Connection String to DBContext
builder.Services.AddDbContext<InterServiceCenterContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultDatabaseConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var urlBasePath = builder.Configuration["UrlBasePath"];

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
//app.UseHttpsRedirection();
app.UsePathBase(urlBasePath);
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.UseHttpsRedirection();
app.Run();