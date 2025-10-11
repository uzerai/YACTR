using System.Security.Claims;
using System.Text.Json;
using FastEndpoints;
using FastEndpoints.Swagger;
using FileSignatures;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Minio;
using NetTopologySuite;
using NetTopologySuite.IO.Converters;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Npgsql;
using NSwag;
using YACTR.Data;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Repository.ConfigurationExtension;
using YACTR.DI.Authorization.ConfigurationExtension;
using YACTR.DI.Authorization.Permissions;
using YACTR.DI.Service;
using YACTR.Swagger;

// ############################################################
// ##########  APP BUILDING  ##################################
// ############################################################
///
/// This is where the NuGet packages are configured and non-internal services are registered.
/// If you are adding a new NuGet package, please add it to this section, and register all other 
/// injected services that are internally developed as part of the API in the section below this one.
var builder = WebApplication.CreateBuilder(args);

/// ############################################################
/// ##########  CUSTOM SERVICES SETUP  #########################
/// ############################################################
/// 
/// Repository setup and registration done here;
/// 
/// If you are adding a new repository:
/// Please add it to the extension method in
///     YACTR.DI.Repository.ConfigurationExtension.RepositoryServiceConfigurationExtensions
/// instead of adding them here.
/// 
builder.Services.AddRepositories();
// The claims transformation is what provides the user permissions to our custom local
// authorization handling that checks the database user _post_ validating authentication with
// our third party IDP.
builder.Services.AddTransient<IClaimsTransformation, DatabaseUserPermissionClaimsTransformer>();
builder.Services.AddPermissionsAuthorizationHandling();
builder.Services.AddApplicationServices();

/// Authentication extraction through JWT Bearer tokens.
/// Intended to be used with the corresponding Auth0 tenant; but 
/// technically be used with any Oauth2.0 compliant identity provider.
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth0:Domain"];
        options.Audience = builder.Configuration["Auth0:Audience"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,

            NameClaimType = ClaimTypes.NameIdentifier,
            ValidIssuer = builder.Configuration["Auth0:Issuer"],
            ValidAudience = builder.Configuration["Auth0:Audience"],
        };
    });
builder.Services.AddAuthorization();

// CORS setup based on AllowedHosts from configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("ConfiguredCors", policy =>
    {
        var allowedHosts = builder.Configuration["AllowedHosts"];
        if (string.IsNullOrWhiteSpace(allowedHosts) || allowedHosts == "*")
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            var origins = allowedHosts
                .Split([';', ',', ' ', '\t', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
                .Select(h => h.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || h.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ? h : $"https://{h}")
                .ToArray();

            policy
                .WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

builder.Services
    .AddFastEndpoints()
    // This configures the JSON serialization in the generated schema.
    .SwaggerDocument(swaggerSettings =>
    {
        swaggerSettings.SerializerSettings = serializerSettings =>
        {
            serializerSettings.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            serializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            serializerSettings.Converters.Add(new GeoJsonConverterFactory());
        };

        swaggerSettings.DocumentSettings = docSettings =>
        {
            docSettings.AddSecurity("JWTBearerAuth", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Bearer auth using the Authorization header."
            });
            /**
            * <see cref="NSwagNtsGeoJsonSchemaMappers"/>
            */
            docSettings.SchemaSettings.AddNtsGeoJsonSchemas();
        };
    })
    // This configures the serialization between Entity <-> JSON.
    .ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.SerializerOptions.Converters.Add(new GeoJsonConverterFactory());
        options.SerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    });

// Add NodaTime clock service so we can use it in the database context for timestamping BaseEntity objects.
// This is the SystemClock for the running version of the server.
// Replace it in the test environment to be whatever you wish.
builder.Services.AddSingleton<IClock>(NodaTime.SystemClock.Instance);

// Add NetTopologySuite.IO.Converters.GeoJsonConverterFactory to the service container.
builder.Services.AddSingleton(NtsGeometryServices.Instance);

// Add MinIO Client service.
builder.Services.AddMinio(configureClient => configureClient
    .WithEndpoint(builder.Configuration["Minio:Endpoint"])
    .WithCredentials(
        builder.Configuration["Minio:AccessKey"],
        builder.Configuration["Minio:SecretKey"])
    .WithSSL(false)  // Set to true if your MinIO server uses HTTPS
    .Build());

// Add FileSignatures inspector service.
builder.Services.AddSingleton<IFileFormatInspector>(new FileFormatInspector());

// Add main application database context.
// Will contain references to all entities in the application.
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseContextConnection"), npgsqlSourceBuilder =>
    {
        // Additional configuration for the Npgsql connection.
        // Such as, enum mappings:
        npgsqlSourceBuilder.MapEnum<ClimbingType>("climbing_type");

        // Here used to enable NodaTime support.
        npgsqlSourceBuilder.UseNodaTime();
        npgsqlSourceBuilder.MigrationsHistoryTable("migrations");

        // Enable NetTopologySuite
        npgsqlSourceBuilder.UseNetTopologySuite();

        // Enable dynamic JSON support, allowing JSON B columns.
        npgsqlSourceBuilder.ConfigureDataSource(source => source.EnableDynamicJson());

    })
    .UseSnakeCaseNamingConvention();
});

// ############################################################
// ##########  APP INITIALIZATION  ############################
// ############################################################
var app = builder.Build();

// If we're not in dev, just use https.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Enable CORS before auth so preflight requests succeed
app.UseCors("ConfiguredCors");

app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints()
    .UseSwaggerGen();

app.Run();

public partial class Program { }
