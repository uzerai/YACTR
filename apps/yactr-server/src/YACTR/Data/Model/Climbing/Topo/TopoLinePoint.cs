using System.Text.Json.Serialization;

namespace YACTR.Data.Model.Climbing.Topo;

public class TopoLinePoint
{
    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }
}