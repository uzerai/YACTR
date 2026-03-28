using NetTopologySuite.Geometries;

namespace YACTR.Domain.Model;

public class CountryData
{
    public required int Id;
    public required string CountryName;
    public required string AdminName;
    public required string Code;
    public required string Continent;
    public required string Region;
    public required string Subregion;
    public required string WorldBlock;
    public required MultiPolygon Geometry;
}