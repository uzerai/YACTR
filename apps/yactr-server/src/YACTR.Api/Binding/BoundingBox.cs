using System.Globalization;
using Microsoft.Extensions.Primitives;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using ParseResult = FastEndpoints.ParseResult;

namespace YACTR.Api.Binding;

/// <summary>
/// WGS 84 bounding box in OGC API Features <c>bbox</c> order:
/// <c>minLon,minLat,maxLon,maxLat</c>.
/// </summary>
public sealed record BoundingBox(double MinLon, double MinLat, double MaxLon, double MaxLat)
{
    public Geometry ToPolygon()
    {
        var factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        return factory.ToGeometry(new Envelope(MinLon, MaxLon, MinLat, MaxLat));
    }

    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"{MinLon},{MinLat},{MaxLon},{MaxLat}");
}

/// <summary>
/// Parses OGC-style <c>bbox=minLon,minLat,maxLon,maxLat</c> query values into <see cref="BoundingBox"/>.
/// </summary>
public static class BoundingBoxParser
{
    public static ParseResult ParseResult(StringValues input)
    {
        var value = input.ToString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return new ParseResult(false, null);
        }

        var parts = value.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length != 4)
        {
            return new ParseResult(false, null);
        }

        if (!TryParseCoordinate(parts[0], out var minLon)
            || !TryParseCoordinate(parts[1], out var minLat)
            || !TryParseCoordinate(parts[2], out var maxLon)
            || !TryParseCoordinate(parts[3], out var maxLat))
        {
            return new ParseResult(false, null);
        }

        return new ParseResult(true, new BoundingBox(minLon, minLat, maxLon, maxLat));
    }

    private static bool TryParseCoordinate(string value, out double result)
    {
        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
        {
            return false;
        }

        return double.IsFinite(result);
    }
}
