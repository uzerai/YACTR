using Microsoft.EntityFrameworkCore;
using NodaTime;
using Npgsql;
using YACTR.DI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using NodaTime.Serialization.SystemTextJson;
using YACTR.DI.Repository.ConfigurationExtension;
using System.Text.Json;
using YACTR.DI.Authorization.ConfigurationExtension;
using NetTopologySuite;
using NetTopologySuite.IO.Converters;
using YACTR.DI.Swagger;
using Minio;
using YACTR.DI.Service;
using YACTR.DI.Authorization.UserContext;
using FileSignatures;

// ############################################################
// ##########  APP BUILDING  ##################################
// ############################################################
///
/// This is where the NuGet packages are configured and non-internal services are registered.
/// If you are adding a new NuGet package, please add it to this section, and register all other 
/// injected services that are internally developed as part of the API in the section below this one.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SimplifyNetTopologySuiteTypes();
});

/// Authentication extraction through JWT Bearer tokens.
/// Intended to be used with the corresponding Auth0 tenant; but 
/// technically be used with any Oauth2.0 compliant identity provider.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

// Add NodaTime clock service so we can use it in the database context for timestamping BaseEntity objects.
// This is the SystemClock for the running version of the server.
// Replace it in the test environment to be whatever you wish.
builder.Services.AddSingleton<IClock>(SystemClock.Instance);

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

/// Json setup specifically for the support of NodaTime serialization.
/// Also sets the property naming policy to snake_case, because it's the nicer json format.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    
    options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
});

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
builder.Services.AddUserContext();
builder.Services.AddPermissionsAuthorizationHandling();
builder.Services.AddApplicationServices();

// ############################################################
// ##########  APP INITIALIZATION  ############################
// ############################################################
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

// HTTPs redirection by default.
// App library & overarching middleware registration.
app.UseRouting();

// Authentication middleware.
app.UseAuthentication();

// Add user context middleware
app.UseUserContext();

// Authorization middleware.
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
