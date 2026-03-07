using NetTopologySuite.Geometries;

namespace YACTR.Api.Tests;

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
        return geometryFactory.CreatePoint(
            new Coordinate(-122.4194, 37.7749)
        );
    }

    public Polygon NewPolygon()
    {
        return geometryFactory.CreatePolygon([
            new Coordinate(-122.42, 37.77),
            new Coordinate(-122.42, 37.78),
            new Coordinate(-122.41, 37.78),
            new Coordinate(-122.41, 37.77),
            new Coordinate(-122.42, 37.77)
        ]);
    }

    public MultiPolygon NewMultiPolygon()
    {
        return geometryFactory.CreateMultiPolygon([
            NewPolygon()
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