using System.Text.Json.Serialization;

namespace YACTR.Data.Model.Climbing;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ClimbingType
{
    [JsonStringEnumMemberName("Sport")]
    Sport,
    [JsonStringEnumMemberName("Traditional")]
    Traditional,
    [JsonStringEnumMemberName("Boulder")]
    Boulder,
    [JsonStringEnumMemberName("Mixed")]
    Mixed,
    [JsonStringEnumMemberName("Aid")]
    Aid
}
