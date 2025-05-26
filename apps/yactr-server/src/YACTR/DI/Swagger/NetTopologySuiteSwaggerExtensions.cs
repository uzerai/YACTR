using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using NetTopologySuite.Geometries;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace YACTR.DI.Swagger;

/// <summary>
/// Provides swagger mappings for the NetTopologySuite.IO.Converters.GeoJson types, 
/// as for some reason the default mappings are not correctly identified by the swagger auto-configuration.
/// </summary>
public static class NetTopologySuiteSwaggerExtensions
{
  /// <summary>
  /// Defines the custom mappings for the NetTopologySuite types to GeoJson types; 
  /// as supported by the NetTopologySuite.IO.Converters.GeoJsonConverter.
  /// </summary>
  /// <param name="options"></param>
  public static void SimplifyNetTopologySuiteTypes(this SwaggerGenOptions options)
  {
    options.MapType<CoordinateZ>(() => new() {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema> {
          { "x", new() { Type = "number" } },
          { "y", new() { Type = "number" } },
          { "z", new() { Type = "number" } },
        },
    });

    options.MapType<Point>(() => new() {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema> {
          { "type", new() { Type = "string", Default = new OpenApiString("Point") } },
          { "coordinates", new() { 
            Type = "array",
            MaxLength = 3,
            Items = new() { 
              Type = "array", 
              Items = new() { 
                Type = "number" 
              } 
            } 
          } 
        },
      },
    });

    options.MapType<Polygon>(() => new() {
      Type = "object",
      Properties = new Dictionary<string, OpenApiSchema> {
        { "type", new() { Type = "string", Default = new OpenApiString("Polygon") } },
        { 
          "coordinates", new() { 
          Type = "array",
          Items = new() { 
              Type = "array",
              Items = new() { 
                Type = "array",
                MaxLength = 3,
                Items = new() { Type = "number" } 
              }
            }
          }
        }
      }
    });

    options.MapType<MultiPolygon>(() => new() {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema> {
          { "type", new() { Type = "string", Default = new OpenApiString("MultiPolygon") } },
          { "coordinates", new() { 
              Type = "array",
              Items = new() {
                Type = "array",
                Items = new() { 
                  Type = "array",
                  Items = new() { 
                    Type = "array",
                    MaxLength = 3,
                    Items = new() { Type = "number" } 
                  } 
                } 
              } 
            } 
          },
        },
    });
  }
}
