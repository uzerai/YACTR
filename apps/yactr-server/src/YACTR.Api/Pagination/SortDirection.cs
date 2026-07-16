using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace YACTR.Api.Pagination;

/// <summary>
/// Sort direction for endpoints supporting sorting via <see cref="SortedPaginationRequest{TSortBy}"/>.
/// <see cref="EnumMemberAttribute"/> provides the snake_case wire names in the OpenAPI schema (read by NJsonSchema),
/// while <see cref="JsonStringEnumMemberNameAttribute"/> keeps runtime JSON serialization consistent with the schema.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortDirection
{
    [EnumMember(Value = "asc")]
    [JsonStringEnumMemberName("asc")]
    Asc,

    [EnumMember(Value = "desc")]
    [JsonStringEnumMemberName("desc")]
    Desc,
}
