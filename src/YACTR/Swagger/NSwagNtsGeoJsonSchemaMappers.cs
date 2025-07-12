using NetTopologySuite.Geometries;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;

namespace YACTR.Swagger;

/// <summary>
/// <para>
/// This class exists solely to provide correct schema generation for the 
///   <c>NetTopologySuite.IO.Converters.GeoJsonConverterFactory</c>
/// geojson formats.
/// </para>
///
/// <para>It exposes an extension method for adding the mappers to the NSwag generator TypeMappers definition list.</para>
/// 
/// <para>This is required solely as NSwag <see href="https://github.com/RicoSuter/NSwag/issues/2243">does not work with <c>System.Text.Json</c></see>, but using Newtonsoft.Json is out of the question
/// for serializing/deserializing in the http handler.</para>
/// <remarks>
/// The implementation below is based on the <see href="https://fast-endpoints.com/docs/swagger-support#swagger-serializer-options">FastEndpoints</see>
/// documentation and the custom typemapper definition examples in <seealso href="https://github.com/RicoSuter/NJsonSchema/wiki/Type-Mappers">NJsonSchema</seealso>
/// </remarks>
/// </summary>
public static class NSwagNtsGeoJsonSchemaMappers
{
  /// <summary>
  /// Registers a handful of ObjectTypeMappers to the JsonSchemaGeneratorSettings for NSwag to
  /// force the swagger open api spec to conform to what's expected of the endpoints which 
  /// use the NetTopologySuite GeoJson format.
  /// </summary>
  /// <param name="schemaGeneratorSettings"></param>
  public static void AddNtsGeoJsonSchemas(this JsonSchemaGeneratorSettings schemaGeneratorSettings)
  {
    schemaGeneratorSettings.TypeMappers.Add(
        new ObjectTypeMapper(typeof(Point), new JsonSchema()
        {
          Type = JsonObjectType.Object,
          Properties = {
                {
                    "type",
                    new () {
                        Type = JsonObjectType.String,
                        Default = "Point"
                    }
                },
                {
                    "coordinates",
                    new ()
                    {
                        Type = JsonObjectType.Array,
                        MaxItems = 3,
                        MinItems = 3,
                        Default = "[11.11, 22.22, 33.33]",
                        Item = new() {
                            Type = JsonObjectType.Number,
                        }
                    }
                }
            },
        })
    );

    schemaGeneratorSettings.TypeMappers.Add(
        new ObjectTypeMapper(typeof(Polygon), new JsonSchema()
        {
          Type = JsonObjectType.Object,
          Properties = {
                {
                    "type",
                    new () {
                        Type = JsonObjectType.String,
                        Default = "Polygon"
                    }
                },
                {
                    "coordinates",
                    new ()
                    {
                        Type = JsonObjectType.Array,
                        MinItems = 3,
                        Item = new() {
                            Type = JsonObjectType.Array,
                            MaxItems = 3,
                            Default = "[ 11.11, 22.22, 33.33 ]",
                            Item = new() {
                              Type = JsonObjectType.Number
                            }
                        }
                    }
                }
            },
        })
    );

    schemaGeneratorSettings.TypeMappers.Add(
        new ObjectTypeMapper(typeof(LineString), new JsonSchema()
        {
          Type = JsonObjectType.Object,
          Properties = {
                {
                    "type",
                    new () {
                        Type = JsonObjectType.String,
                        Default = "LineString"
                    }
                },
                {
                    "coordinates",
                    new ()
                    {
                        Type = JsonObjectType.Array,
                        MinItems = 2,
                        Item = new() {
                            Type = JsonObjectType.Array,
                            MaxItems = 3,
                            Default = "[ 11.11, 22.22, 33.33 ]",
                            Item = new() {
                              Type = JsonObjectType.Number
                            }
                        }
                    }
                }
            },
        })
    );

    schemaGeneratorSettings.TypeMappers.Add(
        new ObjectTypeMapper(typeof(MultiPolygon), new JsonSchema()
        {
          Type = JsonObjectType.Object,
          Properties = {
                {
                    "type",
                    new () {
                        Type = JsonObjectType.String,
                        Default = "MultiPolygon"
                    }
                },
                {
                    "coordinates",
                    new ()
                    {
                        Type = JsonObjectType.Array,
                        Item = new() {
                            Type = JsonObjectType.Array,
                            MinItems = 3,
                            Item = new() {
                              Type = JsonObjectType.Array,
                              MaxItems = 3,
                              Default = "[ 11.11, 22.22, 33.33 ]",
                              Item = new() {
                                Type = JsonObjectType.Number
                              }
                          }
                        }
                    }
                }
            },
        })
    );
  }
}