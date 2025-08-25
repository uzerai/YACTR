using System.Text.Json.Serialization;

namespace YACTR.Data.Model.Climbing;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ClimbingType
{
    [JsonStringEnumMemberName("sport")]
    Sport,
    [JsonStringEnumMemberName("traditional")]
    Traditional,
    [JsonStringEnumMemberName("boulder")]
    Boulder,
    [JsonStringEnumMemberName("mixed")]
    Mixed,
    [JsonStringEnumMemberName("aid")]
    Aid
}
