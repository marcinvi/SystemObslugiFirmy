using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ReklamacjeAPI.Data;
using ReklamacjeAPI.Security;
using ReklamacjeAPI.Services;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Reklamacje API",
        Version = "v1",
        Description = "REST API dla systemu obsługi reklamacji"
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Wprowadź JWT token w formacie: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersionString = builder.Configuration["DatabaseSettings:ServerVersion"] ?? "10.6.14-MariaDB";
var serverVersion = ServerVersion.Parse(serverVersionString);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IZgloszeniaService, ZgloszeniaService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IDzialanieService, DzialanieService>();
builder.Services.AddScoped<ReturnsService>();
builder.Services.AddScoped<NotificationsService>();
builder.Services.AddSingleton<ReturnSyncProgressService>();
builder.Services.AddScoped<MessagesService>();
builder.Services.AddHttpClient<AllegroApiClient>();
builder.Services.AddScoped<AllegroCredentialsService>();
builder.Services.AddScoped<ModulesService>();    // <--- TEGO BRAKUJE dla modułów
builder.Services.AddScoped<MessagesService>();   // <--- To będzie potrzebne dla Wiadomości
builder.Services.AddScoped<FileService>();
builder.Services.Configure<FileStorageOptions>(builder.Configuration.GetSection("FileStorage"));

var masterKeySecret = builder.Configuration["Security:MasterKeySecret"];
if (string.IsNullOrWhiteSpace(masterKeySecret))
{
    throw new InvalidOperationException("Brak konfiguracji Security:MasterKeySecret. Ustaw klucz główny szyfrowania przed uruchomieniem API.");
}

using (var kdf = new Rfc2898DeriveBytes(
    masterKeySecret,
    EncryptionHelper.Salt,
    50_000,
    HashAlgorithmName.SHA256))
{
    EncryptionHelper.MasterKey = kdf.GetBytes(32);
}
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Reklamacje API v1");
        options.RoutePrefix = string.Empty; // Swagger UI at root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow 
}));

app.Run();
