using System.ComponentModel.DataAnnotations;

using NetTopologySuite.Geometries;

namespace YACTR.Domain.Model;

public class CountryData
{
    [Key]
    [Required]
    public int Id { get; set; }
    public required string CountryName { get; set; }
    public required string AdminName { get; set; }
    public required string Code { get; set; }
    public required string Continent { get; set; }
    public required string Region { get; set; }
    public required string Subregion { get; set; }
    public required string WorldBlock { get; set; }
    public required MultiPolygon Geometry { get; set; }
}