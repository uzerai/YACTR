using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace YACTR.Data.Model;

[PrimaryKey(nameof(Id))]
public abstract partial class BaseEntity
{
    [JsonPropertyOrder(-1)]
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
