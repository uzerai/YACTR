using System.Text.Json.Serialization;

namespace YACTR.Model.Location;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PitchType
{
    [JsonStringEnumMemberName("sport")]
    Sport,
    [JsonStringEnumMemberName("traditional")]
    Traditional,
    [JsonStringEnumMemberName("mixed")]
    Mixed,
    [JsonStringEnumMemberName("aid")]
    Aid,
}