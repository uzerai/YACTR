using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NodaTime;

namespace YACTR.Domain.Model;

public abstract class BaseEntity
{
    [Key]
    [JsonPropertyOrder(-1)]
    [Required]
    public Guid Id { get; set; }

    [JsonPropertyOrder(2)]
    [Required]
    public Instant UpdatedAt { get; set; }

    [JsonPropertyOrder(3)]
    [Required]
    public Instant CreatedAt { get; set; }

    [JsonIgnore]
    public Instant? DeletedAt { get; set; }
}
