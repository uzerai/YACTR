using System.Text.Json.Serialization;

namespace YACTR.Domain.Model.Climbing.Topo;

public class TopoLinePoint
{
    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }
}