using System.Text.Json.Serialization;

namespace YACTR.Domain.Model.Climbing;

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
