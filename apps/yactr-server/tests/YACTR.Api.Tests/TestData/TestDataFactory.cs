using NetTopologySuite.Geometries;

namespace YACTR.Api.Tests.TestData;

/// <summary>
/// Factory for creating test data objects.
/// This is used to create test data objects (without persisting them to the database)
/// for use in the test suite.
/// </summary>
/// <param name="geometryFactory"></param>
public class TestDataFactory(GeometryFactory geometryFactory)
{
    public Point NewPoint()
    {
        return NewPoint(-122.4194, 37.7749);
    }

    public Point NewPoint(double longitude, double latitude)
    {
        return geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
    }

    public Polygon NewPolygon()
    {
        return NewPolygon(-122.42, 37.77, -122.41, 37.78);
    }

    public Polygon NewPolygon(double minLon, double minLat, double maxLon, double maxLat)
    {
        return geometryFactory.CreatePolygon([
            new Coordinate(minLon, minLat),
            new Coordinate(minLon, maxLat),
            new Coordinate(maxLon, maxLat),
            new Coordinate(maxLon, minLat),
            new Coordinate(minLon, minLat)
        ]);
    }

    public MultiPolygon NewMultiPolygon()
    {
        return geometryFactory.CreateMultiPolygon([
            NewPolygon()
        ]);
    }

    public MultiPolygon NewMultiPolygon(double minLon, double minLat, double maxLon, double maxLat)
    {
        return geometryFactory.CreateMultiPolygon([
            NewPolygon(minLon, minLat, maxLon, maxLat)
        ]);
    }

    /// <summary>
    /// Returns an empty Point (IsEmpty == true). For use in validation tests.
    /// </summary>
    public Point EmptyPoint() => geometryFactory.CreatePoint();

    /// <summary>
    /// Returns an empty MultiPolygon (IsEmpty == true). For use in validation tests.
    /// </summary>
    public MultiPolygon EmptyMultiPolygon() => geometryFactory.CreateMultiPolygon();

    public LineString NewLineString()
    {
        return geometryFactory.CreateLineString([
            new Coordinate(-122.42, 37.77),
            new Coordinate(-122.42, 37.78),
        ]);
    }
}